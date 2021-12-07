function [data, label] = splitLabel(data)
    
    label = data(:,end);
    data =  data(:,1:end-1);
end