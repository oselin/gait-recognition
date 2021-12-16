function [cluster] = clusterData(data)
    
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
    for i=1:n
        buffer = sum(data(:,end) == i);
        cluster{i,1} = zeros(buffer, width(data));
    end
    
    counter = ones(1,n);
    for i = 1:height(data)
        myindex = data(i,end);
        if (myindex) %if row has been classified
            cluster{myindex,1}(counter(myindex),:) = data(i,:);
            counter(myindex) = counter(myindex) + 1;
        end
    end
    
end