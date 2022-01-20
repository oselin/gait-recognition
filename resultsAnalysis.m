%% ------------------------------------------------------------------------
%%  GAIT RECOGNITION BASED ON IMU DATA AND ML ALGORITHM
%   Albi Matteo, Cardone Andrea, Oselin Pierfrancesco
%
%   Required packages:
%   Parallel Computing Toolbox
%   Neural Network Toolbox
%   Signal Toolbox
%   Statistics Toolbox
%
%%  Analysis from results of function trainMultipleNets   
% -------------------------------------------------------------------------

%% ------------------------------------------------------------------------
%%  GOAL OF THE FUNCTION
%   Goal of this function is evaluate the performances of the trained
%   network in order to choose the best one.
%   We consider phase accuracy, test accuracy and stream accuracy
% -------------------------------------------------------------------------
clear ;
close all;
clc
addpath("include");
addpath("output");

try
    load("results.mat"); %trained nets
catch ME
    if strcmp(ME.identifier, 'MATLAB:textio:textio:FileNotFound')
        disp("ERROR: result data cannot be found");
        return;
    end
end
[I,J,K,L] = size(results);

streamAcc      = zeros(I, J, K, L); %stream accuracy
meanTestAcc    = zeros(I, J, K, L); %test mean accuracy
meanPhaseAcc   = zeros(I, J, K, L); %phases mean accuracy

% Monodimensional array definition to evaluate max performances
flatten_result = cell(54,1); %trained nets in monodimensional array
flatten_streamAcc = zeros(54,1); %stream accuracy in monodimensional array
flatten_meanTestAcc = zeros(54,1); %test accuracy in monodimensional array
flatten_meanPhaseAcc = zeros(54,1); %phases accuracy in monodimensional array



%% Overall analysis
disp("Overall analysis");

%fill previously defined structures
for i = 1:I
    for j = 1:J
        for k = 1:K
            for l = 1:L
                % save net in monodimensional array
                flatten_result{(i-1)*J*K*L+(j-1)*K*L+(k-1)*L+l} = results{i,j,k,l};
                % compute streaming accuracy and save it (see 
                % simulateStream.m)
                streamAcc(i,j,k,l)    = results{i,j,k,l}.streamAcc;
                % compute mean accuracy of the tests and save it (see
                % trainMultipleNets.m)
                meanTestAcc(i,j,k,l)  = mean(results{i,j,k,l}.testAcc);
                % compute mean accuracy of the phases and save it (see
                % trainMultipleNets.m)
                meanPhaseAcc(i,j,k,l) = mean(results{i,j,k,l}.phaseAcc);
                % save accuracies in monodimensional array
                flatten_streamAcc((i-1)*J*K*L+(j-1)*K*L+(k-1)*L+l) = streamAcc(i,j,k,l);
                flatten_meanTestAcc((i-1)*J*K*L+(j-1)*K*L+(k-1)*L+l) = meanTestAcc(i,j,k,l);
                flatten_meanPhaseAcc((i-1)*J*K*L+(j-1)*K*L+(k-1)*L+l) = meanPhaseAcc(i,j,k,l);
            end
        end
    end
end

%compute max for each accuracy
[MstreamAcc,IstreamAcc] = max(flatten_streamAcc);
[MtestAcc,ItestAcc] = max(flatten_meanTestAcc);
[MphaseAcc,IphaseAcc] = max(flatten_meanPhaseAcc);
%desplay related net
disp(flatten_result{IstreamAcc});
disp(flatten_result{ItestAcc});
disp(flatten_result{IphaseAcc});

%% Net type analysis
% For each type of net layers (GRU and LSTM) it's calcualted the average
% accuracies (stream, test, phase) for each param: 
% N of hidden layers (3 cases)
% N of epochs for training (3 cases)
% Gradient threshold (3 cases)
%
% The avarage for each param's value is calculated among all nets with that
% param value, example:
% GRU net, max epochs = 100
% the avarage is computed on nets of type GRU, max n of epochs equal to
% 100, and all values of n of hidden layers and gradient threshold (in
% total 9 different results)
% ------------------------------------------------

markerSize = 3;
gru = zeros(3,3); %values for gru-type nets
lstm = zeros(3,3); %values for lstm-type nets
x = zeros(1,3); %x-axis values

%nHiddenLayers

for j= 1:J %for each nHiddenLayers value
    % compute mean accuracy among other params (maxEpochs, gradientThreshold):
    x(j) = results{1,j,1,1}.nHiddenLayers;
    gru(1,j) = mean(streamAcc(1,j,:,:),"all"); %mean stream acc for gru nets
    gru(2,j) = mean(meanTestAcc(1,j,:,:),"all"); %mean test acc for gru nets
    gru(3,j) = mean(meanPhaseAcc(1,j,:,:),"all"); %mean phase acc for gru nets
    lstm(1,j) = mean(streamAcc(2,j,:,:),"all"); %mean stream acc for lstm nets
    lstm(2,j) = mean(meanTestAcc(2,j,:,:),"all"); %mean test acc for lstm nets
    lstm(3,j) = mean(meanPhaseAcc(2,j,:,:),"all"); %mean phase acc for lstm nets
