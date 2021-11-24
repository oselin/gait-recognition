function [structure] = setMultiLayerStruct(data, n_columns)
    
    %get the total number fo classes

    class = length(unique(data(:,1))) - 1; %minus 1 bc with class 0 I labeled non-classified data
    
    %%Creation of the empty structure
    for j=1:class
        structure{j} = zeros(1, n_columns, width(data)-1); % minus 1 bc I'm dropping the ID
    end
    
    sortedClassMatrix = sortDataByClass(data);

    %Filling the multi-dimensional matrices structure with data
    for n_class = 1:class
        for layer = 2:width(sortedClassMatrix) %from 2 bc the first column is for the ID
            
            buffer = 1+(sum(sortedClassMatrix(:,1)<n_class));
            for element = 1:fix(sum(sortedClassMatrix(:,1) == n_class)/n_columns)
                
                structure{n_class}(element,:, layer-1) = sortedClassMatrix(buffer:buffer+n_columns-1, layer).';
                buffer = buffer + n_columns;
            end
        end
    end









end