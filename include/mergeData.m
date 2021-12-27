function [timestamp,mydata] = mergeData(varargin)
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   MERGE DATA FUNCTION
    % ---------------------------------------------------------------------
    dataset = varargin{1};
    
    %% Convert the data structure in just one big table
    mydata = [];
    for i = 1:length(dataset)
        mydata = [mydata;dataset{i}];
    end
    
    timestamp = mydata{:,2};

    %FOR THIS KIND OF SENSORS, COLUMN 1,2,3 ARE USELESS
    mydata = mydata(:,4:end);
    %% If removing empty columns is requested
    if (nargin > 1 && strcmp(varargin{2},'remove'))
        % find the non zero columns in the matrix
        isnotempty = zeros(1,width(mydata));
        for j = 1:width(mydata)
            if (sum(mydata{:,j})) 
                isnotempty(j) = 1;
            end
        end
        
        %get column names
        labels = mydata.Properties.VariableNames;
        for k = 1:width(mydata)
            if (~isnotempty(k))
                mydata = removevars(mydata, labels(k));
            end
        end
    end
    
    if (nargin > 2)
        disp("Something went wrong. Please check your function inputs");
        return
    end
end

