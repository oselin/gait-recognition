function [timestamp,mydata] = mergeData(varargin)
    dataset = varargin{1};

    mydata = [];
    for i = 1:length(dataset)
        mydata = [mydata;dataset{i}];
    end
    
    timestamp = mydata{:,2};

    %% If removing empty columns is requested
    if (nargin == 2 && strcmp(varargin{2},'remove'))
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
    
%     rows = 0;
%     max_width = 0;
% 
%     %get the maximum length
%     for i = 1:length(dataset)
%         rows = rows + height(dataset{i});
%         if (width(dataset{i})>max_width)
%             max_width = width(dataset{i});
%         end
%     end
%     
%     
%     %table's width: width + 1 column for task ID - 3 columns (SENSOR ID,
%     %TIMESTAMP, FRAME NUMBER)
%     max_width = max_width + 1 - 3;
%     data = zeros(rows, max_width);
%     
%     r = 1;
%     for i=1:length(dataset)
%         for row = 1:height(dataset{i})
%             %since the second row is the TIMESTAMP
%             id = getID(dataset{i}{row,2});
% 
%             data(r,1:width(dataset{i}{row,:})-2) = [id,dataset{i}{row,4:end}];
% 
%             if (width(dataset{i}{row,4:end}) < max_width-2)
%                 disp("WARNING LINE " + num2str(r) + ": missing some features"); 
%             end
%             r = r + 1;
%         end
%     end
% 
%     timestamp = [1:height(data)];
end

