function [matrixTraining, matrixTesting] = splitData(matrix, percSplit)
    
    if (percSplit > 1)
        disp("The split index must be within 0 and 1");
        return;
    end

    if (isa(matrix, 'table'))
        matrix = matrix{:,4:end};
    end

    len_matrix = height(matrix);
        
    lenPercent = ceil(percSplit * len_matrix);
    matrixTraining = zeros(lenPercent,width(matrix));              
    matrixTesting = zeros((len_matrix-lenPercent),width(matrix));
    
    
    indexes = randperm(len_matrix,len_matrix-lenPercent); %get random row indexes
    
    counterTraining = 1;
    counterTesting = 1;
    
    for i=1:len_matrix
        if (ismember(i,indexes))
            matrixTesting(counterTesting, :) = matrix(i,:);
            counterTesting = counterTesting + 1;
        else
            matrixTraining(counterTraining,:) = matrix(i,:);
            counterTraining = counterTraining + 1;
        end
    end

end



