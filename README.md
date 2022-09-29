# GAIT RECOGNITION BASING ON IMU DATA

The aim of this project is to achieve gait phases recognition using one IMU sensor placed on the ankle of the esaminated subject using Machine Learning algorithms.

## Files brief description

- [Manual](/Manual.pdf): contains a detailed description of all scripts and functions. Please referred to it when running the code.
- [Report](/Report.pdf): reports all methods, theoretical bases and implementation principles used to develop the projects. 
- [resultAnalysis](/resultsAnalysis.m): script used to analyse the models trained by the script [trainMultipleRNN](/trainMultipleRNN.m).
- [RNN](/RNN.m): trains a single Recurrent Neural Network to perform gait recognition based on data collected from an IMU placed on the ankle of the analysed person.
- [trainMultipleRNN](/trainMultipleRNN.m): trains multiple RNN with different parameters and saves the models in [results](/output/results.mat).
- [unsupervisedLearning](/unsupervisedLearning.m): applies an unsupervised approach to accomplish the gait recognition task and reports the results. 

## Folders brief description

- [data](/data/): contains all data gathered with the mitch's sensors used to validate the offline labeling method and the acquisitions obtained only with the IMU sensor used to train the models.
- [include](/include/): contains several scripts used in the matlab main scripts, including the [library](/include/LPMScommunicationBT/) to manage the bluethoot communication with the IMU sensor.
- [mitch](/mitch/): contains software used for the calibration and data collection of the mitch. It contains also several acquired measurements files used as test and to match the pressure sensors ID with their position on the insole.
- [output](/output/): contains:
    - Several MATLab files where trained networks are stored. In particular, [results](/output/results.mat) contains an aggregation of several models trained with different parameters, used to assert which combination was the most performing.
    - [analysis results](/output/analysis%20results/): contains reports of the analysis of the models saved in [results](/output/results.mat).