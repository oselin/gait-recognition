close all;
clear all;
clc;

LEN = 20000;
%% Definition of the algebraic function
syms f(x)
f(x) = sin(sin(15*x)*sin(4*x)^3); 

%% Create a vector of points
myvector = zeros(1,LEN);
for i=1:LEN 
    myvector(i) = f(i/100);
end
myvector = myvector  + 0.2*randn(1,LEN);
%% Plot the algebraic function
figure(1)
fplot(f(x),[0,pi]);
hold on
fplot(0.5, [0, pi])
fplot(-0.5, [0, pi]);
hold off

%% Plot the discrete data
figure(2)
plot(0.01:0.01:LEN/100, myvector);
hold on
plot(0.01:0.01:LEN/100, 0.5*ones(1,LEN), ...
     0.01:0.01:LEN/100, -0.5*ones(1,LEN));
hold off;

%% Find the MAX | MIN elements
[MM, II] = findpeaks(myvector,'MinPeakHeight',0.5);
[mm, ii] = findpeaks(-myvector,'MinPeakHeight',0.5);

%% Find the max | min elements
[M, I] = findpeaks(myvector);
[m, i] = findpeaks(-myvector);

%% Plot the peaks
figure(3)
plot(0.01:0.01:LEN/100, myvector);
hold on
plot(0.01:0.01:LEN/100, 0.5*ones(1,LEN), ...
     0.01:0.01:LEN/100, -0.5*ones(1,LEN));
plot(i/100, -m, 'o', "Color","blue");
plot(ii/100, -mm, 'o', "Color",'green')
plot(II/100,MM,'o', 'Color','red');
hold off;