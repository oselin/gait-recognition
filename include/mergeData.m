function [timestamp,mydata] = mergeData(varargin)
    dataset = varargin{1};

    mydata = [];
    for i = 1:length(dataset)
        mydata = [mydata;dataset{i}];
    end
    
    timestamp = mydata{:,2};

    %% If removing empty columns is requested
    if (nargin > 1 && strcmp(varargin{2},'remove'))
        isnotempty = zeros(1,width(mydata));
        for j = 1:width(mydata)
            if (sum(mydata{:,j})) 
                isnotempty(j) = 1;
            end
        end
        
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

