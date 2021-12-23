using _221e.Mitch;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace _221e.Mitch_ToyApp_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ConfigurePage : Page
    {
        private MainPage mpRef;

        private Mitch_HW.Accelerometer_FS selectedAxl_FS;
        private Mitch_HW.Gyroscope_FS selectedGyr_FS;

        private Mitch_HW.TOF_FS selectedTOF_1_FS;
        private Mitch_HW.TOF_FS selectedTOF_2_FS;

        public static ObservableCollection<object> collection { get; set; }
        public static DataTable dataTable, tempTable;

        public byte selectedMEMS = 0;
        public byte selectedLINE = 0;

        public ConfigurePage()
        {
            this.InitializeComponent();
            PagesGateway.ConfigPg = this;

            DataContext = mpRef;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // if (PagesGateway.ViewPg != null)
            //     PagesGateway.ViewPg.enableView = false;

            mpRef = (MainPage)e.Parameter;

            foreach (NetworkNode nn in mpRef.BLENetwork)
                LV_NetworkNodes.Items.Add(nn.BLEDevice.Name);
        }

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

        private void CBGyrFS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CBGyrFS.SelectedIndex)
            {
                case 0:
                    selectedGyr_FS = Mitch_HW.Gyroscope_FS.GYR_FS_245_DPS;
                    break;
                case 1:
                    selectedGyr_FS = Mitch_HW.Gyroscope_FS.GYR_FS_500_DPS;
                    break;
                case 2:
                    selectedGyr_FS = Mitch_HW.Gyroscope_FS.GYR_FS_1000_DPS;
                    break;
                case 3:
                    selectedGyr_FS = Mitch_HW.Gyroscope_FS.GYR_FS_2000_DPS;
                    break;
                default:
                    selectedGyr_FS = Mitch_HW.Gyroscope_FS.GYR_FS_NULL;
                    break;
            }
        }

        private void CBAxlFS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CBAxlFS.SelectedIndex)
            {
                case 0:
                    selectedAxl_FS = Mitch_HW.Accelerometer_FS.AXL_FS_2_g;
                    break;
                case 1:
                    selectedAxl_FS = Mitch_HW.Accelerometer_FS.AXL_FS_4_g;
                    break;
                case 2:
                    selectedAxl_FS = Mitch_HW.Accelerometer_FS.AXL_FS_8_g;
                    break;
                case 3:
                    selectedAxl_FS = Mitch_HW.Accelerometer_FS.AXL_FS_16_g;
                    break;
                default:
                    selectedAxl_FS = Mitch_HW.Accelerometer_FS.AXL_FS_NULL;
                    break;
            }
        }

        private async void BtnRestart_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count >= 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                if (RBNormal.IsChecked == true)
                    msg = Mitch_BLE_Utils.Cmd_Restart(Mitch_HW.RestartMode.RESTART_RESET);
                else
                    msg = Mitch_BLE_Utils.Cmd_Restart(Mitch_HW.RestartMode.RESTART_BOOT_LOADER);

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
            }

            // Reset cache content 
            var cacheSize = ((Frame)Parent).CacheSize;
            ((Frame)Parent).CacheSize = 0;
            ((Frame)Parent).CacheSize = cacheSize;
        }

        private async void BtnGetCheckUp_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_Checkup();

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnGetFirmwareVersion_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_FwVersion();

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnGetCRC_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_GetCRC();

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnGetBLEName_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_BLE_Name();

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnSetBLEName_Click(object sender, RoutedEventArgs e)
        {
            if (LblNewBLEName.Text != "" && LblNewBLEName.Text.Length < 15)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];

                string bleName = LblNewBLEName.Text;
                msg = Mitch_BLE_Utils.Cmd_BLE_Name(false, bleName);

                var writer = new DataWriter();
                writer.ByteOrder = ByteOrder.BigEndian;

                writer.WriteBytes(msg);
                var writeSuccessful = await mpRef.WriteBufferToSelectedCharacteristicAsync(writer.DetachBuffer(), mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnTimesync_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_EnterTimeSync();

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnEnterTimesync_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count >= 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_EnterTimeSync(false);

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
            }
        }

        private async void BtnExitTimesync_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count >= 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_ExitTimeSync();

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
            }
        }

        private async void BtnEstimateOffset_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_EnterTimeSync(false);

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
            }
        }

        private async void BtnGetOffset_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_GetClockOffset();

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
            }
        }

        /*
        private async void BtnSetOffset_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_SetClockOffset(mpRef.result_estimate);

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
            }
        }
        */

        private async void BtnGetDateTime_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_GetTime();

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
            }
        }

        private async void BtnGet6DOFFS_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_GetFS6DOFS();

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnSet6DOFFS_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count >= 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_GetFS6DOFS(false, selectedGyr_FS, selectedAxl_FS);
                               
                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
            }
        }

        private void CBTOFFS_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CBTOFFS_1.SelectedIndex)
            {
                case 0:
                    selectedTOF_1_FS = Mitch_HW.TOF_FS.TOF_FS_200mm;
                    break;
                case 1:
                    selectedTOF_1_FS = Mitch_HW.TOF_FS.TOF_FS_400mm;
                    break;
                case 2:
                    selectedTOF_1_FS = Mitch_HW.TOF_FS.TOF_FS_600mm;
                    break;
                default:
                    selectedTOF_1_FS = Mitch_HW.TOF_FS.TOF_FS_NULL;
                    break;
            }
        }

        private void CBTOFFS_2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (CBTOFFS_2.SelectedIndex)
            {
                case 0:
                    selectedTOF_2_FS = Mitch_HW.TOF_FS.TOF_FS_200mm;
                    break;
                case 1:
                    selectedTOF_2_FS = Mitch_HW.TOF_FS.TOF_FS_400mm;
                    break;
                case 2:
                    selectedTOF_2_FS = Mitch_HW.TOF_FS.TOF_FS_600mm;
                    break;
                default:
                    selectedTOF_2_FS = Mitch_HW.TOF_FS.TOF_FS_NULL;
                    break;
            }
        }

        private async void BtnGetFSTOF_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];

                // Check DS1
                msg = Mitch_BLE_Utils.Cmd_GetTOFFS(0x00);
                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);

                // Wait 500ms between the two calls
                await Task.Delay(500);

                // Check DS2
                msg = Mitch_BLE_Utils.Cmd_GetTOFFS(0x01);
                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnSetFSTOF_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count >= 1)
            {
                // Set DS1
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_SetTOFFS(selectedTOF_1_FS, 0x00);

                List<int> selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);

                // Wait 500 ms between calls
                await Task.Delay(1000);

                // Set DS2
                msg = Mitch_BLE_Utils.Cmd_SetTOFFS(selectedTOF_2_FS, 0x01);

                selectedItemIndexes = new List<int>();
                foreach (object o in LV_NetworkNodes.SelectedItems)
                    selectedItemIndexes.Add(LV_NetworkNodes.Items.IndexOf(o));

                foreach (var idx in selectedItemIndexes)
                    await mpRef.SendMessage(msg, mpRef.BLENetwork[idx]);
            }
        }

        private async void BtnGetOffsetTOF_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];

                // Check TOF #1
                msg = Mitch_BLE_Utils.Cmd_GetTOFOffset(0x00);
                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);

                // Wait 500 ms between calls
                await Task.Delay(500);

                // Check TOF #2
                msg = Mitch_BLE_Utils.Cmd_GetTOFOffset(0x01);
                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnSetOffsetTOF_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];

                // Check TOF #1
                msg = Mitch_BLE_Utils.Cmd_SetTOFOffset(Convert.ToUInt16(Lbls1offsetVal.Text),0x00);
                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);

                // Wait 500 ms between calls
                await Task.Delay(1000);

                // Check TOF #2
                msg = Mitch_BLE_Utils.Cmd_SetTOFOffset(Convert.ToUInt16(Lbls2offsetVal.Text), 0x01);
                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        /*
        private async void BtnGetGyrCalib_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                mpRef.bis_counter = 0;

                mpRef.tmpNode = null;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => mpRef.tmpNode = mpRef.BLENetwork[PagesGateway.ConfigPg.LV_NetworkNodes.SelectedIndex]);

                MainPage.CalibrationCoefficients = new ObservableCollection<float>();
                mpRef.lineCounter = 0;

                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_GetGyrCalibration(0x01);

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }
        */
        /*
        private async void BtnSetGyrCalib_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                string[] stringArray = LblCalibrationCoeffs.Text.Split(new char[] { ',', '\t', '\n', '\r' });

                MainPage.CalibrationCoefficients = new ObservableCollection<float>();
                mpRef.lineCounter = 0;

                if (stringArray.Length == 12)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        List<float> coeffsValues = new List<float>();
                        for (int j = 0; j < 4; j++)
                        {
                            float tmpValue = float.Parse(stringArray[j + (4 * i)]);
                            coeffsValues.Add(tmpValue);
                        }

                        byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                        msg = Mitch_BLE_Utils.Cmd_SetGyrCalibration((byte)(i + 1), coeffsValues[0], coeffsValues[1], coeffsValues[2], coeffsValues[3]);

                        await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);

                        await Task.Delay(150);
                    }
                }
            }
        }
        */
        /*
        private async void BtnGetAxlCalib_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                mpRef.bis_counter = 0;

                mpRef.tmpNode = null;
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => mpRef.tmpNode = mpRef.BLENetwork[PagesGateway.ConfigPg.LV_NetworkNodes.SelectedIndex]);

                MainPage.CalibrationCoefficients = new ObservableCollection<float>();
                mpRef.lineCounter = 0;

                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_GetAxlCalibration(0x01);

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }
        */
        /*
        private async void BtnSetAxlCalib_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                string[] stringArray = LblCalibrationCoeffs.Text.Split(new char[] { ',', '\t', '\n', '\r' });

                MainPage.CalibrationCoefficients = new ObservableCollection<float>();
                mpRef.lineCounter = 0;

                if (stringArray.Length == 12)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        List<float> coeffsValues = new List<float>();
                        for (int j = 0; j < 4; j++)
                        {
                            float tmpValue = float.Parse(stringArray[j + (4 * i)]);
                            coeffsValues.Add(tmpValue);
                        }

                        byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                        msg = Mitch_BLE_Utils.Cmd_SetAxlCalibration((byte)(i + 1), coeffsValues[0], coeffsValues[1], coeffsValues[2], coeffsValues[3]);

                        await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);

                        await Task.Delay(150);
                    }
                }
            }
        }
        */

        /*
        private async void BtnGetMagCalib_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                // mpRef.bis_counter = 0;

                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                msg = Mitch_BLE_Utils.Cmd_GetMagCalibration(0x01);

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }
        */

        private async void RdbtnGyr_Checked(object sender, RoutedEventArgs e)
        {
            selectedMEMS = 1;

            if (mpRef != null)
                mpRef.tmpNode = null;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (LV_NetworkNodes.SelectedItems.Count == 1)
                    mpRef.tmpNode = mpRef.BLENetwork[PagesGateway.ConfigPg.LV_NetworkNodes.SelectedIndex];
            });

            MainPage.CalibrationCoefficients = new ObservableCollection<float>();
            // mpRef.lineCounter = 0;
        }

        private async void RdbtnAxl_Checked(object sender, RoutedEventArgs e)
        {
            selectedMEMS = 2;

            if (mpRef != null)
                mpRef.tmpNode = null;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (LV_NetworkNodes.SelectedItems.Count == 1)
                    mpRef.tmpNode = mpRef.BLENetwork[PagesGateway.ConfigPg.LV_NetworkNodes.SelectedIndex];
            });

            MainPage.CalibrationCoefficients = new ObservableCollection<float>();
        }

        private async void RdbtnMag_Checked(object sender, RoutedEventArgs e)
        {
            selectedMEMS = 3;

            if (mpRef != null)
                mpRef.tmpNode = null;

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (LV_NetworkNodes.SelectedItems.Count == 1)
                    mpRef.tmpNode = mpRef.BLENetwork[PagesGateway.ConfigPg.LV_NetworkNodes.SelectedIndex];
            });

            MainPage.CalibrationCoefficients = new ObservableCollection<float>();
        }

        private async void BtnGetXCalib_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                // mpRef.bis_counter = 0;
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                switch (selectedMEMS)
                {
                    case 0:     // NONE
                        break;
                    case 1:     // GYROSCOPE
                        msg = Mitch_BLE_Utils.Cmd_GetGyrCalibration(0x01);
                        break;
                    case 2:     // ACCELEROMETER
                        msg = Mitch_BLE_Utils.Cmd_GetAxlCalibration(0x01);
                        break;
                    case 3:     // MAGNETOMETER
                        msg = Mitch_BLE_Utils.Cmd_GetMagCalibration(0x01);
                        break;
                    default:    // DO NOTHING
                        break;
                }

                selectedLINE = 1;

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnGetYCalib_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                // mpRef.bis_counter = 0;
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                switch (selectedMEMS)
                {
                    case 0:     // NONE
                        break;
                    case 1:     // GYROSCOPE
                        msg = Mitch_BLE_Utils.Cmd_GetGyrCalibration(0x02);
                        break;
                    case 2:     // ACCELEROMETER
                        msg = Mitch_BLE_Utils.Cmd_GetAxlCalibration(0x02);
                        break;
                    case 3:     // MAGNETOMETER
                        msg = Mitch_BLE_Utils.Cmd_GetMagCalibration(0x02);
                        break;
                    default:    // DO NOTHING
                        break;
                }

                selectedLINE = 2;

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnGetZCalib_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                // mpRef.bis_counter = 0;
                byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                switch (selectedMEMS)
                {
                    case 0:     // NONE
                        break;
                    case 1:     // GYROSCOPE
                        msg = Mitch_BLE_Utils.Cmd_GetGyrCalibration(0x03);
                        break;
                    case 2:     // ACCELEROMETER
                        msg = Mitch_BLE_Utils.Cmd_GetAxlCalibration(0x03);
                        break;
                    case 3:     // MAGNETOMETER
                        msg = Mitch_BLE_Utils.Cmd_GetMagCalibration(0x03);
                        break;
                    default:    // DO NOTHING
                        break;
                }

                selectedLINE = 3;

                await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
            }
        }

        private async void BtnSetXCalib_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                string[] stringArray = LblCalibrationCoeffsLine1.Text.Split(new char[] { ',', '\t', '\n', '\r' });

                MainPage.CalibrationCoefficients = new ObservableCollection<float>();
                mpRef.lineCounter = 0;

                if (stringArray.Length == 4)
                {
                    // for (int i = 0; i < 3; i++)
                    // {
                    List<float> coeffsValues = new List<float>();
                    for (int j = 0; j < 4; j++)
                    {
                        float tmpValue = float.Parse(stringArray[j]);
                        coeffsValues.Add(tmpValue);
                    }

                    byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                    switch (selectedMEMS)
                    {
                        case 0:     // NONE
                            break;
                        case 1:     // GYROSCOPE
                            msg = Mitch_BLE_Utils.Cmd_SetGyrCalibration(0x01, coeffsValues[0], coeffsValues[1], coeffsValues[2], coeffsValues[3]);
                            break;
                        case 2:     // ACCELEROMETER
                            msg = Mitch_BLE_Utils.Cmd_SetAxlCalibration(0x01, coeffsValues[0], coeffsValues[1], coeffsValues[2], coeffsValues[3]);
                            break;
                        case 3:     // MAGNETOMETER
                            msg = Mitch_BLE_Utils.Cmd_SetMagCalibration(0x01, coeffsValues[0], coeffsValues[1], coeffsValues[2], coeffsValues[3]);
                            break;
                        default:    // DO NOTHING
                            break;
                    }

                    selectedLINE = 1;

                    await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
                    // }
                }
            }
        }

        private async void BtnSetYCalib_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                string[] stringArray = LblCalibrationCoeffsLine2.Text.Split(new char[] { ',', '\t', '\n', '\r' });

                MainPage.CalibrationCoefficients = new ObservableCollection<float>();
                mpRef.lineCounter = 0;

                if (stringArray.Length == 4)
                {
                    // for (int i = 0; i < 3; i++)
                    // {
                    List<float> coeffsValues = new List<float>();
                    for (int j = 0; j < 4; j++)
                    {
                        float tmpValue = float.Parse(stringArray[j]);
                        coeffsValues.Add(tmpValue);
                    }

                    byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                    switch (selectedMEMS)
                    {
                        case 0:     // NONE
                            break;
                        case 1:     // GYROSCOPE
                            msg = Mitch_BLE_Utils.Cmd_SetGyrCalibration(0x02, coeffsValues[0], coeffsValues[1], coeffsValues[2], coeffsValues[3]);
                            break;
                        case 2:     // ACCELEROMETER
                            msg = Mitch_BLE_Utils.Cmd_SetAxlCalibration(0x02, coeffsValues[0], coeffsValues[1], coeffsValues[2], coeffsValues[3]);
                            break;
                        case 3:     // MAGNETOMETER
                            msg = Mitch_BLE_Utils.Cmd_SetMagCalibration(0x02, coeffsValues[0], coeffsValues[1], coeffsValues[2], coeffsValues[3]);
                            break;
                        default:    // DO NOTHING
                            break;
                    }

                    selectedLINE = 1;

                    await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
                    // }
                }
            }
        }

        private async void BtnSetZCalib_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                string[] stringArray = LblCalibrationCoeffsLine3.Text.Split(new char[] { ',', '\t', '\n', '\r' });

                MainPage.CalibrationCoefficients = new ObservableCollection<float>();
                mpRef.lineCounter = 0;

                if (stringArray.Length == 4)
                {
                    // for (int i = 0; i < 3; i++)
                    // {
                    List<float> coeffsValues = new List<float>();
                    for (int j = 0; j < 4; j++)
                    {
                        float tmpValue = float.Parse(stringArray[j]);
                        coeffsValues.Add(tmpValue);
                    }

                    byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                    switch (selectedMEMS)
                    {
                        case 0:     // NONE
                            break;
                        case 1:     // GYROSCOPE
                            msg = Mitch_BLE_Utils.Cmd_SetGyrCalibration(0x03, coeffsValues[0], coeffsValues[1], coeffsValues[2], coeffsValues[3]);
                            break;
                        case 2:     // ACCELEROMETER
                            msg = Mitch_BLE_Utils.Cmd_SetAxlCalibration(0x03, coeffsValues[0], coeffsValues[1], coeffsValues[2], coeffsValues[3]);
                            break;
                        case 3:     // MAGNETOMETER
                            msg = Mitch_BLE_Utils.Cmd_SetMagCalibration(0x03, coeffsValues[0], coeffsValues[1], coeffsValues[2], coeffsValues[3]);
                            break;
                        default:    // DO NOTHING
                            break;
                    }

                    selectedLINE = 1;

                    await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);
                    // }
                }
            }
        }

        /*
        private async void BtnSetMagCalib_Click(object sender, RoutedEventArgs e)
        {
            if (LV_NetworkNodes.SelectedItems.Count == 1)
            {
                string[] stringArray = LblCalibrationCoeffs.Text.Split(new char[] { ',', '\t', '\n', '\r' });

                MainPage.CalibrationCoefficients = new ObservableCollection<float>();
                mpRef.lineCounter = 0;

                if (stringArray.Length == 12)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        List<float> coeffsValues = new List<float>();
                        for (int j = 0; j < 4; j++)
                        {
                            float tmpValue = float.Parse(stringArray[j + (4 * i)]);
                            coeffsValues.Add(tmpValue);
                        }

                        byte[] msg = new byte[Mitch_HW.COMM_MESSAGE_LEN];
                        msg = Mitch_BLE_Utils.Cmd_SetMagCalibration((byte)(i + 1), coeffsValues[0], coeffsValues[1], coeffsValues[2], coeffsValues[3]);

                        await mpRef.SendMessage(msg, mpRef.BLENetwork[LV_NetworkNodes.SelectedIndex]);

                        await Task.Delay(150);
                    }
                }
            }
        }
        */
    }
}
