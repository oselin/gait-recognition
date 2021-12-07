function [data] = transposition(varargin)
    data = varargin{1};
    if (nargin == 1)
        for i = 1:length(data)
            data{i} = transpose(data{i});
        end
    else
        if (strcmp(varargin(2),'c'))
            for i = 1:length(data)
                data{i} = transpose(categorical(data{i}));
            end
        end
    end

end