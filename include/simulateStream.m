function [acc] = simulateStream(network ,testData, reset_label, graphicsEnabled)
    %% --------------------------------------------------------------------
    %%  GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   SIMULATE STREAM FUNCTION
    % ---------------------------------------------------------------------
    
    %%  GOAL OF THE FUNCTION
    %   Goal of this function is simulating incoming data and trying to
    %   label them. Eventually estimate the obtained performances
    %   (=accuracy)
    % ---------------------------------------------------------------------

    % define some parameters
    TIMEFRAME  = 350;                       % the timeframe window we consider in this function
    DATASTREAM = zeros(12, TIMEFRAME);      %specifically for this kind of IMU SENSOR
    results    = zeros(height(testData),1); 
    testData   = detectPhases_3(testData);  %labeling

    y_to_plot = zeros(1,TIMEFRAME); %vector in which storing data
    
    for i = 1:height(testData)

        %New acquisition
        new_stream = testData{i,:};
        %Get rid of useless values, label too
        useless_data = [1:3 10:12 16:19 23:29];
        new_stream(:,useless_data) = [];
        %Update the stream
        DATASTREAM = [DATASTREAM(:,2:end) new_stream'];
        %Predict the label
        y = classify(network, DATASTREAM);

        % beeing the output mode sequence, every time the net classify the
        % data batch, all labels are updated. If reset label is false, we
        % only update the last time instant
                
        if reset_label
            % update all labels
            y_to_plot = grp2idx(y)';
        else
            % add only new last label
            y_to_plot = [y_to_plot(2:end) grp2idx(y(end))];
            
        end
        
        % save stream-like classification result to compute accuracy
        results(i) = grp2idx(y(end)); 

        if graphicsEnabled
            to_plot = DATASTREAM(3,:);
            time = i:i+TIMEFRAME-1;
            plotLabeledData([to_plot' y_to_plot'], time);
            xlim([i i+TIMEFRAME-1]);
            %pause(0.01); %0.01 is the frequency of the incoming data [100Hz]
        end

    end

    % compute accuracy ratio
    acc = sum(results == testData{:,end})/height(testData);
end