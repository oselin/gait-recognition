function [matrixTraining, matrixTesting] = splitData(featuresMatrix, percSplit)
    
    if (percSplit > 1)
        disp("The split index must be within 0 and 1");
        return;
    end

    len_matrix = height(featuresMatrix);
    disp(len_matrix);
    
    lenPercent = ceil(percSplit * len_matrix);
    matrixTraining = zeros(lenPercent,width(featuresMatrix));              
    matrixTesting = zeros((len_matrix-lenPercent),width(featuresMatrix));
    
    
    indexes = randperm(len_matrix,len_matrix-lenPercent); %get random row indexes
    
    counterTraining = 1;
    counterTesting = 1;
    
    for i=1:len_matrix
        
        if (ismember(i,indexes) == 1)
            matrixTesting(counterTesting, :) = featuresMatrix(i,:);
            counterTesting = counterTesting + 1;
        else
            matrixTraining(counterTraining,:) = featuresMatrix(i,:);
            counterTraining = counterTraining + 1;
        end
    end

end



