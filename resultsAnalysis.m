%% ------------------------------------------------------------------------
%   GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
%   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
%
%   Required packages:
%   Parallel Computing Toolbox
%   Neural Network Toolbox
%   Signal Toolbox
%   Statistics Toolbox
% -------------------------------------------------------------------------
clear ;
close all;
clc
addpath("include");
addpath("output");

load("results.mat");
[I,J,K,L] = size(results);

streamAcc = zeros(I, J, K, L);
meanTestAcc = zeros(I, J, K, L);
meanPhaseAcc = zeros(I, J, K, L);
flatten_result = cell(54,1);

%% Overall analysis
disp("Overall analysis");

for i = 1:I
    for j = 1:J
        for k = 1:K
            for l = 1:L
                flatten_result{(i-1)*J*K*L+(j-1)*K*L+(k-1)*L+l} = results{i,j,k,l};
                streamAcc(i,j,k,l) = results{i,j,k,l}.streamAcc;
                meanTestAcc(i,j,k,l) = mean(results{i,j,k,l}.testAcc);
                meanPhaseAcc(i,j,k,l) = mean(results{i,j,k,l}.phaseAcc);
            end
        end
    end
end

[MstreamAcc,IstreamAcc] = max(streamAcc,[],"all");
[MtestAcc,ItestAcc] = max(meanTestAcc,[],"all");
[MphaseAcc,IphaseAcc] = max(meanPhaseAcc,[],"all");
disp(flatten_result{IstreamAcc});
disp(flatten_result{ItestAcc});
disp(flatten_result{IphaseAcc});

%% Net type analysis

markerSize = 3;
gru = zeros(3,3);
ltsm = zeros(3,3);
x = zeros(1,3);

t = tiledlayout('flow','TileSpacing','Compact');
title(t,'Networks accuracy by type, varying:');

%nHiddenLayers

for j= 1:J
    x(j) = results{1,j,1,1}.nHiddenLayers;
    gru(1,j) = mean(streamAcc(1,j,:,:),"all");
    gru(2,j) = mean(meanTestAcc(1,j,:,:),"all");
    gru(3,j) = mean(meanPhaseAcc(1,j,:,:),"all");
    ltsm(1,j) = mean(streamAcc(2,j,:,:),"all");
    ltsm(2,j) = mean(meanTestAcc(2,j,:,:),"all");
    ltsm(3,j) = mean(meanPhaseAcc(2,j,:,:),"all");
end

nexttile
hold on
plot(x, gru(1,:), 'r-o', "MarkerSize", markerSize);
plot(x, gru(2,:), 'r--o', "MarkerSize", markerSize);
plot(x, gru(3,:), 'r:o', "MarkerSize", markerSize);
plot(x, ltsm(1,:), 'b-o', "MarkerSize", markerSize);
plot(x, ltsm(2,:), 'b--o', "MarkerSize", markerSize);
plot(x, ltsm(3,:), 'b:o', "MarkerSize", markerSize);
hold off
% legend("GRU streamAcc", "GRU testAcc", "GRU phaseAcc", "LTSM streamAcc", "LTSM testAcc", "LTSM phaseAcc");
xlabel('N of hidden layers');

%maxEpochs

for k= 1:K
    x(k) = results{1,1,k,1}.maxEpochs;
    gru(1,k) = mean(streamAcc(1,:,k,:),"all");
    gru(2,k) = mean(meanTestAcc(1,:,k,:),"all");
    gru(3,k) = mean(meanPhaseAcc(1,:,k,:),"all");
    ltsm(1,k) = mean(streamAcc(2,:,k,:),"all");
    ltsm(2,k) = mean(meanTestAcc(2,:,k,:),"all");
    ltsm(3,k) = mean(meanPhaseAcc(2,:,k,:),"all");
end

nexttile
hold on
plot(x, gru(1,:), 'r-o', "MarkerSize", markerSize);
plot(x, gru(2,:), 'r--o', "MarkerSize", markerSize);
plot(x, gru(3,:), 'r:o', "MarkerSize", markerSize);
plot(x, ltsm(1,:), 'b-o', "MarkerSize", markerSize);
plot(x, ltsm(2,:), 'b--o', "MarkerSize", markerSize);
plot(x, ltsm(3,:), 'b:o', "MarkerSize", markerSize);
hold off
hleg1 = legend(["GRU streamAcc", "GRU testAcc", "GRU phaseAcc", "LTSM streamAcc", "LTSM testAcc", "LTSM phaseAcc"], ...
    'FontSize',14);
