function [data] = dataPreprocessingUnsupervised(varargin)
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
    
    % The first input element is for sure the dataset structure
    dataset = varargin{1};

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

        dataset{i} = file;


    end
    
    % Let's concatenate the different structures inside the dataset, in
    % order to have a single table
    data = dataset{1};
    for i=2:length(dataset)
        data = [data; dataset{i}];
    end

    % If the data structure is still a table type, convert it as matrix
    if (isa(data,"table"))
         data = data{:,:};
    end
    
    % If more inputs are provided, let's check they are correct
    if (nargin == 3 && varargin{2}=="features")
        % The third element is the size of the batch we're gonna use
        N = varargin{3};

        % Run the features extraction
        data = extractFeatures(data,N);
    end
end

%% FEATURE EXTRACTION FUNCTION
function [table] =  extractFeatures(data,N)
  
    %% SORT DATA ACCORDING TO THEIR CLASS
    % Get the total number of different classes
    class = length(unique(data(:,end)));

    % Define an array in which put the estimated indexes of the beginning of
    % different classes
    indexes= ones(1,class);
    for i=2:class
        indexes(i) = sum(data(:,end)==i-1) + indexes(i-1);
    end

    % Creation of the empty structure
    structure = zeros(height(data),width(data));
    
    % For each data in the data structure, move it in the right row, so
    % just below the last data row of the same class
    for i = 1:height(data)
        row = data(i,:);
        id = row(end);
        structure(indexes(id),:) = row;
        indexes(id) = indexes(id) + 1;
    end

    data = structure;

    N_FEATURES = 7;
 
    % Let's calculate the size of the feature matrix and create it
    n_columns = (width(data)-1)* N_FEATURES + 1;

    % Let's define n_rows as the counter of the estimated rows we will need
    n_rows = 0;
    for i=1:length(unique(data(:,end)))
        n_rows = n_rows + floor(sum(data(:,end)==i)/N);
    end
    % Creation of the feature-empty structure
    table = zeros(n_rows, n_columns);

    % Let's define a temp index in order to properly dive into the data
    % structure (aka row indicator)
    row_index = 1;
    for i=1:n_rows

        if ((row_index+N) < height(data)) %To be sure I do not exeed the data structure

            % If I have less than N element of the same class, discard it
            % and go to the next N right elements
            while (data(row_index,end) ~= data(row_index+N,end))
                row_index = row_index + 1;
            end
            
            % Let's define a column index, because we want to compute all
            % the feature estimations for each column
            column_index = 1;
            for col=1:(width(data)-1) % -1 because the last element is ID
                % Collect a batch of N data on which estimating features
                batch = data(row_index:row_index+N,col);
                
                %% 1 - MEAN
                table(i,column_index) = mean(batch);
                column_index = column_index + 1;
       
                %% 2 -MEDIAN
                table(i,column_index) = median(batch);
                column_index = column_index + 1;
    
                %% 3 - VAR
                table(i,column_index) = var(batch);
                column_index = column_index + 1;
                
                %% 4 - RANGE
                table(i,column_index) = max(batch) - min(batch);
                column_index = column_index + 1;
    
                %% 5 - RMS
                table(i,column_index) = rms(batch);
                column_index = column_index + 1;
    
                %% 6 - MODE
                table(i,column_index) = mode(batch);
                column_index = column_index + 1;
    
                %% 7 - MEAN OR ABSOLUTE DEV
                table(i,column_index) = mad(batch);
                column_index = column_index + 1;
            end
            % Append the right ID
            table(i,end) = data(row_index,end);
            row_index = row_index + N;
        end
    end
end
