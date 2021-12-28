function [X,Y] = dataPreprocessing(dataset)
%% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   DATA PREPROCESSING FUNCTIO
    %   INPUT: cell array of tables
    %   OUTPUT: cell array of formatted data as net input
    %   STEPS:  labeling
    %           delete rows with label = 0
    %           delete useless columns
    %           table reshaping into transposed matrix
    % ---------------------------------------------------------------------
    
    X = cell(numel(dataset),1);
    Y = cell(numel(dataset),1);
    
    useful_data = [ 'AccX (g)', 'AccY (g)', 'AccZ (g)', ...
                    'GyroX (deg/s)', 'GyroY (deg/s)', 'GyroZ (deg/s)', ...
                    'EulerX (deg)', 'EulerY (deg)', 'EulerZ (deg)', ...
                    'LinAccX (g)', 'LinAccY (g)', 'LinAccZ (g)', ...
                    'ID'];

    for i = 1:numel(dataset)
        file = dataset{i};

        %data labeling
        file = detectPhases_3(file);

        %remove rows with label = 0
        file(table2array(file(:,end))==0,:) = [];

        %delete useless columns FOR THIS KIND OF SENSOR
        columns = file.Properties.VariableNames;
        for j = 1:width(columns)
            if(isempty(strfind(useful_data,columns{j})))
                file(:,columns(j)) = [];
            end
        end

        %split label and transpose the table
        X{i} = transpose(table2array(file(:,1:end-1)));
        %transpose label as categorical
        Y{i} = transpose(categorical(table2array(file(:,end))));

    end

end