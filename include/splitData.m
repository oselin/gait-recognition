function [matrixTraining, matrixTesting] = splitData(varargin)
    matrix      = varargin{1};
    percSplit   = varargin{2};
    
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
    
    if (nargin == 3 && strcmp(varargin{3},"rand"))
        disp("Splitting data randomly");
        indexes = randperm(len_matrix,len_matrix-lenPercent); %get random row indexes
    else
        disp("Splitting data sequentially");
        indexes = (1:len_matrix-lenPercent);
    end

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



