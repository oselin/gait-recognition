function [data] = detectPhases(data)

%     %% GAIT RECOGNITION ALG
%     clear ;
%     close all;
%     clc
%     
%     %% LOAD 
%     data = readtable('record_walk_21-11-21_2nd_caviglia.csv');
    
    gyro_x = data.("GyroX (deg/s)");
    gyro_y = data.("GyroY (deg/s)");
    gyro_z = data.("GyroZ (deg/s)");
    
    acc_x = data.("AccX (g)");
    acc_y = data.("AccY (g)");
    acc_z = data.("AccZ (g)");
    
    l_acc_x = data.("LinAccX (g)");
    l_acc_y = data.("LinAccY (g)");
    l_acc_z = data.("LinAccZ (g)");
    
    euler_x = data.("EulerX (deg)");
    euler_y = data.("EulerY (deg)");
    euler_z = data.("EulerZ (deg)");
    
    time = data.("TimeStamp (s)");
    
    t = 1:numel(gyro_x);
    
    %% PLOT
    
    %{
    figure
    hold on
    plot(t,gyro_x,'r');
    plot(t,gyro_y, 'g'); 
    plot(t,gyro_z, 'b');
    hold off
    
    figure
    hold on
    plot(t,euler_x,'r');
    plot(t,euler_y, 'g'); 
    plot(t,euler_z, 'b');
    hold off
    
    figure
    hold on
    plot(t,acc_x,'r');
    plot(t,acc_y, 'g'); 
    plot(t,acc_z, 'b');
    hold off
    
    figure
    hold on
    plot(t,l_acc_x,'r');
    plot(t,l_acc_y, 'g'); 
    plot(t,l_acc_z, 'b');
    hold off
    
    %}
    
%     figure
%     hold on
%     %plot(t,100*l_acc_x,'r');
%     %plot(t,100*l_acc_y,'g');
%     plot(t,euler_y, 'g');
%     plot(t,euler_x, 'r')
%     plot(t,-gyro_z, 'b');
%     xlim([1900,2500])
%     hold off
    
    
    %% FFT
    
    L = length(t);
    T_sample = t(2) -  t(1);
    Fs = 1/T_sample;
    f = Fs*(0:(L/2))/L;
    f_max = f(end);
    
    Y = fft(gyro_z);
    P2 = abs(Y/L);
    P1 = P2(1:L/2+1);
    P1(2:end-1) = 2*P1(2:end-1);
    
%     figure 
%     plot(f,P1) 
    
    %% FILTER
    
    [b,a] = butter(2, 0.3);
    freqz(b,a);
    Z_filt = filtfilt(b,a, gyro_z);
    [b_2, a_2] = butter(2, 0.05);
    sig_2_filt = -filtfilt(b_2,a_2, gyro_z);
    
%     figure
%     hold on
%     plot(t,-Z_filt,'b');
%     plot(t,-gyro_z, 'r');
%     plot(t,sig_2_filt, 'g');
%     xlim([1900,2500])
%     hold off
    
    %% THRESHOLD METHOD MSw
    
    sig = -Z_filt;
    
    MSw = [];
    TO = [];
    IC = [];
    MSt = [];
    
    s_1 = 170;
    s_2 = -150;
    
    sig_filt = sig;
    
    [pk_val_1, pk_loc_1] = findpeaks(sig_filt);
    [pk_val_2, pk_loc_2] = findpeaks(-sig_filt);
    
    [pk_val_3, pk_loc_3] = findpeaks(sig_2_filt);
    
    
    n1 = numel(pk_loc_1);
    n2 = numel(pk_loc_2);
    
    
    pk_tot = sortrows([[pk_loc_1,pk_val_1,zeros(n1,1),zeros(n1,1)];...
                       [pk_loc_2,-pk_val_2,ones(n2,1),zeros(n2,1)]],1);
                   
    
    IC_detected = 0;
    MSt_detected = 0;
    
    w_length = 9;
    
    m_avg_t = 0;
    
    for i=2:n1+n2-w_length
        
        if MSt_detected == 1
           IC_detected = 0; 
        end
        
        if pk_tot(i,2) > s_1
            MSw = [MSw, pk_tot(i,1)];
            pk_tot(i,4) = 1;
        
        elseif pk_tot(i,2) < s_2
            TO = [TO, pk_tot(i,1)];
            pk_tot(i,4) = 1;
        end
                 
        if pk_tot(i,3) == 0 && pk_tot(i,4) == 1
            if pk_tot(i+1,3) == 1
                IC = [IC, pk_tot(i+1,1)];
                pk_tot(i + 1,4) = 1; 
                IC_detected = 1;
            end
        end
    %{
        if IC_detected == 1
            
            m_avg = 0;
    
            for k = i:i+w_length
                
                m_avg = m_avg + pk_tot(k,2);
                
            end
            
            m_avg = m_avg/w_length;
            
            if m_avg < m_avg_t
                
                MSt_detected = 1;
                
                pk_tot(i+1,3) = 3;
                
                MSt = [MSt, pk_tot(i+w_length-1,1)];
                
            else
                
                m_avg_t = m_avg;
                
            end
            
        end
    %}
    end
    %%
        
%     figure
%     hold on
%     plot(t,-gyro_z,'b');
%     plot(t(MSw), -gyro_z(MSw), 'o')
%     plot(t(TO), -gyro_z(TO), '*')
%     plot(t(IC), -gyro_z(IC), 'x')
%     plot(t(MSt), -gyro_z(MSt), 'o')
%     xlim([1900,2500])
%     hold off

    %%  Adding the peaks in the vector
    labels = zeros(height(data),1);
    temp = {MSw, TO, IC, MSt};
    
    for i = 1:length(temp)
        for j = 1:length(temp{i})
            labels(temp{i}(1,j),1) = i;
        end
    end
    
    %% get the first classified point and the class label
    pos = find(labels(:)~=0,1);
    val = labels(pos);
    for i = pos:height(labels)-1
        labels(i) = val;
        if (labels(i+1) ~= val && labels(i+1) ~= 0)
            val = labels(i+1);
        end
    end
    labels(end) = val;

    
    data = [data,array2table(labels,'VariableNames',{'ID'})];




end