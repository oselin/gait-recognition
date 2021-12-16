function [data] = transposition(varargin)
    %transpose data in order to be read properly by the RNN 
    data = varargin{1};
    if (nargin == 1)
        for i = 1:length(data)
            data{i} = transpose(data{i});
        end
    elseif (nargin == 2 && strcmp(varargin(2),'categorical'))
        for i = 1:length(data)
            data{i} = transpose(categorical(data{i}));
        end
    else
        disp("Something went wrong, please check your function input");
        return;
    end
end