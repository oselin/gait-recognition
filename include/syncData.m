function [syncr] = syncData(data, fs, graphicsEnabled)
   
    %% --------------------------------------------------------------------
    %%  GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   SYNCHRONIZE DATA FUNCTION
    % ---------------------------------------------------------------------
    
    %% --------------------------------------------------------------------
    %%  GOAL OF THE FUNCTION
    %   Goal of this function is synchronizing signals from IMU sensor and
    %   MITCH sensors, in order to allow the validation of the
    %   detectPhases_3 algorithm
    % ---------------------------------------------------------------------

    %% --------------------------------------------------------------------
    %%  PARAMETERS
    %   data must be in the following form:
    %   data = {IMU_file, MITCH_FILE, MITCH_FILE};
    %   
    %   fs is the sample frequency of the sensors
    %   graphicsEnabled allows to easily display the plots
    % ---------------------------------------------------------------------
    
    %% TO TEST THE FUNCTION ALONE, UNCOMMENT THIS
    %   and comment function and the last end
    %clc;
    %clear all;
    %close all;
    %file01 = readtable('../data/record_lab_15-12-21_afternoon/IMU.csv', "VariableNamingRule","preserve");
    %file02 = readtable('../data/record_lab_15-12-21_afternoon/mitch.txt', "VariableNamingRule","preserve");
    %data = {file01, file02};
    %fs = 100; %sample frequency
    
    LEN = length(data);
    % Create a cell in which put the imported data
    DATA = cell(1,LEN);

    % For each different data log, extract only the important column
    for i=1:LEN
        if i==1
            DATA{i} = data{i}{:,6}; %column 6 for the IMU   log file
        else
            DATA{i} = data{i}{:,4}; %column 4 for the mitch log file
        end
    end

    

    %% AUTOMATICALLY GET THE BAND-PASS FILTERING FREQUENCY LOOKING AT FFTs
    peak_freq = zeros(1,LEN);
    for i = 1:LEN
        % Transformation in the frequency domain
        fN = fs/2; %Nyquist frequency
        n = length(DATA{i});
        ff = (0:fs/n:fN); %frequency interval up to the Nyquist frequency
        y = fft(DATA{i});
        myfft = abs(y(1:(ceil((n+1)/2)))); %get only the positive frequencies

        %Assuming data shaken at a freq > 1Hz
        myfft = myfft(ceil(n/fs):end);
        ff    = ff(ceil(n/fs):end);
        if graphicsEnabled
            figure(1)
            plot(ff,myfft);
        end
        [~,peak_freq(i)] = max(myfft);   %get the max module in the frequency domain, in order to define the frequency interval for the band bass filtering
        peak_freq(i) = ff(peak_freq(i)); %store the value
    end
   
    %% NORMALIZATION
    for i = 1:LEN
        DATA{i} = DATA{i}/range(DATA{i});
    end
        
    %% FILTERING
    FILTERED_DATA = cell(1, LEN);   %create a cell for storing filtere data
    for i= 1:LEN
        [B,A] = butter(4,[0.02 (peak_freq(i)+6)/fs]); %design a butterworth bandpass filter
        FILTERED_DATA{i} = filter(B,A,DATA{i}); %filter
    end
    
    %% FIND THE VERY MAX
    maxes = zeros(1,LEN); %vector in which storing the max values
    for i = 1:LEN
        [~, maxes(i)] = max(FILTERED_DATA{i}(1:ceil(end/3)));
    end
    
    %% FIND THE MIN TIMEFRAME
    %data can have different durations, so I need to set them equal
    timeframe = height(DATA{1}) - maxes(1);
    for i = 1:LEN
        if (height(DATA{i}) - maxes(i)) < timeframe
            timeframe = height(DATA{i}) - maxes(i);
        end
    end
    
    %% PLOT
    if graphicsEnabled
        figure(2)
        for i = 1:LEN
            plot(1:timeframe,DATA{i}(1:timeframe));
            hold on;
            xline(maxes(i));
        end
        hold off; legend(strcat('Data ',num2str((1:i)')))
        title("Unfiltered signals [Time domain]");
        
        figure(3)
        for i = 1:LEN
            plot(1:timeframe,FILTERED_DATA{i}(1:timeframe));
            hold on;
            xline(maxes(i));
        end
        hold off; legend(strcat('Data ',num2str((1:i)')))
        title("Filtered signals [Time domain]");
    end
    
    %% SYNCHRONIZE DATA
    syncr = cell(1,LEN); %cell in which storing synchronized data
    SYNCR = cell(1,LEN); %just to easily plot accZ sync signals
    for i = 1:LEN
        %the maxes correspond to the same time event, so from that to
        %shared timeframe, store them
        syncr{i} = data{i}(maxes(i):timeframe+(maxes(i)-1),:);
        SYNCR{i} = DATA{i}(maxes(i):timeframe+(maxes(i)-1));
    end
 
    %% PLOT SYNCHRONIZED DATA
    if graphicsEnabled
        figure(4);
        for i = 1:LEN
            plot(1:timeframe,SYNCR{i});
            hold on;
        end
        hold off; legend(strcat('Data ',num2str((1:i)')))
        title("Syncronized signals [Time domain]")
    end
        
end