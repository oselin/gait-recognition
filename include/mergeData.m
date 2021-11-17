function [s] = mergeData(dataset)
    
    rows = 0;
    max_length = 0;

    %get the maximum length
    for i = 1:length(dataset)
        rows = rows + height(dataset{i});
        if (width(dataset{i})>max_length)
            max_length = width(dataset{i});
        end
    end
    
    max_length = max_length + 1;
    s = zeros(rows, max_length);
    
    r = 1;
    for i=1:length(dataset)
        for row = 1:height(dataset{i})
            s(r,1:width(dataset{i}{row,:})-2) = [i,dataset{i}{row,4:end}];
            r = r + 1;
        end
    end

    
    
