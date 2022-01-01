function [acc] = simulateStream(network ,testData, reset_label, graphicsEnabled)
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   SIMULATE STREAM FUNCTION
    % ---------------------------------------------------------------------

    TIMEFRAME  = 350; %[ms]
    DATASTREAM = zeros(12, TIMEFRAME);%specifically for this kind of IMU SENSOR
    results    = zeros(height(testData),1); 
    testData   = detectPhases_3(testData);%labeling

    y_to_plot = zeros(1,TIMEFRAME);
    
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
%             pause(0.01);
        end

    end

    % compute accuracy
    acc = sum(results == testData{:,end})/height(testData);
%     disp("accuracy in stream simulation: " + num2str(acc));
end