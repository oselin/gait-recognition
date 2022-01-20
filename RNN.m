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

%define usage of each file
train = {file01, file02, file03, file04, file06, file07, file08, file09, file11, file12, file13, file14, file15};
test  = {file05, file10};

%% Labeling and preparing data to train and test the network
% see dataPreprocessing.m
[XTrain,YTrain] = dataPreprocessing(train);
[XTest,YTest]   = dataPreprocessing(test);

%% Setting up the RNN network
%% -General settings
numFeatures = height(XTrain{1});
numHiddenUnits = 100;
numClasses = 4;

layers = [ ...
    % input layer
    sequenceInputLayer(numFeatures)
    % recurrent layers: 
    % numHiddenUnits indicates number of hidden layers
    % OutputMode = 'sequence' indicates that the net must classify each 
    % time instant of the streaming
    lstmLayer(numHiddenUnits,'OutputMode','sequence')
    % decision layer
    fullyConnectedLayer(numClasses)
    softmaxLayer
    % output layer
    classificationLayer];

%% -Setting the options for training
miniBatchSize = 1000;
maxEpochs = 250;
gradientThreshold = 2;
executionEnvironment = 'cpu';

options = trainingOptions(...
    'adam', ...
    'MiniBatchSize',miniBatchSize, ...
    'MaxEpochs',maxEpochs, ...
    'GradientThreshold', gradientThreshold, ...
    'Verbose', 0, ...
    'Plots','none', ...
    'ExecutionEnvironment', executionEnvironment);

%train or load
% net = trainNetwork(XTrain,YTrain,layers,options);
net = results{1,1,1,1}.net;    

%% Plot of the testing data
% figure
% plot(testX{1}')
% xlabel("Time Step")
% legend("Feature " + (1:numFeatures))
% title("Test Data")

%% Prediction of the classes (GAIT PHASES) and ACCURACY
% phase accuracy
disp("Accuracy per phase:")
correct = zeros(1,4); % correct predictions
totPhases = zeros(1,4); % tot number of predictions
% for each test case
for i = 1:length(XTest)
    % evaluate network
    YPred = classify(net,XTest{i});
    % for each phase
    for j = 1:4
        % update number of time instant with that phase
        totPhases(j) = totPhases(j) + sum(YTest{i} == categorical(j));
        % update number of correct predictions for that phase
        correct(j) = correct(j) + sum(YPred(YTest{i} == categorical(j)) == categorical(j));      
    end
end 
% display ratio result
disp(correct./totPhases);

% test accuracy
disp("Accuracy per test set")
acc = zeros(1,length(XTest));
% for each test
for i = 1:length(XTest)
    % evaluate network
    YPred = classify(net,XTest{i});
    % compute accuracy for the used test
    % correct predictions / tot number of predictions
    acc(i) = sum(YPred == YTest{i})/numel(YTest{i});
end 
% display ratio result
disp(acc);

%% SIMULATE THE DATASTREAM
% see simulateStream.m
disp(simulateStream(net, file10, 0, 1));