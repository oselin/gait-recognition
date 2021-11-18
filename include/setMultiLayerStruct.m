function [structure] = setMultiLayerStruct(data, n_columns)
    
    %get the maximum number of activities

    act = 0;
    
    for i=1:height(data)
        if (data(i,1)>act)
            act = data(i,1);
        end
    end
    
    %%Creation of the empty structure
    for j=1:act
        structure{j} = zeros(1, n_columns,width(data)-1);
    end
    
    %Filling the multi-dimensional matrices structure with data
    for activity = 1:width(structure)
        for layer = 2:width(data)
            
            buffer = 1+(sum(data(:,1)<activity));
            for element = 1:fix(sum(data(:,1) == activity)/n_columns)
                
                structure{activity}(element,:, layer-1) = data(buffer:buffer+n_columns-1, layer).';
                buffer = buffer + n_columns;
            end
        end
    end









end