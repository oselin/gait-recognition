%% GAIT RECOGNITION BY ML ANALYSIS ON IMU DATA

clear ;
close all;
clc
addpath("include");

%% DATA IMPORTING
try
%     file1 = readtable("data/record_10-11-21_1st_random.csv", 'VariableNamingRule','preserve');
%     file2 = readtable("data/record_10-11-21_2nd_linAccZ.csv", 'VariableNamingRule','preserve');
%     file1 = readtable("data/record_walk_21-11-21_1st_hips/record_walk_21-11-21_1st_hips.csv",'VariableNamingRule','preserve');
    file2 = readtable("data/record_walk_21-11-21_2nd_caviglia/record_walk_21-11-21_2nd_caviglia.csv",'VariableNamingRule','preserve');
    
    disp("Data successfully imported");
catch ME
    if strcmp(ME.identifier, 'MATLAB:textio:textio:FileNotFound')
        disp("ERROR: some data cannot be found");
        return;
    end
end

%% CREATION OF THE DATA STRUCTURE
time    = file2{:,2};
mydata  = file2{:,9};

%% FILTERING
[B, A] = butter(3, 0.01,"high");
 filteredData = filter(B, A,mydata);

%% DATA VISUALIZATION

figure(1)
plot(time,mydata);
hold on
plot(time,filteredData);
hold off

%% Find the MAX | MIN elements
THRESHOLD = 150;
[MM, II] = findpeaks(filteredData,'MinPeakHeight', THRESHOLD);
[mm, ii] = findpeaks(-filteredData,'MinPeakHeight',THRESHOLD);

%% Find the max | min elements
[M, I] = findpeaks(filteredData, 'MinPeakHeight', THRESHOLD/3);
[m, i] = findpeaks(-filteredData,'MinPeakHeight', THRESHOLD/3);

%% Plot the peaks
figure(2)
plot(time, filteredData);
hold on
plot(time, THRESHOLD*ones(1,length(time)), ...
     time, -THRESHOLD*ones(1,length(time)));
plot(i/100, -m, 'o', "Color","blue");
plot(ii/100, -mm, 'o', "Color",'green')
plot(II/100,MM,'o', 'Color','red');
hold off;
