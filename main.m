%% GAIT RECOGNITION BY ML ANALYSIS ON IMU DATA
clear ;
close all;
clc
addpath("include");

%% DATA IMPORTING
try
    file01 = readtable("data/record_walk_7-12-21_caviglia/personaA4kmh.csv","VariableNamingRule","preserve");
    file02 = readtable("data/record_walk_7-12-21_caviglia/personaB4kmh.csv","VariableNamingRule","preserve");
    file03 = readtable("data/record_walk_7-12-21_caviglia/personaC4kmh.csv","VariableNamingRule","preserve");
    file04 = readtable("data/record_walk_7-12-21_caviglia/personaD4kmh.csv","VariableNamingRule","preserve");
    file05 = readtable("data/record_walk_7-12-21_caviglia/personaE4kmh.csv","VariableNamingRule","preserve");
    disp("Data successfully imported");
catch ME
    if strcmp(ME.identifier, 'MATLAB:textio:textio:FileNotFound')
        disp("ERROR: some data cannot be found");
        return;
    end
end
%% Merging all the acquired data
[~, file]= mergeData({file01, file02, file03, file04, file05}, 'remove');

%% Adding LABELS by threshold method
file = detectPhases(file);

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
trainingY = transposition(trainingY,'c');
testX = transposition(testX);
testY = transposition(testY,'c');

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