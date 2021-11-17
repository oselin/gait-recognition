function [structure] = setMultiLayerStruct(data, n_columns)
    
    %get the maximum number of activities

    act = 0;
    
    %markers = {};
    for i=1:height(data)
%         if (i>1)
%             if (data(i,1) ~= data(i-1,1))
%                 markers{width(markers) + 1} = i;
%             end
%         end
        if (data(i,1)>act)
            act = data(i,1);
        end
    end
    
    
    %creation of the structure
    for j=1:act
        structure{j} = zeros(1, n_columns,width(data));
    end








end