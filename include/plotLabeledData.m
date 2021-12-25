function [] = plotLabeledData(mydata)
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   PLOT LABELED DATA FUNCTION
    % ---------------------------------------------------------------------
    
    %get total number of classes
    n = length(unique(mydata{:,end}));
    if (n>30)
        %Can't imagine something with more than 30 classes
        disp("Data seems unlabeled. Please check your input.");
        return;
    end

    colors = ['b', 'r', 'm', 'y', 'g'];

    for i = 4:width(mydata)-1 %SENSORID|TIMESTAMP|FRAMENUM|ID are not useful data to display
        figure(i)
        time = mydata{:,2};
        for j = 1:n
            temp = mydata{:,i};
            %set all the rows different from the i-class to null
            %j-1 cause index goes from 1 to n+1, classes from 0 to n [0 is
            %unclassified
            temp(mydata{:,end} ~= j-1) = NaN;
            plot(time, temp, 'Color', colors(j));
            title(mydata.Properties.VariableNames(i));
            hold on
        end
    hold off
    end
end