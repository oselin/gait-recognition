function [cluster] = clusterData(data)
    
    if (isa(data,'table'))
        data = data{:,:};
    end
    n = unique(data(:,end));

    if ismember(0,n)
        n = length(n) - 1;
    else
        n = length(n);
    end

    cluster = cell(n,1);
    len = zeros(1,n);
    for i=1:n
        len(i) = sum(data(:,end) == i);
        cluster{i,1} = zeros(len(i), width(data));
    end
    
    counter = ones(1,n);
    for i = 1:height(data)
        myindex = data(i,end);
        if (myindex)
            cluster{myindex,1}(counter(myindex),:) = data(i,:);
            counter(myindex) = counter(myindex) + 1;
        end
    end
    
end