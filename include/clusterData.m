function [cluster] = clusterData(data)
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   CLUSTER FUNCTION
    % ---------------------------------------------------------------------
    
    % We work with matrices for better manipulation, so if data is a
    % table, convert it into a matrix
    if (isa(data,'table'))
        data = data{:,:};
    end
    n = length(unique(data(:,end)));
    
    %if unclassified data are present, get the classes size properly
    if ismember(0,n)
        n = length(n) - 1;
    end
    %defining the data structure [cell type]
    cluster = cell(n,1);

    %Setting the full data structure
    %The data structure is a struct with n matrices, where n is the total
    %number of classes (except for class 0)
    for i=1:n
        buffer = sum(data(:,end) == i);
        cluster{i} = zeros(buffer, width(data));
    end
    %let's define a row counter for each matrix
    counter = ones(1,n);
    for i = 1:height(data)
        myindex = data(i,end); %get the class label
        if (myindex) %if row has been classified [I'm dropping unclassified data]
            cluster{myindex}(counter(myindex),:) = data(i,:);
            counter(myindex) = counter(myindex) + 1;
        end
    end
    
end