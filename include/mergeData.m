function [mergedData] = mergeData(data)
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   MERGE DATA FUNCTION
    % ---------------------------------------------------------------------
    %% Convert the data structure in just one big table
    mergedData = data{1};
    for i=2:length(data)
        mergedData = [mergedData; data{i}];
    end
end

