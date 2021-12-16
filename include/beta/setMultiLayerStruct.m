function [structure] = setMultiLayerStruct(data, n_columns)
    
    class = length(unique(data(:,end)));
    if (ismember(0,data(:,end)))
         class = class- 1;
    end
    
    %% Creation of the empty structure
    structure = cell(1,class);
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