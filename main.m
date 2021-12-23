%% GAIT RECOGNITION BY ML ANALYSIS ON IMU DATA
clear ;
close all;
clc
addpath("include");

%% DATA IMPORTING
try
    file01 = readtable("data/record_lab_15-12-21_working/IMU.csv", "VariableNamingRule","preserve");
    file02 = readtable("data/record_lab_15-12-21/IMU_1.csv", "VariableNamingRule","preserve");
    file03 = readtable("data/record_lab_15-12-21/IMU_1.csv", "VariableNamingRule","preserve");
    file04 = readtable("data/record_lab_15-12-21/IMU_1.csv", "VariableNamingRule","preserve");
    disp("Data successfully imported");
catch ME
    if strcmp(ME.identifier, 'MATLAB:textio:textio:FileNotFound')
        disp("ERROR: some data cannot be found");
        return;
    end
end
file = {file01, file02, file03, file04};
%% Adding LABELS by threshold method
%file{1} = detectPhases_new(file{1});
for i = 1:length(file)
    file{i} = detectPhases_new(file{i});
end

%% Merging all the acquired data
%[~, file]= mergeData({file01, file02, file03, file04, file05}, 'remove');
[~, file]= mergeData(file, 'remove');

%% Plot labeled data
plotLabeledData(file(1:8965,:));

%% Creating the TEST SET and the TRANING SET
[training, test] = splitData(file,0.8);

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
    'MaxEpochs',60, ...
    'GradientThreshold',2, ...
    'Verbose',0, ...
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

%% Prediction of the classes (GAIT PHASES)
YPred = classify(net,testX{1});

%% Accuracy of the network
acc = sum(YPred == testY{1})./numel(testY{1})

%% Data visualization
%dataVisualization('data/record_walk_21-11-21_2nd_caviglia/WIN_20211121_14_46_37_Pro.mp4',27,file);

%% Setting data properly for unsupervised learning
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

%acc_kmeans = sum(idx_test==Ytest)/numel(idx_test)

%% Show the results
N = 1000;
gscatter(Xtrain(1:N,1),Xtrain(1:N,2),idx(1:N),"rgcb")
hold on
plot(C(:,1),C(:,2),'kx')
gscatter(Xtest(1:N,1),Xtest(1:N,2),idx_test(1:N), "rgcb" ,'o')
legend('Cluster 1','Cluster 2','Cluster 3','Cluster 4','Cluster Centroid', ...
    'Data classified to Cluster 1','Data classified to Cluster 2', ...
    'Data classified to Cluster 3','Data classified to Cluster 4')
