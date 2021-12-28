function [] = plotLabeledData(mydata, time)
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   PLOT LABELED DATA FUNCTION
    % ---------------------------------------------------------------------
    
    %get total number of classes
    if isa(mydata,'table')
        n = length(unique(mydata{:,end}));
        mydata = mydata{:,:};
    else
        n = length(unique(mydata(:,end)));
    end
    if (n>30)
        %Can't imagine something with more than 30 classes
        disp("Data seems unlabeled. Please check your input.");
        return;
    end

    colors = ['k', 'r', 'm', 'b', 'g'];

    for i = 1:width(mydata)-1 %ID is not useful data to display
        figure(i)
        
        for j = 1:n
            temp = mydata(:,i);
            %set all the rows different from the i-class to null
            %j-1 cause index goes from 1 to n+1, classes from 0 to n [0 is
            %unclassified
            temp(mydata(:,end) ~= j) = NaN;

            plot(time, temp, 'Color',  colors(j));
            %title(mydata.Properties.VariableNames(i));
            hold on
            
        end
    hold off
    end
end