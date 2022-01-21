%% ------------------------------------------------------------------------
%%  GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
%   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
%
%   Required packages:
%   Parallel Computing Toolbox
%   Neural Network Toolbox
%   Signal Toolbox
%   Statistics Toolbox
% -------------------------------------------------------------------------

%% ------------------------------------------------------------------------
%%  GOAL OF THE FUNCTION
%   Goal of this function is grouping data into clusters, according to
%   their similarity
% -------------------------------------------------------------------------

clear ;
close all;
clc
addpath("include");


%% DATA IMPORTING
try
    file01 = readtable('data/record_walk_7-12-21_caviglia/personaA4kmh.csv', "VariableNamingRule","preserve");
    file02 = readtable('data/record_walk_7-12-21_caviglia/personaB4kmh.csv', "VariableNamingRule","preserve");
    file03 = readtable('data/record_walk_7-12-21_caviglia/personaC4kmh.csv', "VariableNamingRule","preserve");
    file04 = readtable('data/record_walk_7-12-21_caviglia/personaD4kmh.csv', "VariableNamingRule","preserve");
    file05 = readtable('data/record_walk_7-12-21_caviglia/personaE4kmh.csv', "VariableNamingRule","preserve");
    file06 = readtable('data/record_walk_7-12-21_caviglia/personaA6kmh.csv', "VariableNamingRule","preserve");
    file07 = readtable('data/record_walk_7-12-21_caviglia/personaB6kmh.csv', "VariableNamingRule","preserve");
    file08 = readtable('data/record_walk_7-12-21_caviglia/personaC5_8kmh.csv', "VariableNamingRule","preserve");
    file09 = readtable('data/record_walk_7-12-21_caviglia/personaD6kmh.csv', "VariableNamingRule","preserve");
    file10 = readtable('data/record_walk_7-12-21_caviglia/personaE6kmh.csv', "VariableNamingRule","preserve");
    %adding cutted lab data 
    file11 = readtable('data/record_lab_15-12-21/IMU1_1.csv', "VariableNamingRule","preserve");
    file12 = readtable('data/record_lab_15-12-21/IMU1_2.csv', "VariableNamingRule","preserve");
    file13 = readtable('data/record_lab_15-12-21/IMU2_1.csv', "VariableNamingRule","preserve");
    file14 = readtable('data/record_lab_15-12-21/IMU3_1.csv', "VariableNamingRule","preserve");
    file15 = readtable('data/record_lab_15-12-21_afternoon/IMU4_1.csv', "VariableNamingRule","preserve");

    disp("Data successfully imported");
catch ME
    if strcmp(ME.identifier, 'MATLAB:textio:textio:FileNotFound')
        disp("ERROR: some data cannot be found");
        return;
    end
end

train = {file01, file02, file03, file04, file06, file07, file08, file09, file11, file12, file13, file14, file15};
test  = {file05, file10};

acc_train = [0 0];
acc_test  = [0 0];
%% START OF K-MEANS METHOD
% A for-cycle is used to run it twice. The first time no feature
% estimations are applied, the second time yes
for i=1:2
    if i==1
        %% [NO FEATURES] Labeling and preparing data to train and test the network
        processed_train = dataPreprocessingUnsupervised(train);
        processed_test  = dataPreprocessingUnsupervised(test );
        disp("Starting K_Means Method on raw data");
    else
        %% [FEATURES] Labeling and preparing data to train and test the network
        processed_train = dataPreprocessingUnsupervised(train,'features',150);
        processed_test  = dataPreprocessingUnsupervised(test, 'features',150);
        disp("Starting K_Means Method on feature data");
    end

    %% Setting data properly for unsupervised learning
    Xtrain = processed_train(:,1:end-1);
    Ytrain = processed_train(:,end);
    
    Xtest = processed_test(:,1:end-1);
    Ytest = processed_test(:, end);
    
    %% Unsupervised Learning: k-Means
    [idx, C] = kmeans(Xtrain, ...
                      4, ...
                      "Replicates", 60 ...
                      );

    % Train accuracy is computed
    acc_train(i) = sum(idx==Ytrain)./numel(idx);
    
    %% Prediction for the unsupervised learning
    [~,idx_test] = pdist2(C,Xtest,'euclidean','Smallest',1);

    % Test accuracy is computed
    acc_test(i) = sum(idx_test==Ytest')./numel(idx_test);

    %%
    f = figure(i);
    gscatter(Xtrain(:,1),Xtrain(:,2),idx,'rbcg')
    hold on
    plot(C(:,1),C(:,2),'kx')
    gscatter(Xtest(:,1),Xtest(:,2),idx_test,'bgmr','oooo')
    legend('Cluster 1','Cluster 2','Cluster 3','Cluster 4','Cluster Centroid', ...
        'Data classified to Cluster 1','Data classified to Cluster 2', ...
        'Data classified to Cluster 3','Data classified to Cluster 4')
    title("k-Means clustering")
    xlabel("Acc(X)")
    ylabel("Acc(y)")
    hold off
end

%% Displaying the results
for i = 1:2
    if i == 1
        mode = "[NOFEATUR] ";
    else
        mode = "[FEATURES] ";
    end

    disp(mode + "Unsupervised [kMeans] train accuracy: " + num2str(acc_train(i)));
    disp(mode + "Unsupervised [kMeans] test  accuracy: " + num2str(acc_test(i)));
end