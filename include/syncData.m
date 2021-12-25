function [syncr] = syncData(data)
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   SYNCHRONIZE DATA FUNCTION
    % ---------------------------------------------------------------------
    
    %%---------------------------------------------------------------------
    %   NOTE
    %   data must be in the following form:
    %   data = {IMU_file, MITCH_FILE, MITCH_FILE};
    %----------------------------------------------------------------------

    %% TO TEST THE FUNCTION ALONE, UNCOMMENT THIS
    %   and comment function and the last end
    %clc;
    %clear all;
    %close all;
    %file01 = readtable('data/record_lab_15-12-21_working/IMU.csv', "VariableNamingRule","preserve");
    %file02 = readtable('data/record_lab_15-12-21_working/mitch.txt', "VariableNamingRule","preserve");
    %data = {file01, file02};

    LEN = length(data);
    DATA = cell(1,LEN);
    for i=1:LEN
        if i==1
            DATA{i} = data{i}{:,6};
        else
            DATA{i} = data{i}{:,4};
        end
    end

    fs = 100; %sample frequency

    %% AUTOMATICALLY GET THE BAND-PASS FILTERING FREQUENCY LOOKING AT FFTs
    peak_freq = zeros(1,LEN);
    for i = 1:LEN
        fN = fs/2;
        n = length(DATA{i});
        ff = (0:fs/n:fN);
        y = fft(DATA{i});
        myfft = abs(y(1:(ceil((n+1)/2))));

        %Assuming data shaken at a freq > 1Hz
        myfft = myfft(ceil(n/fs):end);
        ff    = ff(ceil(n/fs):end);
        [~,peak_freq(i)] = max(myfft);
        peak_freq(i) = ff(peak_freq(i));
    end
   
    %% NORMALIZATION
    for i = 1:LEN
        DATA{i} = DATA{i}/range(DATA{i});
    end
        
    %% FILTERING
    FILTERED_DATA = cell(1, LEN);
    for i= 1:LEN
        [B,A] = butter(4,[0.02 (peak_freq(i)+6)/fs]);
        FILTERED_DATA{i} = filter(B,A,DATA{i});
    end
    
    %% FIND THE VERY MAX
    maxes = zeros(1,LEN);
    for i = 1:LEN
        [~, maxes(i)] = max(FILTERED_DATA{i}(1:ceil(end/3)));
    end
    
    %% FIND THE MIN TIMEFRAME
    timeframe = height(DATA{1}) - maxes(1);
    for i = 1:LEN
        if (height(DATA{i}) - maxes(i)) < timeframe
            timeframe = height(DATA{i}) - maxes(i);
        end
    end
    
    %% PLOT
    figure(1)
    for i = 1:LEN
        plot(1:timeframe,DATA{i}(1:timeframe));
        hold on;
        xline(maxes(i));
    end
    hold off; legend(strcat('Data ',num2str((1:i)')))
    title("Unfiltered signals [Time domain]");
    
    figure(2)
    for i = 1:LEN
        plot(1:timeframe,FILTERED_DATA{i}(1:timeframe));
        hold on;
        xline(maxes(i));
    end
    hold off; legend(strcat('Data ',num2str((1:i)')))
    title("Filtered signals [Time domain]");
    
    %% SYNCHRONIZE DATA
    syncr = cell(1,LEN);
    SYNCR = cell(1,LEN); %just to easily plot accZ sync signals
    for i = 1:LEN
        syncr{i} = data{i}(maxes(i):timeframe+(maxes(i)-1),:);
        SYNCR{i} = DATA{i}(maxes(i):timeframe+(maxes(i)-1));
    end
    
     %% PLOT SYNCHRONIZED DATA
    figure(3);
    for i = 1:LEN
        plot(1:timeframe,SYNCR{i});
        hold on;
    end
    hold off; legend(strcat('Data ',num2str((1:i)')))
    title("Syncronized signals [Time domain]")
    
     %% ESTIMATE THE TOTAL DELAY
%     [~, latest] =min(maxes);
%     
%     disp("The latest sensor to be turned on is " + num2str(latest));
%     file01{maxes(1),2};
%     (file03{maxes(3),1} - file02{maxes(2),1})/1000
%     % fprintf(num2str(file02{maxes(2),1}), '.16g');
%     % fprintf('\n');
%     % fprintf(num2str(file02{maxes(2)+1,1}), '.16g');
    
end