end

fig = figure(1);
%results plot
subplot(2,2,1);
hold on
plot(x, gru(1,:), 'r-o', "MarkerSize", markerSize);
plot(x, gru(2,:), 'r--o', "MarkerSize", markerSize);
plot(x, gru(3,:), 'r:o', "MarkerSize", markerSize);
plot(x, lstm(1,:), 'b-o', "MarkerSize", markerSize);
plot(x, lstm(2,:), 'b--o', "MarkerSize", markerSize);
plot(x, lstm(3,:), 'b:o', "MarkerSize", markerSize);
hold off
% legend("GRU streamAcc", "GRU testAcc", "GRU phaseAcc", "lstm streamAcc", "lstm testAcc", "lstm phaseAcc");
xlabel('N of hidden layers');

%maxEpochs

for k= 1:K %for each maxEpochs value
    % compute mean accuracy among other params (nHiddenLayers, gradientThreshold):
    x(k) = results{1,1,k,1}.maxEpochs;
    gru(1,k) = mean(streamAcc(1,:,k,:),"all");
    gru(2,k) = mean(meanTestAcc(1,:,k,:),"all");
    gru(3,k) = mean(meanPhaseAcc(1,:,k,:),"all");
    lstm(1,k) = mean(streamAcc(2,:,k,:),"all");
    lstm(2,k) = mean(meanTestAcc(2,:,k,:),"all");
    lstm(3,k) = mean(meanPhaseAcc(2,:,k,:),"all");
end

%results plot
subplot(2,2,2);
hold on
plot(x, gru(1,:), 'r-o', "MarkerSize", markerSize);
plot(x, gru(2,:), 'r--o', "MarkerSize", markerSize);
plot(x, gru(3,:), 'r:o', "MarkerSize", markerSize);
plot(x, lstm(1,:), 'b-o', "MarkerSize", markerSize);
plot(x, lstm(2,:), 'b--o', "MarkerSize", markerSize);
plot(x, lstm(3,:), 'b:o', "MarkerSize", markerSize);
xlim([150 200]);
hold off
%define legend properties
hleg1 = legend(["GRU streamAcc", "GRU testAcc", "GRU phaseAcc", "LSTM streamAcc", "LSTM testAcc", "LSTM phaseAcc"], ...
    'FontSize',10);
set(hleg1,'position',[0.7 0.15 0.075 0.25]);
xlabel('N of epochs');

%gradientThreshold

for l= 1:L %for each gradientThreshold value
    % compute mean accuracy among other params (nHiddenLayers, maxEpochs):
    x(l) = results{1,1,1,l}.gradientThreshold;
    gru(1,l) = mean(streamAcc(1,:,:,l),"all");
    gru(2,l) = mean(meanTestAcc(1,:,:,l),"all");
    gru(3,l) = mean(meanPhaseAcc(1,:,:,l),"all");
    lstm(1,l) = mean(streamAcc(2,:,:,l),"all");
    lstm(2,l) = mean(meanTestAcc(2,:,:,l),"all");
    lstm(3,l) = mean(meanPhaseAcc(2,:,:,l),"all");
end

%results plot
subplot(2,2,3);
hold on
plot(x, gru(1,:), 'r-o', "MarkerSize", markerSize);
plot(x, gru(2,:), 'r--o', "MarkerSize", markerSize);
plot(x, gru(3,:), 'r:o', "MarkerSize", markerSize);
plot(x, lstm(1,:), 'b-o', "MarkerSize", markerSize);
plot(x, lstm(2,:), 'b--o', "MarkerSize", markerSize);
plot(x, lstm(3,:), 'b:o', "MarkerSize", markerSize);
% legend("GRU streamAcc", "GRU testAcc", "GRU phaseAcc", "lstm streamAcc", "lstm testAcc", "lstm phaseAcc");
xlabel('Gradient threshold');
% pbaspect([4,1,1])
% xlim([15000/100,16000/100])
hold off
sgtitle('Networks accuracy by type, varying:')

exportgraphics(fig,'graphs.png','Resolution',1000);

return

% same as above, with results display (no plot)
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
    disp("lstm");
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
disp("lstm");
for k= 1:K
    disp("          "+ ...
        num2str(results{2,1,k,1}.maxEpochs)+"            "+ ...
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
disp("lstm");
for l= 1:L
    disp("          "+ ...
        num2str(results{2,1,1,l}.gradientThreshold)+"            "+ ...
        num2str(mean(streamAcc(2,:,:,l),"all"))+"             "+ ...
        num2str(mean(meanTestAcc(2,:,:,l),"all"))+"           "+ ...
        num2str(mean(meanPhaseAcc(2,:,:,l),"all")) );
end





