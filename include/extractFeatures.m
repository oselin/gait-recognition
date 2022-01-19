function [table] =  extractFeatures(data,N)

    data = setMultiLayerStruct(data);

    N_FEATURES = 8;
 
    %Let's calculate the size of the feature matrix and create it
    n_columns = (width(data)-1)* N_FEATURES + 1;

    n_rows = 0;
    for i=1:length(unique(data(:,end)))
        n_rows = n_rows + floor(sum(data(:,end)==i)/N);
    end
    
    table = zeros(n_rows, n_columns);
    row_index = 1;
    for i=1:n_rows
        if ((row_index+N) < height(data))
            while (data(row_index,end) ~= data(row_index+N,end))
                row_index = row_index + 1;
            end
        
            column_index = 1;
            for col=1:(width(data)-1)
                batch = data(row_index:row_index+N,col);
                
                %% 1 - MEAN
                table(i,column_index) = mean(batch);
                column_index = column_index + 1;
    
                %% 2 - STD DEV
                table(i,column_index) = std(batch);
                column_index = column_index + 1;
    
                %% 3 -MEDIAN
                table(i,column_index) = median(batch);
                column_index = column_index + 1;
    
                %% 4 - VAR
                table(i,column_index) = var(batch);
                column_index = column_index + 1;
                
                %% 5 - RANGE
                table(i,column_index) = max(batch) - min(batch);
                column_index = column_index + 1;
    
                %% 6 - RMS
                table(i,column_index) = rms(batch);
                column_index = column_index + 1;
    
                %% 7 - MODE
                table(i,column_index) = mode(batch);
                column_index = column_index + 1;
    
                %% 8 - MEAN OR ABSOLUTE DEV
                table(i,column_index) = mad(batch);
                column_index = column_index + 1;
            end
            table(i,end) = data(row_index,end);
            row_index = row_index + N;
        end
    end
end
