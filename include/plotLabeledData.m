function [] = plotLabeledData(mydata)
    n = length(unique(mydata{:,end}));
    colors = ['b', 'r', 'm', 'y', 'g'];
    for i = 4:width(mydata)-1 %SENSORID|TIMESTAMP|FRAMENUM|ID are not useful data
        figure(i)
        time = mydata{:,2};
        for j = 1:n
            temp = mydata{:,i};
            temp(mydata{:,end} ~= j-1) = NaN;
            plot(time, temp, 'Color', colors(j));
            title(mydata.Properties.VariableNames(i));
            hold on
        end
    hold off
    end
end