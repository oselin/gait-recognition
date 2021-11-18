function [timestamp,data] = mergeData(dataset)
    
    rows = 0;
    max_width = 0;

    %get the maximum length
    for i = 1:length(dataset)
        rows = rows + height(dataset{i});
        if (width(dataset{i})>max_width)
            max_width = width(dataset{i});
        end
    end
    
    
    %table's width: width + 1 column for task ID - 3 columns (SENSOR ID,
    %TIMESTAMP, FRAME NUMBER)
    max_width = max_width + 1 - 3;
    data = zeros(rows, max_width);
    
    r = 1;
    for i=1:length(dataset)
        for row = 1:height(dataset{i})
            data(r,1:width(dataset{i}{row,:})-2) = [i,dataset{i}{row,4:end}];
            r = r + 1;
        end
    end

    timestamp = [1:height(data)];
end

