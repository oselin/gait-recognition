using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using LiveCharts;
using LiveCharts.Uwp;
using LiveCharts.Defaults;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Core;

using _221e.Mitch;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace _221e.Mitch_ToyApp_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewPage : Page
    {
        private MainPage mpRef;

        public ChartValues<ObservableValue> CV_PressureTime_1 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_2 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_3 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_4 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_5 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_6 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_7 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_8 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_9 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_10 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_11 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_12 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_13 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_14 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_15 { get; set; }
        public ChartValues<ObservableValue> CV_PressureTime_16 { get; set; }

        public SeriesCollection SC_PressureTime { get; set; }

        public ChartValues<ObservableValue> CV_Gyroscope_X { get; set; }
        public ChartValues<ObservableValue> CV_Gyroscope_Y { get; set; }
        public ChartValues<ObservableValue> CV_Gyroscope_Z { get; set; }

        public ChartValues<ObservableValue> CV_Accelerometer_X { get; set; }
        public ChartValues<ObservableValue> CV_Accelerometer_Y { get; set; }
        public ChartValues<ObservableValue> CV_Accelerometer_Z { get; set; }

        public ChartValues<ObservableValue> CV_Magnetometer_X { get; set; }
        public ChartValues<ObservableValue> CV_Magnetometer_Y { get; set; }
        public ChartValues<ObservableValue> CV_Magnetometer_Z { get; set; }

        public ChartValues<ObservableValue> CV_TimeOfFlight_Left { get; set; }
        public ChartValues<ObservableValue> CV_TimeOfFlight_Right { get; set; }

        public LineSeries LS_Gyroscope_X { get; set; }
        public LineSeries LS_Gyroscope_Y { get; set; }
        public LineSeries LS_Gyroscope_Z { get; set; }

        public LineSeries LS_Accelerometer_X { get; set; }
        public LineSeries LS_Accelerometer_Y { get; set; }
        public LineSeries LS_Accelerometer_Z { get; set; }

        public LineSeries LS_Magnetometer_X { get; set; }
        public LineSeries LS_Magnetometer_Y { get; set; }
        public LineSeries LS_Magnetometer_Z { get; set; }

        public LineSeries LS_TimeOfFlight_Left { get; set; }
        public LineSeries LS_TimeOfFlight_Right { get; set; }

        public SeriesCollection SC_Gyroscope { get; set; }
        public SeriesCollection SC_Accelerometer { get; set; }
        public SeriesCollection SC_Magnetometer { get; set; }

        public SeriesCollection SC_TimeOfFlight_Left { get; set; }
        public SeriesCollection SC_TimeOfFlight_Right { get; set; }

        public ChartValues<HeatPoint> Values { get; set; }

        public bool enableView = false;

        public ViewPage()
        {
            this.InitializeComponent();
            PagesGateway.ViewPg = this;

            // DataContext = mpRef;

            
            CV_Gyroscope_X = new ChartValues<ObservableValue>();
            CV_Gyroscope_Y = new ChartValues<ObservableValue>();
            CV_Gyroscope_Z = new ChartValues<ObservableValue>();

            CV_Accelerometer_X = new ChartValues<ObservableValue>();
            CV_Accelerometer_Y = new ChartValues<ObservableValue>();
            CV_Accelerometer_Z = new ChartValues<ObservableValue>();

            CV_Magnetometer_X = new ChartValues<ObservableValue>();
            CV_Magnetometer_Y = new ChartValues<ObservableValue>();
            CV_Magnetometer_Z = new ChartValues<ObservableValue>();

            CV_TimeOfFlight_Left = new ChartValues<ObservableValue>();
            CV_TimeOfFlight_Right = new ChartValues<ObservableValue>();

            LS_Gyroscope_X = new LineSeries
            {
                Values = CV_Gyroscope_X,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            LS_Gyroscope_Y = new LineSeries
            {
                Values = CV_Gyroscope_Y,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            LS_Gyroscope_Z = new LineSeries
            {
                Values = CV_Gyroscope_Z,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            LS_Accelerometer_X = new LineSeries
            {
                Values = CV_Accelerometer_X,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            LS_Accelerometer_Y = new LineSeries
            {
                Values = CV_Accelerometer_Y,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            LS_Accelerometer_Z = new LineSeries
            {
                Values = CV_Accelerometer_Z,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            LS_Magnetometer_X = new LineSeries
            {
                Values = CV_Magnetometer_X,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            LS_Magnetometer_Y = new LineSeries
            {
                Values = CV_Magnetometer_Y,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            LS_Magnetometer_Z = new LineSeries
            {
                Values = CV_Magnetometer_Z,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            LS_TimeOfFlight_Left = new LineSeries
            {
                Values = CV_TimeOfFlight_Left,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            LS_TimeOfFlight_Right = new LineSeries
            {
                Values = CV_TimeOfFlight_Right,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            SC_Gyroscope = new SeriesCollection { LS_Gyroscope_X, LS_Gyroscope_Y, LS_Gyroscope_Z  };
            SC_Accelerometer = new SeriesCollection { LS_Accelerometer_X, LS_Accelerometer_Y, LS_Accelerometer_Z };
            SC_Magnetometer = new SeriesCollection { LS_Magnetometer_X, LS_Magnetometer_Y, LS_Magnetometer_Z };

            SC_TimeOfFlight_Left = new SeriesCollection { LS_TimeOfFlight_Left };
            SC_TimeOfFlight_Right = new SeriesCollection { LS_TimeOfFlight_Right };

            CV_PressureTime_1 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_1 = new LineSeries
            {
                Title = "5",
                Values = CV_PressureTime_1,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_2 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_2 = new LineSeries
            {
                Title = "7",
                Values = CV_PressureTime_2,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_3 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_3 = new LineSeries
            {
                Title = "3",
                Values = CV_PressureTime_3,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_4 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_4 = new LineSeries
            {
                Title = "6",
                Values = CV_PressureTime_4,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_5 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_5 = new LineSeries
            {
                Title = "4",
                Values = CV_PressureTime_5,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_6 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_6 = new LineSeries
            {
                Title = "0",
                Values = CV_PressureTime_6,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_7 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_7 = new LineSeries
            {
                Title = "1",
                Values = CV_PressureTime_7,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_8 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_8 = new LineSeries
            {
                Title = "2",
                Values = CV_PressureTime_8,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_9 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_9 = new LineSeries
            {
                Title = "13",
                Values = CV_PressureTime_9,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_10 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_10 = new LineSeries
            {
                Title = "14",
                Values = CV_PressureTime_10,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_11 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_11 = new LineSeries
            {
                Title = "15",
                Values = CV_PressureTime_11,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_12 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_12 = new LineSeries
            {
                Title = "11",
                Values = CV_PressureTime_12,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_13 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_13 = new LineSeries
            {
                Title = "12",
                Values = CV_PressureTime_13,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_14 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_14 = new LineSeries
            {
                Title = "10",
                Values = CV_PressureTime_14,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_15 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_15 = new LineSeries
            {
                Title = "9",
                Values = CV_PressureTime_15,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            CV_PressureTime_16 = new ChartValues<ObservableValue>() { new ObservableValue(10) };
            var LS_PressureTime_16 = new LineSeries
            {
                Title = "8",
                Values = CV_PressureTime_16,
                StrokeThickness = 1,
                Fill = new SolidColorBrush(),
                PointGeometrySize = 0,
                DataLabels = false
            };

            SC_PressureTime = new SeriesCollection { LS_PressureTime_1, LS_PressureTime_2, LS_PressureTime_3, LS_PressureTime_4,
                                          LS_PressureTime_5, LS_PressureTime_6, LS_PressureTime_7, LS_PressureTime_8,
                                          LS_PressureTime_9, LS_PressureTime_10, LS_PressureTime_11, LS_PressureTime_12,
                                          LS_PressureTime_13, LS_PressureTime_14, LS_PressureTime_15, LS_PressureTime_16 };

            DataContext = this;
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // enableView = true;
            mpRef = (MainPage)e.Parameter;

            foreach (NetworkNode nn in mpRef.BLENetwork)
                LV_NetworkNodes_View.Items.Add(nn.BLEDevice.Name);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            BtnStopStream_Click(this, null);
        }

        public SeriesCollection SeriesCollection { get; set; }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            // SVMenu.IsPaneOpen = !SVMenu.IsPaneOpen;
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            // SVMenu.IsPaneOpen = true;
            // FramePageContainer.Navigate(typeof(MainPage));
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        public string[] DataToBePlotted { get; set; }

        public Mitch_Data CurrentData { get; set; }

        private List<float> axl_X = new List<float>();
        private List<float> axl_Y = new List<float>();
        private List<float> axl_Z = new List<float>();

        private List<float> gyr_X = new List<float>();
        private List<float> gyr_Y = new List<float>();
        private List<float> gyr_Z = new List<float>();

        private List<float> mag_X = new List<float>();
        private List<float> mag_Y = new List<float>();
        private List<float> mag_Z = new List<float>();

        private List<float> tof_1 = new List<float>();
        private List<float> tof_2 = new List<float>();

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();
                if (items.Count > 0)
                {
                    var storageFile = items[0] as StorageFile;

                    // Read the overall file content
                    string text = await Windows.Storage.FileIO.ReadTextAsync(storageFile);
                    // Split file content into rows
                    string[] lines = text.Split('\n');

                    int percentageLoading = 0;
                    // Iterate towards the loaded file content
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        // pbLoading.Value = 0;
                        // pbLoading.IsIndeterminate = true;
                        
                        // pbLoading.Value = 0;
                        int len = lines.Count();
                        for (int i = 11; i < len - 1; i++)
                        {
                            string[] channels = lines[i].Split('\t');

                            axl_X.Add(float.Parse(channels[1]));
                            axl_Y.Add(float.Parse(channels[2]));
                            axl_Z.Add(float.Parse(channels[3]));

                            gyr_X.Add(float.Parse(channels[4]));
                            gyr_Y.Add(float.Parse(channels[5]));
                            gyr_Z.Add(float.Parse(channels[6]));

                            mag_X.Add(float.Parse(channels[7]));
                            mag_Y.Add(float.Parse(channels[8]));
                            mag_Z.Add(float.Parse(channels[9]));

                            tof_1.Add(float.Parse(channels[10]));
                            tof_2.Add(float.Parse(channels[11]));

                            /*
                            SC_Accelerometer[0].Values.Add(new ObservableValue(float.Parse(channels[1])));
                            SC_Accelerometer[1].Values.Add(new ObservableValue(float.Parse(channels[2])));
                            SC_Accelerometer[2].Values.Add(new ObservableValue(float.Parse(channels[3])));

                            SC_Gyroscope[0].Values.Add(new ObservableValue(float.Parse(channels[4])));
                            SC_Gyroscope[1].Values.Add(new ObservableValue(float.Parse(channels[5])));
                            SC_Gyroscope[2].Values.Add(new ObservableValue(float.Parse(channels[6])));

                            SC_Magnetometer[0].Values.Add(new ObservableValue(float.Parse(channels[7])));
                            SC_Magnetometer[1].Values.Add(new ObservableValue(float.Parse(channels[8])));
                            SC_Magnetometer[2].Values.Add(new ObservableValue(float.Parse(channels[9])));

                            SC_TimeOfFlight_Left[0].Values.Add(new ObservableValue(float.Parse(channels[10])));
                            SC_TimeOfFlight_Right[0].Values.Add(new ObservableValue(float.Parse(channels[11])));
                            */

                            percentageLoading = (int)(((float)i / len) * 100);
                            // pbLoading.Value = (int)(((float)i / len) * 100);
                        }

                        SC_Accelerometer[0].Values = new ChartValues<float>(axl_X);
                        SC_Accelerometer[1].Values = new ChartValues<float>(axl_Y);
                        SC_Accelerometer[2].Values = new ChartValues<float>(axl_Z);

                        SC_Gyroscope[0].Values = new ChartValues<float>(gyr_X);
                        SC_Gyroscope[1].Values = new ChartValues<float>(gyr_Y);
                        SC_Gyroscope[2].Values = new ChartValues<float>(gyr_Z);

                        SC_Magnetometer[0].Values = new ChartValues<float>(mag_X);
                        SC_Magnetometer[1].Values = new ChartValues<float>(mag_Y);
                        SC_Magnetometer[2].Values = new ChartValues<float>(mag_Z);

                        SC_TimeOfFlight_Left[0].Values = new ChartValues<float>(tof_1);
                        SC_TimeOfFlight_Right[0].Values = new ChartValues<float>(tof_2);

                        // if (percentageLoading >= 99)
                            // pbLoading.Value = 0;
                        // pbLoading.IsIndeterminate = false;

                    });
                }
            }
        }

        public string UIVersion { get; set; }
        public string DeviceInfo { get; set; }
        public string DeviceId { get; set; }
        public string FirmwareVersion { get; set; }
        public string HardwareVersion { get; set; }
        public string AcquisitionInfo { get; set; }
        public string AcquisitionMode { get; set; }
        public string AcquisitionFreq { get; set; }
        public string AcquisitionConf { get; set; }

        private string[] ImportTextFile(StorageFile storage)
        {
            string[] lines = File.ReadAllLines(storage.Path);

            if (lines[0].Contains("General Information:"))
            {
                if (lines[1].Contains("UI version:"))
                    UIVersion = lines[1].Substring(11);

                // if (lines[2].Contains("Device information:"))
                //     DeviceInfo = "N.A."; 

                if (lines[3].Contains("Device Id:"))
                    DeviceId = "N.A.";

                if (lines[4].Contains("Firmware Version:"))
                    FirmwareVersion = lines[4].Substring(17);

                if (lines[5].Contains("Hardware Version:"))
                    FirmwareVersion = lines[5].Substring(17);

                // if (lines[6].Contains("Acquisition Information:"))
                //     AcquisitionInfo = "N.A.";

                if (lines[7].Contains("Mode:"))
                    AcquisitionMode = lines[7].Substring(5);

                if (lines[8].Contains("Sampling frequency:"))
                    AcquisitionFreq = lines[8].Substring(19);

                if (lines[0].Contains("Axl FS:") && lines[0].Contains("Gyro FS:"))
                { }
                    // 16 g 2000 dps
            }
            return lines;
        }

        public Mitch_HW.StreamMode SelectedStreamMode { get; set; }
        public Mitch_HW.StreamFrequency SelectedStreamFrequency { get; set; }

        private void CBStreamMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CBStreamMode.SelectedIndex)
            {
                case 0:     //  < ComboBoxItem Content = "6DOF" />
                    SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_6DOF;
                    break;
                case 1:     // < ComboBoxItem Content = "9DOF" />
                    SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_9DOF;
                    break;
                case 2:     // < ComboBoxItem Content = "TOF" />
                    SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_TOF;
                    break;
                case 3:     // < ComboBoxItem Content = "PRESSURE" />
                    SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_PRESSURE;
                    break;
                case 4:     // < ComboBoxItem Content = "6DOF + TOF" />
                    SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_6DOF_TOF;
                    break;
                default:
                    // NONE
                    SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_NONE;
                    break;
            }
        }

        private void CBStreamFreq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CBStreamFreq.SelectedIndex)
            {
                case 0:     // < ComboBoxItem Content = "5" />
                    SelectedStreamFrequency = Mitch_HW.StreamFrequency.STREAM_FREQ_5Hz;
                    break;
                case 1:     // < ComboBoxItem Content = "10" />
                    SelectedStreamFrequency = Mitch_HW.StreamFrequency.STREAM_FREQ_10Hz;
                    break;
                case 2:     // < ComboBoxItem Content = "25" />
                    SelectedStreamFrequency = Mitch_HW.StreamFrequency.STREAM_FREQ_25Hz;
                    break;
                default:
                    // NONE
                    SelectedStreamFrequency = Mitch_HW.StreamFrequency.STREAM_FREQ_NONE;
                    break;
            }
        }

        public bool viewPanelEnabled = false;

        private async void BtnStartStream_Click(object sender, RoutedEventArgs e)
        {
            viewPanelEnabled = true;
            if (SelectedStreamMode == Mitch_HW.StreamMode.STREAM_MODE_6DOF || SelectedStreamMode == Mitch_HW.StreamMode.STREAM_MODE_6DOF_TOF ||
                SelectedStreamMode == Mitch_HW.StreamMode.STREAM_MODE_PRESSURE || SelectedStreamMode == Mitch_HW.StreamMode.STREAM_MODE_TOF)
            {
                // 2 nodes can perform streaming together
                if (LV_NetworkNodes_View.SelectedItems.Count >= 1 && LV_NetworkNodes_View.SelectedItems.Count <= 2)
                {
                    byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];

                    // Hard-coded 5 Hz acquisition frequency in streaming mode 
                    // TODO: enable again the combobox with multiple frequencies
                    SelectedStreamFrequency = Mitch_HW.StreamFrequency.STREAM_FREQ_5Hz;

                    msg = Mitch_BLE_Utils.Cmd_StartStreaming(SelectedStreamMode, SelectedStreamFrequency);

                    List<int> selectedItemIndexes = new List<int>();
                    foreach (object o in LV_NetworkNodes_View.SelectedItems)
                        selectedItemIndexes.Add(LV_NetworkNodes_View.Items.IndexOf(o));

                    foreach (var idx in selectedItemIndexes)
                        await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
                }
            }
            else if (SelectedStreamMode == Mitch_HW.StreamMode.STREAM_MODE_9DOF)
            {
                // 9DOFs must be executed only one note at a time
                if (LV_NetworkNodes_View.SelectedItems.Count == 1)
                {
                    byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                    msg = Mitch_BLE_Utils.Cmd_StartStreaming(SelectedStreamMode, SelectedStreamFrequency);

                    List<int> selectedItemIndexes = new List<int>();
                    foreach (object o in LV_NetworkNodes_View.SelectedItems)
                        selectedItemIndexes.Add(LV_NetworkNodes_View.Items.IndexOf(o));

                    foreach (var idx in selectedItemIndexes)
                        await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
                }
            }
        }

        private async void BtnStopStream_Click(object sender, RoutedEventArgs e)
        {
            viewPanelEnabled = false;

            if (LV_NetworkNodes_View.SelectedItems.Count >= 1 && LV_NetworkNodes_View.SelectedItems.Count <= 2)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_SetState(Mitch_HW.SystemState.SYS_IDLE);

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes_View.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes_View.Items.IndexOf(o));

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);

                /*
                // Clear workspace
                LblCurrentStream.Text = "";

                PagesGateway.ViewPg.CV_PressureTime_1 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_2 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_3 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_4 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_5 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_6 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_7 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_8 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_9 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_10 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_11 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_12 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_13 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_14 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_15 = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_PressureTime_16 = new ChartValues<ObservableValue>();

                PagesGateway.ViewPg.CV_Gyroscope_X = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_Gyroscope_Y = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_Gyroscope_Z = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_Accelerometer_X = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_Accelerometer_Y = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_Accelerometer_Z = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_Magnetometer_X = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_Magnetometer_Y = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_Magnetometer_Z = new ChartValues<ObservableValue>();

                PagesGateway.ViewPg.CV_TimeOfFlight_Left = new ChartValues<ObservableValue>();
                PagesGateway.ViewPg.CV_TimeOfFlight_Right = new ChartValues<ObservableValue>();
                */
            }
        }
    }
}
