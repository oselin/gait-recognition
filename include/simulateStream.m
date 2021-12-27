function [] = simulateStream(network ,testData, reset_label)
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   SIMULATE STREAM FUNCTION
    % ---------------------------------------------------------------------

    TIMEFRAME = 350; %[ms]
    DATASTREAM = zeros(12, TIMEFRAME);%specifically for this kind of IMU SENSOR
    results = zeros(height(testData),1); 
    testData = detectPhases_new_2(testData);%labeling

    y_to_plot = zeros(1,TIMEFRAME);
    
    for i = 1:height(testData)

        %New acquisition
        new_stream = testData{i,:};
        %Get rid of useless values, label too
        useless_data = [1:3 10:12 16:19 23:27];
        new_stream(:,useless_data) = [];
        %Update the stream
        DATASTREAM = [DATASTREAM(:,2:end) new_stream'];
        %Predict the label
        y = classify(network, DATASTREAM);
          

        to_plot = DATASTREAM(3,:);
        time = i:i+TIMEFRAME-1;
        if reset_label
            y_to_plot = grp2idx(y)';
        else
            y_to_plot = [y_to_plot(2:end) grp2idx(y(end))];
            results(i) = grp2idx(y(end));
        end

%         plotLabeledData([to_plot' y_to_plot'], time);
%         xlim([i i+TIMEFRAME-1]);
%         pause(0.01);
    end
    acc = sum(results == table2array(testData(:,end)))/height(testData);
    disp(size(table2array(testData(:,end))));
    disp(size(results));
    disp(acc);
end