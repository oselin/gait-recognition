%% ------------------------------------------------------------------------
%
%%   GAIT PHASE RECOGNITION: OFFLINE AUTOMATIC THREASHOLD METHOD
%
%% ------------------------------------------------------------------------


function [data] = detectPhases_new_2(data_in)
    %% LOAD 

    %data = readtable(data_in);

    sig = data_in.("GyroZ (deg/s)");

    t = 1:numel(sig);

    L = length(t);
    
    %% PLOT and FFT
    
    %{
        
        figure
        hold on
        plot(t,sig, 'b');
        hold off
    
    %% FFT

        T_sample = t(2) -  t(1);

        Fs = 1/T_sample;

        f = Fs*(0:(L/2))/L;

        f_max = f(end);

        Y = fft(sig);
        P2 = abs(Y/L);
        P1 = P2(1:L/2+1);
        P1(2:end-1) = 2*P1(2:end-1);

        figure 
        plot(f,P1)
        
    %}

    %% THRESHOLD METHOD
    
    
    % Implementare ulteriori condizioni di sequenzialità per eliminare
    % disturbi dovuti a grosse oscillazioni, soprattutto per la ricerca di
    % un unico MSw

    %{
            Step  
    
              1.     filtrare con un filtro passa basso frequenza in modo 
                    da trovare un solo picco, ma allo stesso tempo tenere il
                    più possibie fedele il segnale. Il segnale risulta essere 
                    ben definito sotto una frequenza relativa di 0.3.
    
              2.     filtrare con un filtro passa basso con una frequenza più
                    bassa in modo da identificare la fase 2 

              3.     impostare una soglia in modo automatico
                    metodo pensato: media tra il massimo valore e la media del
                    segnale, tenendo in considerazione solamente la parte
                    positiva, volendo si può introdurre un peso.
                    Creare matrice dove salvare segnali filtrati e numero
                    della fase.

              4.     grande ciclo for su tutto il segnale
         
              5.     trovare i picchi massimi corrispondenti alle fasi 
                    MSw, massima velocità di rotazione della gamba durante
                    la fase di swing. 
                    Massimo locale e maggiore della soglia.
                    Mettere flag MSw, togliere altri flag.
              
              6.     trovato MSw 
                    piccolo ciclo for all'indietro per trovare il minimo 
                    corrispondente alla fase TO. 
                    Mettere flag TO, break
              
              7.     trovato MSw
                    procedere con la ricerca della fase successiva:
                    trovare il minimo successivo a MSw corrispondente a IC.
                    Mettere flag IC, togliere flag MSw.
              
              8.     trovato IC
                    procedere alla ricerca del massimo successivo
                    corrispondente alla fase MSt, il massimo va ricercato
                    nel segnale filtrato a più bassa frequenza.
                    Mettere flag MSt, togliere flag IC.
            
              9.     trovato MSt
                    procedere fino a trovare il massimo successivo, cioè
                    MSw, ripetere dal punto 5.

    %}

    %% 1. filtering

    [b_1,a_1] = butter(2, 0.4);

    sig_filt = filtfilt(b_1,a_1, sig);
    
    %% 2. filtering
    
    [b_2,a_2] = butter(2, 0.1);

    sig_filt_2 = filtfilt(b_2,a_2, sig);

    %{
        figure
        hold on
        plot(t,sig,'b','LineWidth',1);
        plot(t,sig_filt, 'r','LineWidth',1);
        plot(t,sig_filt_2, 'k','LineWidth',1);
        xlim([1900,2100])
        hold off
    %}

    %% 3. threashold and sig new

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

    k = 0.3;

    threashold = (max_sig*k + mean_a(1))/2;  % cambiato valore di soglia

    %% 5. to 9. find phases

    MSw_found = 0; %flags
    IC_found  = 0;
    TO_found  = 0;
    MSt_found = 0;

    for i=2:L-1

        if (sig_new(i,1) > sig_new(i-1,1)) && (sig_new(i,1) > sig_new(i+1,1))...  %MSw condition
           && sig_new(i,1) > threashold

            MSw_found = 1; % flag MSw
            
            TO_found  = 0;
            
            IC_found  = 0;
            
            MSt_found = 0;
        
        end

        if MSw_found

            if TO_found == 0
                
                for k = i-1:-1:2 % backward for cycle to find TO
                    
                    sig_new(k,3) = 3;
                    
                    if (sig_new(k,1) < sig_new(k+1,1))&&  (sig_new(k,1) < sig_new(k-1,1)) %TO condition
                       
                        TO_found = 1; % flag TO
                        
                        break
                    
                    end
                    
                end
                
            end

            sig_new(i,3) = 4;


            if (sig_new(i,1) < sig_new(i-1,1)) && (sig_new(i,1) < sig_new(i+1,1))...
                && sig_new(i,1) < threashold % IC condition

                IC_found = 1; % flag IC
                
                MSw_found = 0;

            end
        end

        if IC_found

            sig_new(i,3) = 1;

            if (sig_new(i,2) > sig_new(i-1,2)) && (sig_new(i,2) > sig_new(i+1,2)) % MSt condition

                MSt_found = 1; % flag MSt
                
                IC_found  = 0;
                
            end

        end

        if MSt_found == 1

            sig_new(i,3) = 2;

        end
    end



    %{
        figure
        hold on
        plot(t,sig_new(:,1),'b');
        plot(t,100*sig_new(:,3), 'r');
        xlim([1900,2500])
        hold off
    
    %}
    
    
    data = [data_in, array2table(sig_new, 'VariableNames',{'gyro_z_filt','gyro_z_heavy_filt', 'ID'})];
    
end





















