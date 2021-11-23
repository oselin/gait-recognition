function [table] = featuresTOtable(data, vars, features)
    
    labels = cell(1,width(data));
    
    buffer = 1;
    for i = 4:length(vars)
        for j = 1:length(features)
            labels{buffer} = char(vars{i} + "-" + features{j});
            buffer = buffer + 1;
        end
    end
    labels{end} = 'ID';
    %labels = convertStringsToChars(labels);
    
    table = array2table(data, 'VariableNames',labels);
    
end