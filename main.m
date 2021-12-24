%% GAIT RECOGNITION BY ML ANALYSIS ON IMU DATA
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
%     file06 = readtable('data/record_lab_15-12-21/IMU_1.csv', "VariableNamingRule","preserve");
%     file07 = readtable('data/record_lab_15-12-21/IMU_2.csv', "VariableNamingRule","preserve");
%     file08 = readtable('data/record_lab_15-12-21/IMU_3.csv', "VariableNamingRule","preserve");
    disp("Data successfully imported");
catch ME
    if strcmp(ME.identifier, 'MATLAB:textio:textio:FileNotFound')
        disp("ERROR: some data cannot be found");
        return;
    end
end
file = {file01, file02, file03, file04};

%% Adding LABELS by threshold method
for i = 1:length(file)
    file{i} = detectPhases_new_2(file{i});
end
file05 = detectPhases_new_2(file05);
[~, file05] = mergeData({file05}, 'remove');
%% Merging all the acquired data
[~, file]= mergeData(file, 'remove');

%% Plot labeled data
plotLabeledData(file(1:8965,:));

%% Creating the TEST SET and the TRANING SET
%[training, test] = splitData(file,0.8);

training = file;
test = file05;

[trainingX, trainingY] = splitLabel(training);
[testX, testY]         = splitLabel(test);

%% Setting up the RNN network
%% -General settings
numFeatures = width(training) - 1;
numHiddenUnits = 200;
numClasses = 4;

layers = [ ...
    sequenceInputLayer(numFeatures)
    lstmLayer(numHiddenUnits,'OutputMode','sequence')
    fullyConnectedLayer(numClasses)
    softmaxLayer
    classificationLayer];

%% -Setting the options for the LSTM
options = trainingOptions('adam', ...
    'MaxEpochs', 60, ...
    'GradientThreshold', 2, ...
    'Verbose', 0, ...
    'Plots','training-progress');

trainingX = transposition(trainingX);
trainingY = transposition(trainingY,'categorical');
testX = transposition(testX);
testY = transposition(testY,'categorical');

net = trainNetwork(trainingX,trainingY,layers,options);

%% Plot of the testing data
figure
plot(testX{1}')
xlabel("Time Step")
legend("Feature " + (1:numFeatures))
title("Test Data")

%% Prediction of the classes (GAIT PHASES) and ACCURACY
for i = 1:length(testX)
    YPred = classify(net,testX{i});
    acc = sum(YPred == testY{i})/numel(testY{i});
    disp("Accuracy for phase "+ num2str(i)+": " + num2str(acc));
end

%% Data visualization
%dataVisualization('data/record_walk_21-11-21_2nd_caviglia/WIN_20211121_14_46_37_Pro.mp4',27,file);

%% Setting data properly for unsupervised learning
if isa(training, 'table')
    training = training{:,:};
end
if isa(test, 'table')
    test = test{:,:};
end
Xtrain = training(:,1:end-1);
Ytrain = training(:,end);

Xtest = test(:,1:end-1);
Ytest = test(:, end);

%% Unsupervised Learning: k-Means
[idx, C] = kmeans(Xtrain, ...
                  4, ...
                  "Display","final", ...
                  "Replicates", 30 ...
                  );
comp = [idx,Ytrain];

%% Prediction for the unsupervised learning
[~,idx_test] = pdist2(C,Xtest,'euclidean','Smallest',1);

acc_kmeans = sum(idx_test==Ytest')./numel(idx_test);
disp("Unsupervised [kMeans] accuracy: " + num2str(acc_kmeans));

%% Show the results
N = 1:9900;
gscatter(Xtrain(N,7),Xtrain(N,8),idx(N)',"rgcb")
hold on
plot(C(:,1),C(:,2),'kx')
gscatter(Xtest(N,7),Xtest(N,8),idx_test(N)', "rgcb" ,'o')
legend('Cluster 1','Cluster 2','Cluster 3','Cluster 4','Cluster Centroid', ...
    'Data classified to Cluster 1','Data classified to Cluster 2', ...
    'Data classified to Cluster 3','Data classified to Cluster 4')
