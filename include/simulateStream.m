function [] = simulateStream(network ,testData)
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   SIMULATE STREAM FUNCTION
    % ---------------------------------------------------------------------

    TIMEFRAME = 350; %[ms]
    DATASTREAM = zeros(13, TIMEFRAME);%specifically for this kind of IMU SENSOR

    y = zeros(1,TIMEFRAME);
    
    for i = 1:height(testData)

        %New acquisition
        new_stream = testData{i,:};
        %Get rid of useless values
        useless_data = [1:3 10:12 17:19 23:26];
        new_stream(:,useless_data) = [];
        %Update the stream
        DATASTREAM = [DATASTREAM(:,2:end) new_stream'];
        %Predict the label
        new_y = classify(network, DATASTREAM);

        to_plot = DATASTREAM(3,:);
        time = i:i+TIMEFRAME-1;
        y = [y(2:end) grp2idx(new_y(end))];
        plotLabeledData([to_plot' y'], time);
        xlim([i i+TIMEFRAME-1])
        pause(0.01);
    end
end