set(hleg1,'position',[0.6 0.1 0.3 0.3])
xlabel('N of epochs');

%gradientThreshold

for l= 1:L
    x(l) = results{1,1,1,l}.gradientThreshold;
    gru(1,l) = mean(streamAcc(1,:,:,l),"all");
    gru(2,l) = mean(meanTestAcc(1,:,:,l),"all");
    gru(3,l) = mean(meanPhaseAcc(1,:,:,l),"all");
    ltsm(1,l) = mean(streamAcc(2,:,:,l),"all");
    ltsm(2,l) = mean(meanTestAcc(2,:,:,l),"all");
    ltsm(3,l) = mean(meanPhaseAcc(2,:,:,l),"all");
end

nexttile
hold on
plot(x, gru(1,:), 'r-o', "MarkerSize", markerSize);
plot(x, gru(2,:), 'r--o', "MarkerSize", markerSize);
plot(x, gru(3,:), 'r:o', "MarkerSize", markerSize);
plot(x, ltsm(1,:), 'b-o', "MarkerSize", markerSize);
plot(x, ltsm(2,:), 'b--o', "MarkerSize", markerSize);
plot(x, ltsm(3,:), 'b:o', "MarkerSize", markerSize);
hold off
% legend("GRU streamAcc", "GRU testAcc", "GRU phaseAcc", "LTSM streamAcc", "LTSM testAcc", "LTSM phaseAcc");
xlabel('Gradient threshold');

return

disp("  N hidden layers");
disp("  nHiddenLayers      stream            test              phase");
disp("GRU");
for j= 1:J
    disp("          "+ ...
        num2str(results{1,j,1,1}.nHiddenLayers)+"            "+ ...
        num2str(mean(streamAcc(1,j,:,:),"all"))+"             "+ ...
        num2str(mean(meanTestAcc(1,j,:,:),"all"))+"           "+ ...
        num2str(mean(meanPhaseAcc(1,j,:,:),"all")) );
end
    disp("LTSM");
for j= 1:J
    disp("          "+ ...
        num2str(results{2,j,1,1}.nHiddenLayers)+"            "+ ...
        num2str(mean(streamAcc(2,j,:,:),"all"))+"             "+ ...
        num2str(mean(meanTestAcc(2,j,:,:),"all"))+"           "+ ...
        num2str(mean(meanPhaseAcc(2,j,:,:),"all")) );
end

disp("  N of epochs");
disp("  maxEpochs      stream            test              phase");
disp("GRU");
for k= 1:K
    disp("          "+ ...
        num2str(results{1,1,k,1}.maxEpochs)+"            "+ ...
        num2str(mean(streamAcc(1,:,k,:),"all"))+"             "+ ...
        num2str(mean(meanTestAcc(1,:,k,:),"all"))+"           "+ ...
        num2str(mean(meanPhaseAcc(1,:,k,:),"all")) );
end
disp("LTSM");
for k= 1:K
    disp("          "+ ...
        num2str(results{2,1,k,1}.gradientThreshold)+"            "+ ...
        num2str(mean(streamAcc(2,:,k,:),"all"))+"             "+ ...
        num2str(mean(meanTestAcc(2,:,k,:),"all"))+"           "+ ...
        num2str(mean(meanPhaseAcc(2,:,k,:),"all")) );
end

disp("  Gradient threshold");
disp("  gradientThreshold      stream            test              phase");
disp("GRU");
for l= 1:L
    disp("          "+ ...
        num2str(results{1,1,1,l}.gradientThreshold)+"            "+ ...
        num2str(mean(streamAcc(1,:,:,l),"all"))+"             "+ ...
        num2str(mean(meanTestAcc(1,:,:,l),"all"))+"           "+ ...
        num2str(mean(meanPhaseAcc(1,:,:,l),"all")) );
end
disp("LTSM");
for l= 1:L
    disp("          "+ ...
        num2str(results{2,1,1,l}.gradientThreshold)+"            "+ ...
        num2str(mean(streamAcc(2,:,:,l),"all"))+"             "+ ...
        num2str(mean(meanTestAcc(2,:,:,l),"all"))+"           "+ ...
        num2str(mean(meanPhaseAcc(2,:,:,l),"all")) );
end





