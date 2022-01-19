function [structure] = setMultiLayerStruct(data)

    if (isa(data,"table"))
         data = data{:,:};
    end
    class = length(unique(data(:,end)));

    %% Creation of the empty structure
    indexes= ones(1,class);
    for i=2:class
        indexes(i) = sum(data(:,end)==i-1) + indexes(i-1);
    end
    structure = zeros(height(data),width(data));
    
    for i = 1:height(data)
        row = data(i,:);
        id = row(end);
        structure(indexes(id),:) = row;
        indexes(id) = indexes(id) + 1;
        
    end
    

end