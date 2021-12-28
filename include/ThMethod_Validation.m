 %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   GAIT PHASE RECOGNITION: OFFLINE AUTOMATIC THREASHOLD METHOD
    %   
    %   - Description of the method with the example synckedData_IMU_mitch.mat
    %     and validation of the this teoretical method through the use of 
    %     pressure sensors (mitch) 
    % ---------------------------------------------------------------------

function [] = ThMethod_Validation(data)

    %% LOAD 
    
    dataIMU = load(data);

    sig = dataIMU.GyroZ_deg_s_;

    t = 1:numel(sig);

    L = length(t);
    
    %% PLOT
        
    figure
    hold on
    plot(t,sig, 'b');
    hold off
    
    %% FFT

    T_sample = t(2) -  t(1);

    Fs = 1/T_sample;

    f = Fs*(0:(L/2))/L;
        
    Y = fft(sig);
    P2 = abs(Y/L);
    P1 = P2(1:L/2+1);
    P1(2:end-1) = 2*P1(2:end-1);

    figure 
    plot(f,P1)

    %% THRESHOLD METHOD MSw

    %{
           Step 1:  trovare il picco massimo corrispondente alla fase 
                    MSw, massima velocità di rotasione della gamba durante
                    la fase di swing

              1.1   impostare una soglia in modo automatico
                    metodo pensato: media tra il massimo valore e la media del
                    segnale, tenendo in considerazione solamente la parte
                    positiva, volendo si può introdurre un peso

              1.2   filtrare a bassa frequenza in modo da trovare un solo
                    picco, il segnale risulta essere ben definito
                    sotto una frequenza relativa di 0.3. 

              1.3   trovare la posizione dei picchi positivi, mettere flag 


           Step 2:  trovare la fase TO, distacco del piede: ricerca di un minimo
                    antecedente al punto MSw

                    ciclo partendo dalla fine del segnale che trova il minimo 
                    precedente al flag MSw

                    (se necessario filtrare a 0.3)

                    mettere flag

           Step 3:  trovare la fase IC, contatto del piede: rcerca di un minimo
                    successivo al punto MSw

                    ciclo partendo dall'inizio e del segnale che trova il minimo 
                    precedente al flag MSw

                    (se necessario filtrare a 0.3)

                    mettere flag

           Step 4:  trovare fase MSt

    %}

    %% 1.2 filtering

    [b_1,a_1] = butter(2, 0.3);

    freqz(b_1,a_1);

    sig_filt = filtfilt(b_1,a_1, sig);

    [b_2,a_2] = butter(2, 0.5);  % cambiato filtraggio

    sig_filt_2 = filtfilt(b_2,a_2, sig);

    figure
    hold on
    plot(t,sig,'b','LineWidth',1);
    plot(t,sig_filt, 'r','LineWidth',1);
    plot(t,sig_filt_2, 'k','LineWidth',1);
    xlim([1900,2100])
    hold off


    %% 1.1 threashold and sig new

    sig_zero = zeros(L);
    sig_new = zeros(L,3);

    for i=1:L
        sig_new(i,1) = sig_filt(i);
        sig_new(i,2) = sig_filt_2(i);
        if sig_filt(i) > 0
            sig_zero(i) = sig_filt(i);
        else
            sig_zero(i) = NaN;
        end
    end    

    max_sig = max(sig);

    mean_a = mean(sig_zero,'omitnan'); %% vector?
    
    k = 0.1;

    threashold = (max_sig*k + mean_a(1))/2;  % cambiato valore di soglia

    %% 1.3 find peaks

    MSw_found = 0;
    IC_found  = 0;
    TO_found  = 0;
    MSt_found = 0;
    
    TO_idx = 0;

    for i=2:L-1

        if (sig_new(i,1) > sig_new(i-1,1)) && (sig_new(i,1) > sig_new(i+1,1))...  %condizione massimo MSw
           && sig_new(i,1) > threashold && MSw_found == 0

            MSw_found = 1; % flag MSw
            TO_found  = 0;
            IC_found  = 0;
            MSt_found = 0;
        end

        if MSw_found

            if TO_found == 0
                for k = i-1:-1:2 % backward for cycle to find TO
                    sig_new(k,3) = 3;
                    if (sig_new(k,1) < sig_new(k+1,1))&&  (sig_new(k,1) < sig_new(k-1,1))
                        TO_found = 1;
                        TO_idx = k;
                        break
                    end
                end
            end
            
            if TO_found == 1 %search for MSt
                for k = TO_idx-1:-1:2
                    sig_new(k,3) = 2;
                    if (sig_new(k,2) > sig_new(k+1,2)) &&  (sig_new(k,2) > sig_new(k-1,2)) && sig_new(k,2) > -60
                        MSt_found = 1;
                        break
                    end
                end
            end

            sig_new(i,3) = 4;


             if (sig_new(i,1) < sig_new(i-1,1)) && (sig_new(i,1) < sig_new(i+1,1))...
                && sig_new(i,1) < threashold % condizione IC

                IC_found = 1;
                MSw_found = 0;

            end
        end

        if IC_found

            sig_new(i,3) = 1;
