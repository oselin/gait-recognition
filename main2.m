%% GAIT RECOGNITION BY ML ANALYSIS ON IMU DATA

clear ;
close all;
clc
addpath("include");

%% MACROS
NCOLUMNS = 150;
STARTING_FS = 100;

activities = ["PhaseONE","PhaseTWO","PhaseTHREE", "PhaseFOUR"];

features_avaiable = [
                       "%s mean%s",...                  %01
                       "%s std%s",...                   %02
                       "%s peak%s",...                  %03
                       "%s peak position%s",...         %04
                       "%s mediana%s",...               %05
                       "%s variance%s",...              %06
                       "%s covariance%s",...            %07
                       "%s range%s",...                 %08
                       "%s rms%s",...                   %09
                       "%s mode%s",...                  %10
                       "%s mean or median abs dev%s",...    %11
                       %"%s meanFFT%s"
                       ];


%% DATA IMPORTING
try
    file1 = readtable("data/record_10-11-21_1st_random.csv", 'VariableNamingRule','preserve');
    file2 = readtable("data/record_10-11-21_2nd_linAccZ.csv", 'VariableNamingRule','preserve');
catch ME
    if strcmp(ME.identifier, 'MATLAB:textio:textio:FileNotFound')
        disp("ERROR: some data cannot be found");
        return;
    end
end


%% CREATION OF THE DATA STRUCTURE

dataset = {file1, file2};
[time, data] = mergeData(dataset);

%% DATA VISUALIZATION
yyaxis left, plot(time,data(:,3));
hold on
yyaxis right,plot(time,data(:,1));
set(gca, 'YTick', data(1,1):data(end,1));

%% MULTI-LAYER STRUCTURE
m = setMultiLayerStruct(data, NCOLUMNS);


