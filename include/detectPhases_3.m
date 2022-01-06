function [data] = detectPhases_3(data_in)
    
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   GAIT PHASE RECOGNITION: OFFLINE AUTOMATIC THREASHOLD METHOD
    % ---------------------------------------------------------------------
   
   %% LOAD
    
    % select the signal of interest: GyroZ
    sig = data_in.("GyroZ (deg/s)");
    
    % Time vector [ms * 10]
    t = 1:numel(sig);
    
    % length of signal
    L = length(t);
  
    %% THRESHOLD METHOD
  
    %{
            Step  
    
              1.     filter with a low-pass filter to find only one peak for 
                     period, the cutoff freq must be not too small to preserve
                     a good signal
    
              2.     filter with a low-pass filter with  lower freq for phase 2
         
              3.     calculate an automatic threshold -> weighted average between
                     the positive part of the signal and its maximum 
                     create a matrix to store the conditioned signals and phases

              4.     big for loop on all the signal
         
              5.     find maximum peaks indicating MSw phase
              
              6.     once found MSw 
                     backward for loop to find the nearest minimum -> TO
  
              
              7.     once found TO
                     backward for loop to find the nearest maximum -> MSt
              
              8.     once found MSt
                     go on with forward loop from MSw to find the nearest
                     minimum -> IC
            
              9.     repeat from point 5.

    %}

    %% 1. filtering
    
    % for filtering it's used a butterworth filter
    % not too selective and with appropriate cutoff 
    % freq to preserve IC and TO

    [b_1,a_1] = butter(2, 0.3);

    sig_filt = filtfilt(b_1,a_1, sig);
    
    %% 2. filtering
    
    % cutoff freq to avoid noise in the part of the signal
    % between MSt and TO
    
    [b_2,a_2] = butter(2, 0.15);

    sig_filt_2 = filtfilt(b_2,a_2, sig);

    %% 3. threashold and sig new
    
    % initialize signal to calculate the average on the >0 part
    sig_zero = zeros(L);
    
    % initialize matrix to store signal
    sig_new = zeros(L,3);

    % fill the signal_zero and the matrix [sig_filt_1, sig_filt_2, phases]
    for i=1:L
        sig_new(i,1) = sig_filt(i);
        sig_new(i,2) = sig_filt_2(i);
        
        % split signal, save only >0 part else nan 
        if sig_filt(i) > 0
            sig_zero(i) = sig_filt(i);
        else
            sig_zero(i) = NaN;
        end
    end    
    
    % compute max of the signal
    max_sig = max(sig);
    
    % compute mean
    mean_a = mean(sig_zero,'omitnan'); 
    
    % parameter to weight the max value of the signal
    k = 0.3;
    
    % computhe threshold
    threshold = (max_sig*k + mean_a(1))/2;  % cambiato valore di soglia

    %% 5. to 9. find phases

    % flags for the phases
    MSw_found = 0;
    IC_found  = 0;
    TO_found  = 0;
    MSt_found = 0;
    
    % remember the index when TO = 1
    TO_idx = 0;

    for i=2:L-1

        if (sig_new(i,1) > sig_new(i-1,1)) && (sig_new(i,1) > sig_new(i+1,1))...  %maximum condition to find MSw
           && sig_new(i,1) > threshold && MSw_found == 0

            MSw_found = 1; % flag MSw and reset all flags
            TO_found  = 0;
            IC_found  = 0;
            MSt_found = 0;
        end

        if MSw_found 

            if TO_found == 0
                for k = i-1:-1:2 % backward for cycle to find TO
                    sig_new(k,3) = 3; % save the phase value
                    if (sig_new(k,1) < sig_new(k+1,1))&&  (sig_new(k,1) < sig_new(k-1,1)) % condition for TO
                        TO_found = 1; % flag TO
                        TO_idx = k;
                        break
                    end
                end
            end
            
            if TO_found == 1 % backward for cycle to find MSt
                for k = TO_idx-1:-1:2
                    sig_new(k,3) = 2; % save the phase value
                    if (sig_new(k,2) > sig_new(k+1,2)) &&  (sig_new(k,2) > sig_new(k-1,2)) && sig_new(k,2) > -60 % condition for MSt (-60 safety threshold)
                        MSt_found = 1; % flag MSt
                        break
                    end
                end
            end

            sig_new(i,3) = 4; % save the phase value


             if (sig_new(i,1) < sig_new(i-1,1)) && (sig_new(i,1) < sig_new(i+1,1))...
                && sig_new(i,1) < threshold %  IC condition

                IC_found = 1; % flag IC
                MSw_found = 0; % reet flag MSw

            end
        end

        if IC_found

            sig_new(i,3) = 1; % save the phase value
            
        end
        
    end
    
    % output of the function: add to the data in input the fitered signals and the phases (ID)
    data = [data_in, array2table(sig_new, 'VariableNames',{'gyro_z_filt','gyro_z_heavy_filt', 'ID'})];
    
end





















