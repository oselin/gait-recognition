function [ff, output] = freqTransform(timeData, fs)

    %For a better frequency-domain transformation, it's better to subtract
    %the mean value of the series

    intervals = unique(timeData(:,1));

    %Empty matrix to fill with the mean values
    mean_matrix = zeros(height(timeData), width(timeData));   
    
    for i = 1:intervals

        %Temp sub-matrix on which calculate the mean
        submatrix = timeData(timeData(:,1)==i,:);
      
        mean_matrix(timeData(:,1)==i,:) = mean_matrix(timeData(:,1)==i,:) + mean(submatrix);

    end

    timeData = [timeData(:,1), timeData(:, 2:end) - mean_matrix(:,2:end)];


    %Nyquist frequence
    fN = fs/2;
    
    %sample total number
    n = height(timeData);
    
    %Frequences vector [normalized]
    ff = (0:fs/n:fN)/fN;

    %Normalized Fourier transform
    y = fft(timeData)/n;
    
    
    %PLOT della fft: NOTA-> uso il valore assoluto di fft! Inoltre mi fermo a
    %metà dati ->freq positive
    
    output = abs(y(1:(ceil((n+1)/2)),:));



end
