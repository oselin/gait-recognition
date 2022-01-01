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
test  = {file05, file10};

%% Labeling and preparing data to train and test the network
[XTrain,YTrain] = dataPreprocessing(train);
[XTest,YTest]   = dataPreprocessing(test);

%% Parameters of the RNN network
% Constant parameters
NumFeatures = height(XTrain{1});
NumClasses = 4;
ExecutionEnvironment = 'cpu';
MiniBatchSize = 1000;

% Variable parameters
NetType = { 'gru', 'lstm'};
NHiddenLayers     = [50  100 150];
MaxEpochs         = [150 175 200];
GradientThreshold = [1   1.5 2];

% struct were save net, params and accuracies
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

I = length(NetType);
J = length(NHiddenLayers);
K = length(MaxEpochs);
L = length(GradientThreshold);
results = cell(I, J, K, L);

%% Training

disp("Start training");

layers = []; %net structure

% to compute phase accuracy
correct   = zeros(1,NumClasses); %result from classification
totPhases = zeros(1,NumClasses); %correct label

% to compute test accuracy
acc = zeros(1,length(XTest));

%define net's layers
for i = 1:I
    %net type
    netData.netType = NetType{i};
    for j = 1:J
        % n of hidden layers
        netData.nHiddenLayers = NHiddenLayers(j);
        % build net
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
            % n of epochs
            netData.maxEpochs = MaxEpochs(k);
            for l = 1:L
                % gradient threshold
                netData.gradientThreshold = GradientThreshold(l);
                % build option struct
                options = trainingOptions(...
                    'adam', ...
                    'MiniBatchSize',MiniBatchSize, ...
                    'MaxEpochs',netData.maxEpochs, ...
                    'GradientThreshold', netData.gradientThreshold, ...
                    'Verbose', false, ...
                    'Plots','none', ...
                    'ExecutionEnvironment', ExecutionEnvironment);
                
                %index of trained net
                netNumber = (i-1)*J*K*L+(j-1)*K*L+(k-1)*L+l;
                disp("Training net number: "+num2str(netNumber));
                
                %training
                netData.net = trainNetwork(XTrain,YTrain,layers,options);
                
                %compute accuracy
                for m = 1:length(XTest)
                    %prediction
                    YPred = classify(netData.net,XTest{m}); 
                    %test accuracy
                    acc(m) = sum(YPred == YTest{m})/numel(YTest{m}); 
                    for n = 1:NumClasses %for each phase
                        % n of tot labels for each phase
                        totPhases(n) = totPhases(n) + sum(YTest{m} == categorical(n)); 
                        % n of correct prediction for each label
                        correct(n) = correct(n) + sum(YPred(YTest{m} == categorical(n)) == categorical(n)); 
                    end
                end 
                netData.phaseAcc = correct./totPhases; %phase acc
                netData.testAcc = acc; %test acc

                netData.streamAcc = simulateStream(netData.net, file10, 0, 0);%stream acc
                    
                results{i,j,k,l} = netData; %saving net
            end
        end
    end
end

save('results.mat', 'results');
