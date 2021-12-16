% function [] = syncData()
clc
clear all
close all
file01 = load("../data/synchedData_IMU_mitch.mat");
file02 = readtable("../data/record_lab_15-12-21/mitch_1_1.txt","VariableNamingRule","preserve");
file03 = readtable("../data/record_lab_15-12-21/mitch_1_3.txt","VariableNamingRule","preserve");

data = {file01{:,6}, file02{:,4}, file03{:,4}};
for i=13:28
    plot(1:height(file03{:,i}),file03{:,i});
    hold on
end
% fs = 100;
% [B,A] = butter(5,0.1,"high");
%     for i = 1:length(data)
%         fN = fs/2;
%         n = length(data{i});
%         myfft = fft(data{i})/n;
%         myfft = filter(B,A,myfft);
%         myfft = abs(myfft(1:(ceil((n+1)/2))));
%         
%         figure(i);
%         ff = (0:fs/n:fN)/fN;
%         plot(ff, myfft);
%     end
% 
% 
% %%
% 
% clear all;
% close all;
% clc;
% file01 = readtable("../data/record_lab_15-12-21/IMU_1.csv", "VariableNamingRule","preserve");
% file02 = readtable("../data/record_lab_15-12-21/mitch_1_1.txt","VariableNamingRule","preserve");
% file03 = readtable("../data/record_lab_15-12-21/mitch_2_1.txt","VariableNamingRule","preserve");
% 
% % ffile01 = file01{:,6};% - mean(file01{:,6});
% % ffile02 = file02{:,4};% - mean(file02{:,4});
% % ffile03 = file03{:,4};% - mean(file03{:,4});
% 
% ffile01 = (file01{:,6})/range(file01{:,6});
% ffile02 = (file02{:,4})/range(file02{:,4});
% ffile03 = (file03{:,4})/range(file03{:,4});
% disp("filtered");
% [p1, off1] = max(ffile01);
% [p2, off2] = max(ffile02);
% [p3, off3] = max(ffile03);
% disp("max found");
% 
% LEN = min([height(file01),height(file02), height(file03)]);
% disp("plotting...");
% figure(1)
% %plot(1:LEN,ffile01(1:LEN),'Color','b');
% xlim([0,4e4]);
% hold on
% % figure(2)
% plot(1:LEN,ffile02(1:LEN)/6000,'Color','y');
% xlim([0,4e4]);
% % figure(3)
% plot(1:LEN,ffile03(1:LEN)/4000,'Color','r');
% xlim([0,4e4]);
% % yline(p1);
% % yline(p2/6000);
% % yline(p3/4000);





% 
% end