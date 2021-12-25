clear ;
close all;
clc
addpath("include");
dataIMU = readtable('data/record_lab_15-12-21/IMU_2.csv','VariableNamingRule','preserve');
% dataIMU(1:26410,:) = [];
% dataIMU1 = dataIMU(1:5400,:);
dataIMU2 = dataIMU(23100:end,:);

% t_IMU1 = 1:(numel(dataIMU1.("AccZ (g)")));
t_IMU2 = 1:(numel(dataIMU2.("AccZ (g)")));
% figure 
% plot(t_IMU1, -dataIMU1.("AccZ (g)")(1:end));
figure 
plot(t_IMU2, -dataIMU2.("AccZ (g)")(1:end));

writetable(dataIMU2,"data/record_lab_15-12-21/IMU2_1.csv");
