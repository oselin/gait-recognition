function [unlabeled_data, label] = splitLabel(data)
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   SPLIT LABEL FUNCTION
    % ---------------------------------------------------------------------
    
    % I want to work with matrices, so if data is a table, convert it into
    % a matrix
    if (isa(data, 'table'))
        data = data{:,:};
    end
    %get total number of classes
    n = length(unique(data(:,end))) - 1;
    
    %create empty structure to be filled with data
    label = cell(n,1);
    unlabeled_data = cell(n,1);
    
    %clusterize data (i-class data go to i position in the struct)
    data = clusterData(data);

    for i = 1:n
        %split clusterized data into clusterized unlabeled and clusterized
        %label
        unlabeled_data{i} = data{i}(:,1:end-1);
        label{i} = data{i}(:,end);
    end

end