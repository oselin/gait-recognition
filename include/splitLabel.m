function [unlabeled_data, label] = splitLabel(data)
    
    if (isa(data, 'table'))
        data = data{:,:};
    end
    n = length(unique(data(:,end))) - 1;

    label = cell(n,1);
    unlabeled_data = cell(n,1);

    data = clusterData(data);

    for i = 1:n
        unlabeled_data{i} = data{i}(:,1:end-1);
        label{i} = data{i}(:,end);
    end

end