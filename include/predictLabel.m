function [] = predictLabel(network, testData)
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   PREDICT FUNCTION
    % ---------------------------------------------------------------------
    
    
    for i = 1:length(testData)
        YPred = classify(network,testData{i});
        acc = sum(YPred == testY{i})/numel(testY{i});
        disp("Accuracy for phase "+ num2str(i)+": " + num2str(acc));
    end

end