%{
            if (sig_new(i,2) > sig_new(i-1,2)) && (sig_new(i,2) > sig_new(i+1,2))

                MSt_found = 1;

                IC_found  = 0;
            end
%}
        end
%{
        if MSt_found == 1

            sig_new(i,3) = 2;

        end
%}
    end




    figure(3)
    hold on
    plot(t,sig_new(:,1),'b');
    plot(t,100*sig_new(:,3), 'r');
    xlim([1900,2500])
    hold off
  
    %% VALIDATION WITH PRESSURE SENSORS

    gyro_z = -dataIMU.GyroZ_deg_s_;

    l = numel(dataIMU(:,1));

    tt = 1:l;

    fco = 0.9766;

    figure(4)
    hold on
    plot(tt, gyro_z)
    plot(tt*fco, 100*dataIMU.P8_L1)
    plot(tt*fco, 100*dataIMU.P5_L2)
    plot(tt*fco, 100*dataIMU.P16_H3)
    xlim([14000,14500])
    hold off

    [b_3,a_3] = butter(2, 0.15);

    pres_filt_8 = filtfilt(b_3,a_3, dataIMU.P8_L1);
    pres_filt_16 = filtfilt(b_3,a_3, dataIMU.P16_H3);
    pres_filt_5 = filtfilt(b_3,a_3, dataIMU.P5_L2);


    figure(5)
    hold on
    plot(tt*fco, dataIMU.P8_L1,':','LineWidth',1)
    plot(tt*fco, dataIMU.P5_L2,':','LineWidth',1)
    plot(tt*fco, dataIMU.P16_H3,':','LineWidth',1)
    plot(tt*fco, pres_filt_8,'LineWidth',1)
    plot(tt*fco, pres_filt_5,'LineWidth',1)
    plot(tt*fco, pres_filt_16,'LineWidth',1)
    xlim([14000,14500])
    xlim([14030,14160])
    hold off


    gait_phase_p_f = ones(l,1);

    pres_dif = ones(l,1);

    for i = 1:l
        
        if pres_filt_16(i) < 2.2 && pres_filt_5(i) < 2.2
            
           pres_dif(i,1) = pres_filt_16(i) - pres_filt_5(i);
           
        end
        
    end

    figure(6)
    hold on
    plot(tt, gyro_z)
    plot(tt*fco, 100*pres_filt_8)
    plot(tt*fco, 100*pres_filt_16)
    plot(tt*fco, 100*pres_filt_5)
    plot(tt*fco,100*pres_dif)
    xlim([14000,14500])
    hold off

    two_found = 0;

    pres_th = 2;

    for i = 2:l-1
        
       if pres_filt_8(i) < pres_th || pres_filt_16(i) < pres_th
           
           gait_phase_p_f(i,1) = 1;
           
       else
           
           gait_phase_p_f(i,1) = 4;
           
       end


       if pres_dif(i) < pres_dif(i-1) && pres_dif(i) < pres_dif(i+1) && pres_dif(i) < -1
           
           % find minimum of difference, the feet equally distribute pressure
           
            two_found = 1;
            
       end

       if two_found == 1 && gait_phase_p_f(i,1) < 4
           
           gait_phase_p_f(i,1) = gait_phase_p_f(i,1) +1;
           
       end

       if pres_filt_16(i) > 2.2
           
           two_found = 0;
           
       end


    %{
       %Old method
       if pres_filt_8(i) < 2.7
           gait_phase_p_f(i,1) = gait_phase_p_f(i,1) - 1;
       end
    %}

    end

    %sig_ay_filt = filtfilt(b_1,a_1,dataIMU.AccY_g_);

    figure(7)
    hold on
    plot(tt, gyro_z)
    plot(tt*fco,(100*gait_phase_p_f)-200,'r--','LineWidth',1)
    plot(tt,100*sig_new(:,3)-200, 'g','LineWidth',1)
    plot(tt,sig_new(:,2), 'b','LineWidth',1)
    %plot(tt, 100*sig_ay_filt,'k')
    plot(tt,tt*0)
    xlim([14000,14500])
    hold off



end
















