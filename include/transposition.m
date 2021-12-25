function [data] = transposition(varargin)
    %% --------------------------------------------------------------------
    %   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
    %   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
    %
    %   TRANSPOSE FUNCTION
    % ---------------------------------------------------------------------
    
    %transpose data in order to be read properly by the RNN 

    if (nargin < 1)
        disp("Something went wrong. Please check your inputs");
        return
    end

    data = varargin{1};
    if (nargin == 1)
        for i = 1:length(data)
            data{i} = transpose(data{i});
        end
    elseif (nargin == 2 && strcmp(varargin(2),'categorical'))
        %RNN wants labels as CATEGORICAL type data
        for i = 1:length(data)
            data{i} = transpose(categorical(data{i}));
        end
    else
        disp("Something went wrong, please check your function input");
        return;
    end
end