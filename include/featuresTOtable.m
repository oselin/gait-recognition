function [table] = featuresTOtable(data, vars, features)
    
    labels = cell(width(data));

    for i = 1:length(vars)
        for j = 1:length(features)
            labels{i} = char(vars{i} + "-" + features{j});
        end
    end
    table = array2table(data, 'VariableNames',labels);
end