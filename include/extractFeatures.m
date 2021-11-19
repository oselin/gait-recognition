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

    n_features = 11;
    %Let's calculate the size of the feature matrix
    n_columns = size(data{1});
    n_columns = n_columns(end)*n_features + 1;
    n_rows = 0;

    for i=1:width(data)
        n_rows = n_rows + height(data{i});
    end
    
    table = zeros(n_rows, n_columns);
    
    index = 1;
    row = 1;
    for i=1:height(table)
         for feature = 1:n_features
            table(i,(feature)*n_features:(feature+1)*n_features) = mean(data{index}(row,:,:));
         end

         if row > height(data{index})
             index = index + 1;
             row = 1;
         end
    end
    


    
end