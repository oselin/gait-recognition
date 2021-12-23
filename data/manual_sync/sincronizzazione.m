clc;
clear all;
close all;

%extract data
dataIMU = readtable("..\records\record_lab_15-12-21_working\IMU.csv");
dataMitch = importdata("..\records\record_lab_15-12-21_working\mitch.txt");

%choose first frames until sync part
mitch_pick_n = 2333;
IMU_pick_n = 826;

%check
% t_mitch = (1:(numel(dataMitch.data(:,1))-mitch_pick_n+1));
% t_IMU = (1:(numel(dataIMU.AccZ_g_)-IMU_pick_n+1));
% figure 
% plot(t_mitch, dataMitch.data(mitch_pick_n:end,4));
% figure 
% plot(t_IMU, -dataIMU.AccZ_g_(IMU_pick_n:end));

%check dimension
check_IMU = -dataIMU.AccZ_g_(826:end);
check_mitch = dataMitch.data(2333:(numel(new_IMU)+2332),4);

%cut data
dataIMU([1:825],:) = [];
dataMitch.data(1:2332,:) = [];
new_lenght = numel(dataIMU(:,1));
scale_factor = 0.9766;
new_lenght./scale_factor;
for i = 1:numel(new_lenght)
    new_t = new_lenght(i);
    
end

dataMitch.data(numel(dataIMU(:,1))+1:end,: ) = [];

%add columns
dataIMU.P8_L1 = dataMitch.data(:,20);
dataIMU.P5_L2 = dataMitch.data(:,17);
dataIMU.P1_M1 = dataMitch.data(:,13);
dataIMU.P2_M2 = dataMitch.data(:,14);
dataIMU.P14_H1 = dataMitch.data(:,26);
dataIMU.P15_H2 = dataMitch.data(:,27);
dataIMU.P16_H3 = dataMitch.data(:,28);
dataIMU.P13_H4 = dataMitch.data(:,25);
dataIMU.mitch_accZ = dataMitch.data(:,4);
dataIMU.mitch_gyroZ = dataMitch.data(:,7);

t = 1:numel(dataIMU(:,"AccX_g_"));
figure 
plot(t, dataIMU.mitch_accZ);
figure 
plot(t, -dataIMU.AccZ_g_);




 
