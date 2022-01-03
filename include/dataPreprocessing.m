function [X,Y] = dataPreprocessing(dataset)
    %% --------------------------------------------------------------------
    %%  GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   DATA PREPROCESSING FUNCTION
    % ---------------------------------------------------------------------

    %%  GOAL OF THE FUNCTION
    %   Goal of this function is performing pre-processing operations in
    %   order to accomodate data structures for the training network
    % ---------------------------------------------------------------------

    %% --------------------------------------------------------------------
    %   INPUT: cell array of tables
    %   OUTPUT: cell array of formatted data as net input
    %   STEPS:  labeling
    %           delete rows with label = 0
    %           delete useless columns
    %           table reshaping into transposed matrix
    % ---------------------------------------------------------------------
    
    X = cell(numel(dataset),1); %to store data stream
    Y = cell(numel(dataset),1); %to store labels
    
    %data to keep from input dataset
    useful_data = [ 'AccX (g)', 'AccY (g)', 'AccZ (g)', ...
                    'GyroX (deg/s)', 'GyroY (deg/s)', 'GyroZ (deg/s)', ...
                    'EulerX (deg)', 'EulerY (deg)', 'EulerZ (deg)', ...
                    'LinAccX (g)', 'LinAccY (g)', 'LinAccZ (g)', ...
                    'ID'];

    for i = 1:numel(dataset)
        %% Extract i-th file
        file = dataset{i}; 

        %% Data labeling
        file = detectPhases_3(file);

        %% Remove rows with label = 0
        file(file{:,end}==0,:) = [];

        %% Delete useless columns FOR THIS KIND OF SENSOR
        % extract file columns name
        columns = file.Properties.VariableNames;
        for j = 1:width(columns)
            % if column's name isn0t in useful_data
            if(isempty(strfind(useful_data,columns{j})))
                % delete column
                file(:,columns(j)) = [];
            end
        end

        % split data and label and transpose the table
        X{i} = transpose(file{:,1:end-1});
        Y{i} = transpose(categorical(file{:,end}));

    end

end