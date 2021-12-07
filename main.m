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


%% Adding LABELS by threshold method
file = detectPhases(file2);

%% Creating the TEST SET and the TRANING SET
[training, test] = splitData(file,0.8);

[trainingX, trainingY] = splitLabel(training);
[testX, testY] = splitLabel(test);

%% Setting up the RNN network
%% -General settings
numFeatures = width(testX);
numHiddenUnits = 200;
numClasses = 4;

layers = [ ...
    sequenceInputLayer(numFeatures)
    lstmLayer(numHiddenUnits,'OutputMode','sequence')
    fullyConnectedLayer(numClasses)
    softmaxLayer
    classificationLayer];

%% Setting the options for the LSTM
options = trainingOptions('adam', ...
    'MaxEpochs',60, ...
    'GradientThreshold',2, ...
    'Verbose',0, ...
    'Plots','training-progress');


net = trainNetwork(trainingX,trainingY,layers,options);

%YPred = classify(net,XTest{1});