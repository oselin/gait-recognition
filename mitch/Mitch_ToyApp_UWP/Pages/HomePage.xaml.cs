using _221e.Mitch;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Uwp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static _221e.Mitch_ToyApp_UWP.MainPage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace _221e.Mitch_ToyApp_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class HomePage : Page
    {
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

        private MainPage mpRef;

        public Mitch_HW.StreamMode SelectedStreamMode { get; set; }
        public Mitch_HW.StreamFrequency SelectedStreamFrequency { get; set; }

        public Mitch_HW.LogMode SelectedLogMode { get; set; }
        public Mitch_HW.LogFrequency SelectedLogFrequency { get; set; }

        public HomePage()
        {
            this.InitializeComponent();
            PagesGateway.HomePg = this;

            NavigationCacheMode = NavigationCacheMode.Enabled;

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
            mpRef = (MainPage)e.Parameter;
            App.CurrentRootPage = this;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        { 
            base.OnNavigatedFrom(e);
        }

        /*
        private void Grid_Holding(object sender, RightTappedRoutedEventArgs e)
        {
            //FrameworkElement senderElement = sender as FrameworkElement;
            //// If you need the clicked element:
            //// Item whichOne = senderElement.DataContext as Item;
            //FlyoutBase flyoutBase = FlyoutBase.GetAttachedFlyout(senderElement);
            //flyoutBase.ShowAt(senderElement);


            ListView listView = (ListView)sender;
            int idx = listView.SelectedIndex;
            if (idx != -1)
            {
                MFIService.Text = "Service UUID: " + mpRef.BLENetwork[idx].SelectedService.Uuid.ToString();
                MFICmdChars.Text = "Cmd Characteristic UUID: " + mpRef.BLENetwork[idx].SelectedCharacteristic_cmd.Uuid.ToString();
                if (mpRef.BLENetwork[idx].CharacteristicCollection.Count == 2)
                    MFIDataChars.Text = "Data Characteristic UUID: " + mpRef.BLENetwork[idx].SelectedCharacteristic_data.Uuid.ToString();
                else
                    MFIDataChars.Text = "Data Characteristic UUID: N.A.";

                myContextMenu.ShowAt(listView, e.GetPosition(listView));
            }
        }
        */

        private void BtnEnumerate_Click(object sender, RoutedEventArgs e)
        {
            mpRef.EnumerateDevices();
        }

        private void BtnPair_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            var selectedDevices = ResultsListView.SelectedItems;
            if (selectedDevices.Count > 0)
            {
                List<BluetoothLEDeviceDisplay> tmp = new List<BluetoothLEDeviceDisplay>();
                foreach (var item in selectedDevices)
                    tmp.Add(item as BluetoothLEDeviceDisplay);

                bool result = await mpRef.ConnectDevice(tmp);
            }
        }

        private void BtnResetAll_Click(object sender, RoutedEventArgs e)
        {
            // TODO: review reset all method

            mpRef.StopBleDeviceWatcher();
            BtnEnumerate.Content = "Start Search";

            mpRef.KnownDevices.Clear();
            mpRef.UnknownDevices.Clear();

            mpRef.BLENetwork.Clear();

            mpRef.ServiceCollection.Clear();
            mpRef.CharacteristicCollection.Clear();

            mpRef.ServiceCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();
            mpRef.CharacteristicCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();

            mpRef.registeredCharacteristic_cmd = new ObservableCollection<GattCharacteristic>();
            mpRef.registeredCharacteristic_data = new ObservableCollection<GattCharacteristic>();

            // Clear all fieds
            LblBatteryCharge.Text = "";
            LblMemoryControl.Text = "";
            LblFileInfoIndex.Text = "";
            LblCurrentFileInfo.Text = "";
        }

        private async void BtnShutdown_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count >= 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_Shutdown();

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);

                // TODO: Reset UI to be added after shutdown
            }
        }

        private async void BtnWrite_Click(object sender, RoutedEventArgs e)
        {
            /*
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                string str = LblWrite.Text;
                string[] splitStr = str.Split(' ');

                int len = splitStr.Length;

                if (str != "" && len > 0 && len <= 20)
                {
                    byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                    for (int i = 0; i < len; i++)
                    {
                        int value = Convert.ToInt32(splitStr[i], 16);
                        msg[i] = Convert.ToByte(value);
                    }

                    await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
                }
            }
            */

            if (LV_NetworkNodes.SelectedItems.Count >= 1)
            {
                string str = LblWrite.Text;
                string[] splitStr = str.Split(' ');

                int len = splitStr.Length;

                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];

                if (str != "" && len > 0 && len <= 20)
                {    
                    for (int i = 0; i < len; i++)
                    {
                        int value = Convert.ToInt32(splitStr[i], 16);
                        msg[i] = Convert.ToByte(value);
                    }
               
                    List<int> selectedItemIndexes = new List<int>();
                    foreach (object o in LV_NetworkNodes.SelectedItems)
                        selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                    foreach (var idx in selectedItemIndexes)
                        await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
                }

                // await Task.Delay(500);

                // msg = Mitch_BLE_Utils.Cmd_GetState();
                // await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnRead_Click(object sender, RoutedEventArgs e)
        {

        }

        /*
        private async void BtnGetState_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_GetState();

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }
        */

        private async void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                if (RBNormal.IsChecked == true)
                    msg = Mitch_BLE_Utils.Cmd_Restart(Mitch_HW.RestartMode.RESTART_RESET);
                else
                    msg = Mitch_BLE_Utils.Cmd_Restart(Mitch_HW.RestartMode.RESTART_BOOT_LOADER);

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnGetBatteryCharge_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_Battery_Charge();

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnGetBatteryVoltage_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_Battery_Voltage();

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnGetTime_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_GetTime();

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnMemoryControl_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_Memory_Control();

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnEraseMemory_Click(object sender, RoutedEventArgs e)
        {
             if (LV_NetworkNodes.SelectedItems.Count >= 1)
             {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_Memory_Control(false);

                /////////////////////////////////////////////////////////////////////////////
                var title = "Memory Erase";
                var content = "Are you sure to erase the memory?";

                var yesCommand = new UICommand("Yes", cmd => { });
                var noCommand = new UICommand("No", cmd => { });
                var cancelCommand = new UICommand("Cancel", cmd => { });
  
                var dialog = new MessageDialog(content, title);
                dialog.Options = MessageDialogOptions.None;
                dialog.Commands.Add(yesCommand);

                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 0;

                if (noCommand != null)
                {
                    dialog.Commands.Add(noCommand);
                    dialog.CancelCommandIndex = (uint)dialog.Commands.Count - 1;
                }

                if (cancelCommand != null)
                {
                    dialog.Commands.Add(cancelCommand);
                    dialog.CancelCommandIndex = (uint)dialog.Commands.Count - 1;
                }

                var command = await dialog.ShowAsync();

                if (command == yesCommand)
                {
                    List<int> selectedItemIndexes = new List<int>();
                    foreach (object o in LV_NetworkNodes.SelectedItems)
                        selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                    foreach (var idx in selectedItemIndexes)
                        await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);

                    title = "Memory Erase";
                    content = "Memory erase process in progress!\nPlease wait until green LED stops blinking.";

                    yesCommand = new UICommand("Ok", cmd => { });
                    cancelCommand = new UICommand("Cancel", cmd => { });

                    dialog = new MessageDialog(content, title);
                    dialog.Options = MessageDialogOptions.None;
                    dialog.Commands.Add(yesCommand);

                    dialog.DefaultCommandIndex = 0;
                    dialog.CancelCommandIndex = 0;

                    if (cancelCommand != null)
                    {
                        dialog.Commands.Add(cancelCommand);
                        dialog.CancelCommandIndex = (uint)dialog.Commands.Count - 1;
                    }

                    command = await dialog.ShowAsync();
                }
                else if (command == noCommand)
                {
                    // handle no command
                }
                else
                {
                    // handle cancel command
                }
         
                /////////////////////////////////////////////////////////////////////////////

             }
        }

        private async void BtnFileInfo_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                if (LblFileInfoIndex.Text != "")
                {
                    int fileId;
                    bool isNumeric = int.TryParse(LblFileInfoIndex.Text, out fileId);
                    if (isNumeric && fileId > 0)
                    {
                        byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                        msg = Mitch_BLE_Utils.Cmd_Memory_FileInfo(fileId);

                        await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
                    }
                }
            }
        }

        private void BtnFileDownload_Click(object sender, RoutedEventArgs e)
        {
            // NOT AVAILABLE - TO BE IMPLEMENTED IN MITCH FW RELEASE
        }

        private void CBLogMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CBLogMode.SelectedIndex)
            {
                case 0:     // < ComboBoxItem Content = "IMU" />
                    SelectedLogMode = Mitch_HW.LogMode.LOG_MODE_IMU;
                    break;
                case 1:     // < ComboBoxItem Content = "IMU + INSOLE" />
                    SelectedLogMode = Mitch_HW.LogMode.LOG_MODE_IMU_INSOLE;
                    break;
                case 2:     // < ComboBoxItem Content = "ALL" />
                    SelectedLogMode = Mitch_HW.LogMode.LOG_MODE_ALL;
                    break;
                case 3:     // < ComboBoxItem Content = "IMU [Timestamp]" />
                    SelectedLogMode = Mitch_HW.LogMode.LOG_MODE_IMU_TIMESTAMP;
                    break;
                case 4:     // < ComboBoxItem Content = "IMU + INSOLE [Timestamp]" />
                    SelectedLogMode = Mitch_HW.LogMode.LOG_MODE_IMU_INSOLE_TIMPESTAMP;
                    break;
                case 5:     // < ComboBoxItem Content = "ALL [Timestamp]" />
                    SelectedLogMode = Mitch_HW.LogMode.LOG_MODE_ALL_TIMESTAMP;
                    break;
                default:
                    // NONE
                    SelectedLogMode = Mitch_HW.LogMode.LOG_MODE_NONE;
                    break;
            }
        }

        private void CBLogFreq_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CBLogFreq.SelectedIndex)
            {
                case 0:     // < ComboBoxItem Content = "25" />
                    SelectedLogFrequency = Mitch_HW.LogFrequency.LOG_FREQ_25HZ;
                    break;
                case 1:     // < ComboBoxItem Content = "50" />
                    SelectedLogFrequency = Mitch_HW.LogFrequency.LOG_FREQ_50HZ;
                    break;
                case 2:     // < ComboBoxItem Content = "100" />
                    SelectedLogFrequency = Mitch_HW.LogFrequency.LOG_FREQ_100HZ;
                    break;
                case 3:     // < ComboBoxItem Content = "200" />
                    SelectedLogFrequency = Mitch_HW.LogFrequency.LOG_FREQ_200HZ;
                    break;
                case 4:     // < ComboBoxItem Content = "500" />
                    SelectedLogFrequency = Mitch_HW.LogFrequency.LOG_FREQ_500HZ;
                    break;
                case 5:     // < ComboBoxItem Content = "1000" />
                    SelectedLogFrequency = Mitch_HW.LogFrequency.LOG_FREQ_1000HZ;
                    break;
                default:
                    // NONE
                    SelectedLogFrequency = Mitch_HW.LogFrequency.LOG_FREQ_200HZ;
                    break;
            }
        }

        private async void BtnStartLog_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count >= 1)
            {
                // Clear all fieds
                LblBatteryCharge.Text = "";
                LblMemoryControl.Text = "";
                LblFileInfoIndex.Text = "";
                LblCurrentFileInfo.Text = "";

                if (SelectedLogMode == Mitch_HW.LogMode.LOG_MODE_NONE || 
                    SelectedLogFrequency == Mitch_HW.LogFrequency.LOG_FREQ_NONE)
                {
                    // Create the message dialog and set its content
                    var messageDialog = new MessageDialog("No log configuration found.");

                    // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                    messageDialog.Commands.Add(new UICommand(
                        "Close",
                        new UICommandInvokedHandler(this.CommandInvokedHandler)));

                    // Set the command that will be invoked by default
                    messageDialog.DefaultCommandIndex = 0;

                    // Set the command to be invoked when escape is pressed
                    messageDialog.CancelCommandIndex = 1;

                    // Show the message dialog
                    await messageDialog.ShowAsync();
                }
                else
                {
                    byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                    msg = Mitch_BLE_Utils.Cmd_StartLog(SelectedLogMode, SelectedLogFrequency);

                    List<int> selectedItemIndexes = new List<int>();
                    foreach (object o in LV_NetworkNodes.SelectedItems)
                        selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                    mpRef.lastSetState = NativeStates.LOG;

                    foreach (var idx in selectedItemIndexes)
                        await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
                }
            }
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            // Display message showing the label of the command that was invoked
            mpRef.rootPage.NotifyUser("", NotifyType.StatusMessage);
        }

        private async void BtnStopLog_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count >= 1)
            {
                // Clear all fieds
                LblBatteryCharge.Text = "";
                LblMemoryControl.Text = "";
                LblFileInfoIndex.Text = "";
                LblCurrentFileInfo.Text = "";

                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_SetState(Mitch_HW.SystemState.SYS_IDLE);

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                mpRef.lastSetState = NativeStates.IDLE;

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
            }
        }

        private void CBStreamMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CBStreamMode.SelectedIndex)
            {
                case 0:     //  < ComboBoxItem Content = "STREAM_MODE_PRESSURE" />
                    SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_PRESSURE;
                    break;
                case 1:     // < ComboBoxItem Content = "STREAM_MODE_6DOF_TOF" />
                    SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_6DOF_TOF;
                    break;
                case 2:     // < ComboBoxItem Content = "STREAM_MODE_TOF" />
                    SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_TOF;
                    break;
                case 3:     // < ComboBoxItem Content = "STREAM_MODE_6DOF" />
                    SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_6DOF;
                    break;
                case 4:     // < ComboBoxItem Content = "STREAM_MODE_9DOF" />
                    SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_9DOF;
                    break;
                // case 5:     // < ComboBoxItem Content = "STREAM_MODE_6DOFs_ORIENTATION" />
                //     SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_6DOFs_ORIENTATION;
                //     break;
                // case 5:     // < ComboBoxItem Content = "STREAM_MODE_ORIENTATION" />
                //     SelectedStreamMode = Mitch_HW.StreamMode.STREAM_MODE_ORIENTATION;
                //     break;
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
                case 3:     // < ComboBoxItem Content = "50" />
                    SelectedStreamFrequency = Mitch_HW.StreamFrequency.STREAM_FREQ_50Hz;
                    break;
                default:
                    // NONE
                    SelectedStreamFrequency = Mitch_HW.StreamFrequency.STREAM_FREQ_NONE;
                    break;
            }
        }

        private async void BtnStartStream_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedStreamMode == Mitch_HW.StreamMode.STREAM_MODE_NONE ||
                       SelectedStreamFrequency == Mitch_HW.StreamFrequency.STREAM_FREQ_NONE)
            {
                mpRef.lastSetState = NativeStates.IDLE;

                // Create the message dialog and set its content
                var messageDialog = new MessageDialog("No streaming configuration found.");

                // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                // messageDialog.Commands.Add(new UICommand(
                //     "Try again",
                //     new UICommandInvokedHandler(this.CommandInvokedHandler)));
                messageDialog.Commands.Add(new UICommand(
                    "Close",
                    new UICommandInvokedHandler(this.CommandInvokedHandler)));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 0;

                // Set the command to be invoked when escape is pressed
                messageDialog.CancelCommandIndex = 1;

                // Show the message dialog
                await messageDialog.ShowAsync();
            }
            else 
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];

                // Hard-coded 5 Hz acquisition frequency in streaming mode 
                // TODO: enable again the combobox with multiple frequencies
                // SelectedStreamFrequency = Mitch_HW.StreamFrequency.STREAM_FREQ_5Hz;

                msg = Mitch_BLE_Utils.Cmd_StartStreaming(SelectedStreamMode, SelectedStreamFrequency);

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                mpRef.lastSetState = NativeStates.STREAM;

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
            }
            /*
            if (SelectedStreamMode == Mitch_HW.StreamMode.STREAM_MODE_6DOF || SelectedStreamMode == Mitch_HW.StreamMode.STREAM_MODE_6DOF_TOF ||
                SelectedStreamMode == Mitch_HW.StreamMode.STREAM_MODE_PRESSURE || SelectedStreamMode == Mitch_HW.StreamMode.STREAM_MODE_TOF)
            {
                // 2 nodes can perform streaming together
                if (LV_NetworkNodes.SelectedItems.Count >= 1 && LV_NetworkNodes.SelectedItems.Count <= 2)
                {
                    byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];

                    // Hard-coded 5 Hz acquisition frequency in streaming mode 
                    // TODO: enable again the combobox with multiple frequencies
                    SelectedStreamFrequency = Mitch_HW.StreamFrequency.STREAM_FREQ_5Hz;

                    msg = Mitch_BLE_Utils.Cmd_StartStreaming(SelectedStreamMode, SelectedStreamFrequency);

                    List<int> selectedItemIndexes = new List<int>();
                    foreach (object o in LV_NetworkNodes.SelectedItems)
                        selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                    mpRef.lastSetState = NativeStates.STREAM;

                    foreach (var idx in selectedItemIndexes)
                        await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
                }
            }
            else if (SelectedStreamMode == Mitch_HW.StreamMode.STREAM_MODE_9DOF)
            {
                // 9DOFs must be executed only one note at a time
                if (LV_NetworkNodes.SelectedItems.Count == 1)
                {
                    byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                    msg = Mitch_BLE_Utils.Cmd_StartStreaming(SelectedStreamMode, SelectedStreamFrequency);

                    List<int> selectedItemIndexes = new List<int>();
                    foreach (object o in LV_NetworkNodes.SelectedItems)
                        selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                    mpRef.lastSetState = NativeStates.STREAM;

                    foreach (var idx in selectedItemIndexes)
                        await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
                }
            }
            else
            {
                if (SelectedStreamMode == Mitch_HW.StreamMode.STREAM_MODE_NONE ||
                       SelectedStreamFrequency == Mitch_HW.StreamFrequency.STREAM_FREQ_NONE)
                {
                    mpRef.lastSetState = NativeStates.IDLE;

                    // Create the message dialog and set its content
                    var messageDialog = new MessageDialog("No streaming configuration found.");

                    // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                    // messageDialog.Commands.Add(new UICommand(
                    //     "Try again",
                    //     new UICommandInvokedHandler(this.CommandInvokedHandler)));
                    messageDialog.Commands.Add(new UICommand(
                        "Close",
                        new UICommandInvokedHandler(this.CommandInvokedHandler)));

                    // Set the command that will be invoked by default
                    messageDialog.DefaultCommandIndex = 0;

                    // Set the command to be invoked when escape is pressed
                    messageDialog.CancelCommandIndex = 1;

                    // Show the message dialog
                    await messageDialog.ShowAsync();
                }
            }
            */
        }

        private async void BtnStopStream_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count >= 1 && LV_NetworkNodes.SelectedItems.Count <= 2)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_SetState(Mitch_HW.SystemState.SYS_IDLE);

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                mpRef.lastSetState = NativeStates.IDLE;

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
            }
        }
    }
}
