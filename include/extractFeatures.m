function [table] =  extractFeatures(data)

%%features_aviable = [
%                       "mean",...                  %01
%                       "std",...                   %02
%                       "peak",...                  %03
%                       "peak position",...         %04
%                       "median",...                %05
%                       "variance",...              %06
%                       "covariance",...            %07
%                       "range",...                 %08
%                       "rms",...                   %09
%                       "mode",...                  %10
%                       "mean or median abs dev",   %11
%                       ];

    n_features = 8;

    %Let's calculate the size of the feature matrix and create it
    n_columns = length(data{1}(1,1,:)) * n_features + 1;
    n_rows = 0;

    for i=1:width(data)
        n_rows = n_rows + height(data{i});
    end
    
    table = zeros(n_rows, n_columns);
    
    % Let's fill the matrix
    index = 1;
    row_index = 1;
    for i=1:height(table)
        
        column_index = 1;
        for layer = 1:length(data{index}(1,1,:))

            myrow = data{index}(row_index,:,layer);

            %% 1 - MEAN
            table(i,column_index) = mean(myrow);
            column_index = column_index + 1;

            %% 2 - STD DEV
            table(i,column_index) = std(myrow);
            column_index = column_index + 1;

            %% 3 -MEDIAN
            table(i,column_index) = median(myrow);
            column_index = column_index + 1;

            %% 4 - VAR
            table(i,column_index) = var(myrow);
            column_index = column_index + 1;
            
            %% 5 - RANGE
            table(i,column_index) = max(myrow) - min(myrow);
            column_index = column_index + 1;

            %% 6 - RMS
            table(i,column_index) = rms(myrow);
            column_index = column_index + 1;

            %% 7 - MODE
            table(i,column_index) = mode(myrow);
            column_index = column_index + 1;

            %% 8 - MEAN OR ABSOLUTE DEV
            table(i,column_index) = mad(myrow);
            column_index = column_index + 1;
        end

        table(row_index, end) = index;

         if row_index > height(data{index})
             index = index + 1;
             row_index = 1;
         end
    end
    


    
end