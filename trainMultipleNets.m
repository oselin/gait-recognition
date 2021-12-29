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
test = {file05, file10};

%% Labeling and preparing data to train and test the network
[XTrain,YTrain] = dataPreprocessing(train);
[XTest,YTest] = dataPreprocessing(test);

%% Parameters of the RNN network
% Constant parameters
NumFeatures = height(XTrain{1});
NumClasses = 4;
ExecutionEnvironment = 'gpu';
MiniBatchSize = 10000;

% Variable parameters
NetType = { 'gru', 'lstm'};
NHiddenLayers = [50 100 150];
MaxEpochs = [150 175 200];
GradientThreshold = [1 1.5 2];


netData = struct(...
            'netType', '', ...
            'nHiddenLayers',  0.0, ...
            'maxEpochs',  0.0, ...
            'gradientThreshold',  0.0, ...
            'net',  [], ...
            'phaseAcc',  [], ...
            'testAcc',  [], ...
            'streamAcc',  0.0 ...
        );

results = cell(length(NetType), length(NHiddenLayers), length(MaxEpochs), length(GradientThreshold));


%% Training

disp("Start training");

layers = [];
correct = zeros(1,4);
totPhases = zeros(1,4);
acc = zeros(1,length(XTest));

I = length(NetType);
J = length(NHiddenLayers);
K = length(MaxEpochs);
L = length(GradientThreshold);

%define net's layers
for i = 1:I
    netData.netType = NetType{i};
    for j = 1:J
        netData.nHiddenLayers = NHiddenLayers(j);
        if(strcmp(netData.netType,'gru')) 
            layers = [sequenceInputLayer(NumFeatures)
                gruLayer(netData.nHiddenLayers,'OutputMode','sequence')
                fullyConnectedLayer(NumClasses)
                softmaxLayer
                classificationLayer];
        elseif(strcmp(netData.netType,'lstm'))
            layers = [sequenceInputLayer(NumFeatures)
                lstmLayer(netData.nHiddenLayers,'OutputMode','sequence')
                fullyConnectedLayer(NumClasses)
                softmaxLayer
                classificationLayer];
        else
            disp('Error in param definition: netType');
            return;
        end
        
        %define training options
        for k = 1:K
            netData.maxEpochs = MaxEpochs(k);
            for l = 1:L
                netData.gradientThreshold = GradientThreshold(l);
                options = trainingOptions(...
                    'adam', ...
                    'MiniBatchSize',MiniBatchSize, ...
                    'MaxEpochs',netData.maxEpochs, ...
                    'GradientThreshold', netData.gradientThreshold, ...
                    'Verbose', false, ...
                    'Plots','none', ...
                    'ExecutionEnvironment', ExecutionEnvironment);
                
                netNumber = (i-1)*J*K*L+(j-1)*K*L+(k-1)*L+l;
                disp("Training net number: "+num2str(netNumber));

                netData.net = trainNetwork(XTrain,YTrain,layers,options);
                
                %compute accuracy
                for m = 1:length(XTest)
                    YPred = classify(netData.net,XTest{m});
                    acc(m) = sum(YPred == YTest{m})/numel(YTest{m});
                    for n = 1:4
                        totPhases(n) = totPhases(n) + sum(YTest{m} == categorical(n));
                        correct(n) = correct(n) + sum(YPred(YTest{m} == categorical(n)) == categorical(n));
                    end
                end 
                netData.phaseAcc = correct./totPhases;
                netData.testAcc = acc;

                netData.streamAcc = simulateStream(netData.net, file10, 0);
                    
                results{i,j,k,l} = netData;
            end
        end
    end
end

save('results.mat', results);