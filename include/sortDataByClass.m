function [matrix] = sortDataByClass(data)

    %get unique class number
    n = length(unique(data(:,1))) - 1; %i want to drop 0-class

    pos = zeros(height(data),n);
    validRows = 0;
    for i = 1:n
        pos(:,i) = data(:,1) == i;
        validRows = validRows + sum(pos(i)==1);
    end


    matrix = zeros(validRows, width(data));
    buffer = 1;
    for class = 1:n        
        for i = 1:height(data)
            if pos(i,class)
                matrix(buffer, :) = data(i, :);
                buffer = buffer + 1;
            end
        end
    end

    
end