function [output] = freqTransform(timeData)

    %For a better frequency-domain transformation, it's better to subtract
    %the mean value of the series

    intervals = unique(timeData(:,1));
    range = 0;

    for i = 1:intervals
        %get the correct range
        submatrix = timeData(timeData(:,1)==1,:);
        mymean = mean(submatrix);
        for j = 1:hegith(submatrix)
            submatrix(j,:) = submatrix(j,:) - mymean;
        end

    end
end
