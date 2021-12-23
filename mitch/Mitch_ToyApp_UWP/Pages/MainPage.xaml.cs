using _221e.Mitch;
using LiveCharts.Defaults;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace _221e.Mitch_ToyApp_UWP
{
    public class NetworkNode : INotifyPropertyChanged
    {
        public NetworkNode(ushort id, BluetoothLEDevice dev, ObservableCollection<BluetoothLEAttributeDisplay> serv, ObservableCollection<BluetoothLEAttributeDisplay> chars)
        {
            DeviceID = id;
            BLEDevice = dev;
            ServiceCollection = serv;
            CharacteristicCollection = chars;
        }

        public ushort DeviceID { get; set; }

        public BluetoothLEDevice BLEDevice { get; set; }

        public ObservableCollection<BluetoothLEAttributeDisplay> ServiceCollection { get; set; }
        public ObservableCollection<BluetoothLEAttributeDisplay> CharacteristicCollection { get; set; }

        public GattDeviceService SelectedService { get; set; }
        public GattCharacteristic SelectedCharacteristic_cmd { get; set; }
        public GattCharacteristic SelectedCharacteristic_data { get; set; }

        public static ObservableCollection<GattCharacteristic> SelectedCharacteristicNetwork_cmd { get; set; }
        public static ObservableCollection<GattCharacteristic> SelectedCharacteristicNetwork_data { get; set; }

        public bool SubscribedForNotifications_cmd { get; set; }
        public bool SubscribedForNotifications_data { get; set; }

        private string deviceName;
        public string DeviceName
        {
            get { return deviceName; }
            set
            {
                if (value != DeviceName)
                {
                    deviceName = value;
                    OnPropertyChanged("DeviceName");
                }
            }
        }

        // private Mitch_HW.SystemState deviceStatus;
        private string deviceStatus;
        // public Mitch_HW.SystemState DeviceStatus
        public string DeviceStatus
        {
            get { return deviceStatus; }
            set
            {
                if (value != DeviceStatus)
                {
                    deviceStatus = value;
                    OnPropertyChanged("DeviceStatus");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage rootPage;

        public NetworkNode tmpNode = null;

        // Device enumeration variables
        private DeviceWatcher deviceWatcher;

        public ObservableCollection<BluetoothLEDeviceDisplay> KnownDevices = new ObservableCollection<BluetoothLEDeviceDisplay>();
        public List<DeviceInformation> UnknownDevices = new List<DeviceInformation>();

        // private string SelectedBleDeviceId;
        // private string SelectedBleDeviceName;

        // Device pairing variables
        private bool isBusy = false;

        // Services and Characteristics enumeration variables
        public ObservableCollection<BluetoothLEAttributeDisplay> ServiceCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();
        public ObservableCollection<BluetoothLEAttributeDisplay> CharacteristicCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();

        public BluetoothLEDevice bluetoothLeDevice = null;
        public GattCharacteristic selectedCharacteristic_cmd;
        public GattCharacteristic selectedCharacteristic_data;

        // Only one registered characteristic at a time.
        public ObservableCollection<GattCharacteristic> registeredCharacteristic_cmd = new ObservableCollection<GattCharacteristic>();
        public ObservableCollection<GattCharacteristic> registeredCharacteristic_data = new ObservableCollection<GattCharacteristic>();

        private GattPresentationFormat presentationFormat;

        private bool subscribedForNotifications_cmd = false;
        private bool subscribedForNotifications_data = false;

        private bool enableBLEmode = false;

        private ushort deviceID = 0;
        private string deviceState = "";
        // private Mitch_HW.SystemState deviceState = Mitch_HW.SystemState.SYS_NULL;

        public enum NativeStates
        {
            NONE = 0,
            STREAM = 1,
            LOG = 2,
            IDLE = 3
        }

        public bool setStateFlag = false;
        public NativeStates lastSetState = NativeStates.NONE;

        public ObservableCollection<NetworkNode> BLENetwork = new ObservableCollection<NetworkNode>();

        public List<string> AvailableDevices { get; set; }
        private async void UpdateDevicesList()
        {
            string aqs = SerialDevice.GetDeviceSelector();
            var deviceCollection = await DeviceInformation.FindAllAsync(aqs);
            List<string> portNamesList = new List<string>();
            foreach (var item in deviceCollection)
            {
                var serialDevice = await SerialDevice.FromIdAsync(item.Id);
                if (serialDevice != null)
                {
                    var portName = serialDevice.PortName;
                    portNamesList.Add(portName);
                }
            }

            AvailableDevices = new List<string>();
            /*
            DeviceInformationCollection deviceCollection = await DeviceInformation.FindAllAsync();

            AvailableDevices = new List<string>();
            if (deviceCollection.Any())
            {
                foreach(DeviceInformation dev in deviceCollection)
                    AvailableDevices.Add(dev.Name);
            }

            int numOfDevices = AvailableDevices.Count;
            */

            // PagesGateway.HomePg_USB.LblCOMPort.ItemsSource = AvailableDevices;

            // string[] portNames = SerialPort.GetPortNames();
            // AvailableCOMPorts = portNames.ToList<string>();
            // AvailableCOMPorts.Sort();

            // Aggiungo i dispositivi alle comboBox
            // LblCOMPort.DataSource = new BindingSource(portNumbers, null);
            // cmbDev.SelectedIndex = -1;
        }

        DateTime T1;
        DateTime T4;
        int estimate_iterations = 10;
        int current_estimate_iterations = 0;
        public double result_estimate = 0;
        UInt64 reference_epoch = 1554105600;

        private float current_gyr_resolution = 0;
        private float current_axl_resolution = 0;
        private float current_mag_resolution = 0;

        public static ObservableCollection<float> CalibrationCoefficients { get; set; }
        public int lineCounter = 0;

        public MainPage()
        {
            this.InitializeComponent();
            PagesGateway.MainPg = this;
            rootPage = this;
            FramePageContainer.Navigate(typeof(HomePage), rootPage);
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            SVMenu.IsPaneOpen = !SVMenu.IsPaneOpen;
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            SVMenu.IsPaneOpen = true;
            FramePageContainer.Navigate(typeof(HomePage), rootPage);

            if (PagesGateway.ViewPg != null)
            {
                PagesGateway.ViewPg = null;
                if (iterationCounter > 10)
                    iterationCounter = 0;
            }
        }

        private void BtnConfigure_Click(object sender, RoutedEventArgs e)
        {
            SVMenu.IsPaneOpen = true;
            FramePageContainer.Navigate(typeof(ConfigurePage), rootPage);
        }

        private void BtnView_Click(object sender, RoutedEventArgs e)
        {
            SVMenu.IsPaneOpen = true;
            FramePageContainer.Navigate(typeof(ViewPage), rootPage);
        }




        #region Error Codes

        readonly int E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED = unchecked((int)0x80650003);
        readonly int E_BLUETOOTH_ATT_INVALID_PDU = unchecked((int)0x80650004);
        readonly int E_ACCESSDENIED = unchecked((int)0x80070005);
        readonly int E_DEVICE_NOT_AVAILABLE = unchecked((int)0x800710df); // HRESULT_FROM_WIN32(ERROR_DEVICE_NOT_AVAILABLE)

        #endregion


        #region DEVICE ENUMERATION

        // Start / Stop device enumeration
        public void EnumerateDevices()
        {
            if (deviceWatcher == null)
            {
                StartBleDeviceWatcher();
                // BtnEnumerate.Content = "Stop Search BLE Devices";
                // rootPage.NotifyUser($"Device watcher started.", NotifyType.StatusMessage);
            }
            else
            {
                StopBleDeviceWatcher();
                // BtnEnumerate.Content = "Search BLE Devices";
                // rootPage.NotifyUser($"Device watcher stopped.", NotifyType.StatusMessage);
            }
        }

        // Starts a device watcher that looks for all nearby Bluetooth devices (paired or unpaired). 
        // Attaches event handlers to populate the device collection.
        private void StartBleDeviceWatcher()
        {
            // Standard property strings: https://msdn.microsoft.com/en-us/library/windows/desktop/ff521659(v=vs.85).aspx
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable", "System.DeviceInterface.Bluetooth.ServiceGuid" };

            // Search for paired and non-paired devices in a single query
            string aqsAllBluetoothLEDevices = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";

            deviceWatcher = DeviceInformation.CreateWatcher(
                                aqsAllBluetoothLEDevices,
                                requestedProperties,
                                DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher
            deviceWatcher.Added += DeviceWatcher_Added;
            deviceWatcher.Updated += DeviceWatcher_Updated;
            deviceWatcher.Removed += DeviceWatcher_Removed;
            deviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            deviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Clear device collection
            KnownDevices.Clear();

            // Start device watcher
            deviceWatcher.Start();
        }

        // Stops watching for all nearby Bluetooth devices
        public void StopBleDeviceWatcher()
        {
            if (deviceWatcher != null)
            {
                // Unregister the event handlers
                deviceWatcher.Added -= DeviceWatcher_Added;
                deviceWatcher.Updated -= DeviceWatcher_Updated;
                deviceWatcher.Removed -= DeviceWatcher_Removed;
                deviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
                deviceWatcher.Stopped -= DeviceWatcher_Stopped;

                // Stop device watcher
                deviceWatcher.Stop();
                deviceWatcher = null;
            }
        }

        // Manages KNOWN devices display
        private BluetoothLEDeviceDisplay FindBluetoothLEDeviceDisplay(string id)
        {
            foreach (BluetoothLEDeviceDisplay bleDeviceDisplay in KnownDevices)
            {
                if (bleDeviceDisplay.Id == id)
                {
                    return bleDeviceDisplay;
                }
            }
            return null;
        }

        // Manages UN-KNOWN devices display
        private DeviceInformation FindUnknownDevices(string id)
        {
            foreach (DeviceInformation bleDeviceInfo in UnknownDevices)
            {
                if (bleDeviceInfo.Id == id)
                {
                    return bleDeviceInfo;
                }
            }
            return null;
        }

        // Add device to the device list (i.e., observable collection) on the UI thread because the collection is databound to a UI element
        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (this)
                {
                    // Debug.WriteLine(String.Format("Added {0}{1}", deviceInfo.Id, deviceInfo.Name));

                    // If the task runs after the app stopped the deviceWatcher, check for race condition
                    if (sender == deviceWatcher)
                    {
                        // Make sure device isn't already present in the list
                        if (FindBluetoothLEDeviceDisplay(deviceInfo.Id) == null)
                        {
                            if (deviceInfo.Name != string.Empty)
                            {
                                // Display device friendly name, if available
                                KnownDevices.Add(new BluetoothLEDeviceDisplay(deviceInfo));
                            }
                            else
                            {
                                // Add it to a list in case the name gets updated later
                                UnknownDevices.Add(deviceInfo);
                            }
                        }

                    }
                }
            });
        }

        // Updates the device list (i.e., observable collection) on the UI thread because the collection is databound to a UI element
        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (this)
                {
                    // Debug.WriteLine(String.Format("Updated {0}{1}", deviceInfoUpdate.Id, ""));

                    // If the task runs after the app stopped the deviceWatcher, check for race condition
                    if (sender == deviceWatcher)
                    {
                        BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                        if (bleDeviceDisplay != null)
                        {
                            // Device is already being displayed - update UX
                            bleDeviceDisplay.Update(deviceInfoUpdate);
                            return;
                        }

                        DeviceInformation deviceInfo = FindUnknownDevices(deviceInfoUpdate.Id);
                        if (deviceInfo != null)
                        {
                            deviceInfo.Update(deviceInfoUpdate);

                            // If device has been updated with a friendly name it's no longer unknown
                            if (deviceInfo.Name != String.Empty)
                            {
                                KnownDevices.Add(new BluetoothLEDeviceDisplay(deviceInfo));
                                UnknownDevices.Remove(deviceInfo);
                            }
                        }
                    }
                }
            });
        }

        // Removes device from the device list (i.e., observable collection) on the UI thread because the collection is databound to a UI element
        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                lock (this)
                {
                    // Debug.WriteLine(String.Format("Removed {0}{1}", deviceInfoUpdate.Id, ""));

                    // If the task runs after the app stopped the deviceWatcher, check for race condition
                    if (sender == deviceWatcher)
                    {
                        // Find the corresponding DeviceInformation in the collection and remove it
                        BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                        if (bleDeviceDisplay != null)
                        {
                            KnownDevices.Remove(bleDeviceDisplay);
                        }

                        DeviceInformation deviceInfo = FindUnknownDevices(deviceInfoUpdate.Id);
                        if (deviceInfo != null)
                        {
                            UnknownDevices.Remove(deviceInfo);
                        }
                    }
                }
            });
        }

        // Manage "ENUMERATION COMPLETED" event
        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (sender == deviceWatcher)
                {
                    // rootPage.NotifyUser($"{KnownDevices.Count} devices found. Enumeration completed.",
                    //     NotifyType.StatusMessage);
                }
            });
        }

        // Stop device watcher
        private async void DeviceWatcher_Stopped(DeviceWatcher sender, object e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (sender == deviceWatcher)
                {
                    // rootPage.NotifyUser($"No longer watching for devices.",
                    //         sender.Status == DeviceWatcherStatus.Aborted ? NotifyType.ErrorMessage : NotifyType.StatusMessage);
                }
            });
        }

        // Show device list
        private void ResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //// Save the selected device's ID for use in other scenarios.
            //foreach (var sl in ResultsListView.SelectedItems)
            //{
            //    var bleDeviceDisplay = sl as BluetoothLEDeviceDisplay;
            //    if (bleDeviceDisplay != null)
            //    {
            //        rootPage.SelectedBleDeviceId.Add(bleDeviceDisplay.Id);
            //        rootPage.SelectedBleDeviceName.Add(bleDeviceDisplay.Name);
            //    }
            //}
        }

        #endregion

        #region DEVICE PAIRING

        // Pair selected device
        public async void PairButton_Click(List<BluetoothLEDeviceDisplay> selectedDevices)
        {
            // Do not allow a new Pair operation to start if an existing one is in progress
            if (isBusy)
            {
                return;
            }

            isBusy = true;      // LOCK

            // rootPage.NotifyUser("Pairing started. Please wait...", NotifyType.StatusMessage);

            // Iterate over current selected items in case the user changes it while we are pairing
            foreach (var sl in selectedDevices)
            {
                var bleDeviceDisplay = sl as BluetoothLEDeviceDisplay;
                if (bleDeviceDisplay != null)
                {
                    // Pair the currently selected device
                    DevicePairingResult result = await bleDeviceDisplay.DeviceInformation.Pairing.PairAsync();
                }
            }

            isBusy = false;     // UNLOCK
        }

        #endregion

        #region DEVICE CONNECTION

        private async void ResetButton_Click()
        {
            // StopBleDeviceWatcher();
            // BtnEnumerate.Content = "Start Search";

            // BtnEnumerate.Content = "Start Search";
            // BtnPair.IsEnabled = false;
            // BtnConnect.IsEnabled = true;

            // KnownDevices.Clear();
            // UnknownDevices.Clear();

            // BLENetwork.Clear();
            // LV_NetworkNodes.Items.Clear();

            // ServiceCollection.Clear();
            // CharacteristicCollection.Clear();

            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => BtnEnumerate.Content = "Start Search");
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => BtnPair.IsEnabled = false);
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => BtnConnect.IsEnabled = true);

            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => KnownDevices.Clear()); 
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UnknownDevices.Clear());

            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => BLENetwork.Clear());
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => LV_NetworkNodes.Items.Clear());

            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => ServiceCollection.Clear());
            //await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => CharacteristicCollection.Clear());
        }

        // Connect selected device
        public async Task<bool> ConnectDevice(List<BluetoothLEDeviceDisplay> selectedDevices)
        {
            if (!await ClearBluetoothLEDeviceAsync())
                return false;

            try
            {
                // BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
                foreach (var item in selectedDevices)
                {
                    NetworkNode tempNode = null;
                    ServiceCollection = null;
                    CharacteristicCollection = null;

                    var bleDeviceDisplay = item as BluetoothLEDeviceDisplay;
                    if (bleDeviceDisplay != null)
                        bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(bleDeviceDisplay.Id);

                    if (bluetoothLeDevice != null)
                    {
                        // connection_ = new _03b1MuseConnection_BLE();

                        // Note: BluetoothLEDevice.GattServices property will return an empty list for unpaired devices. For all uses we recommend using the GetGattServicesAsync method.
                        // BT_Code: GetGattServicesAsync returns a list of all the supported services of the device (even if it's not paired to the system).
                        // If the services supported by the device are expected to change during BT usage, subscribe to the GattServicesChanged event.
                        GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);

                        if (result.Status == GattCommunicationStatus.Success)
                        {
                            ServiceCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();

                            var services = result.Services;
                            foreach (var service in services)
                                ServiceCollection.Add(new BluetoothLEAttributeDisplay(service));

                            if (ServiceCollection.Count == 3)
                            {
                                // Select the 3rd element of the services list by default
                                CharacteristicCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();
                                int charsCounter = await GetCharacteristics(ServiceCollection[2]);
                                if (charsCounter == 1)
                                {
                                    // Select only command characteristic (BOOTLOADER MODE)
                                    tempNode = new NetworkNode(0, bluetoothLeDevice, ServiceCollection, CharacteristicCollection);

                                    if (tempNode != null)
                                    {
                                        tempNode.DeviceName = bluetoothLeDevice.Name;
                                        tempNode.SelectedService = ServiceCollection[2].service;

                                        // EnableCharacteristicPanels(CharacteristicCollection[0].characteristic.CharacteristicProperties);
                                        tempNode.SelectedCharacteristic_cmd = CharacteristicCollection[0].characteristic;
                                        selectedCharacteristic_cmd = tempNode.SelectedCharacteristic_cmd;
                                        CmdCharacteristicSubscribeButton_Click(tempNode);

                                        // Require device ID
                                        byte[] msg = Mitch_BLE_Utils.Cmd_GetDeviceID();
                                        await SendMessage(msg, tempNode);

                                        await Task.Delay(100);  // FIXME: to be improved!

                                        tempNode.DeviceID = deviceID;

                                        // Require device state
                                        msg = Mitch_BLE_Utils.Cmd_GetState();
                                        await SendMessage(msg, tempNode);

                                        await Task.Delay(100);  // FIXME: to be improved!

                                        tempNode.DeviceStatus = deviceState;
                                    }
                                }
                                else if (charsCounter == 2)
                                {
                                    // Select the first characteristic for command and the second for data (APP / NORMAL MODE)
                                    tempNode = new NetworkNode(0, bluetoothLeDevice, ServiceCollection, CharacteristicCollection);

                                    if (tempNode != null)
                                    {
                                        tempNode.DeviceName = bluetoothLeDevice.Name;
                                        tempNode.SelectedService = ServiceCollection[2].service;

                                        // EnableCharacteristicPanels(CharacteristicCollection[0].characteristic.CharacteristicProperties);
                                        tempNode.SelectedCharacteristic_cmd = CharacteristicCollection[0].characteristic;
                                        selectedCharacteristic_cmd = tempNode.SelectedCharacteristic_cmd;

                                        // EnableCharacteristicPanels(CharacteristicCollection[1].characteristic.CharacteristicProperties);
                                        tempNode.SelectedCharacteristic_data = CharacteristicCollection[1].characteristic;
                                        selectedCharacteristic_data = tempNode.SelectedCharacteristic_data;

                                        CmdCharacteristicSubscribeButton_Click(tempNode);
                                        DataCharacteristicSubscribeButton_Click(tempNode);

                                        // Require device ID
                                        byte[] msg = Mitch_BLE_Utils.Cmd_GetDeviceID();
                                        await SendMessage(msg, tempNode);

                                        await Task.Delay(100);  // FIXME: to be improved!

                                        tempNode.DeviceID = deviceID;
                                        // networkNodes.Add(new Tuple<ushort, bool>(deviceID, false));

                                        // Require device state
                                        msg = Mitch_BLE_Utils.Cmd_GetState();
                                        await SendMessage(msg, tempNode);

                                        await Task.Delay(100);  // FIXME: to be improved!

                                        tempNode.DeviceStatus = deviceState;
                                    }
                                }
                            }
                        }
                    }

                    if (tempNode != null)
                        BLENetwork.Add(tempNode);
                }
                StopBleDeviceWatcher();
                return true;
            }
            catch (Exception) // when (ex.HResult == E_DEVICE_NOT_AVAILABLE)
            {
                return false;
            }
        }

        public enum NotifyType
        {
            StatusMessage,
            ErrorMessage
        };

        public void NotifyUser(string strMessage, NotifyType type)
        {
            // If called from the UI thread, then update immediately.
            // Otherwise, schedule a task on the UI thread to perform the update.
            if (Dispatcher.HasThreadAccess)
            {
                UpdateStatus(strMessage, type);
            }
            else
            {
                var task = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => UpdateStatus(strMessage, type));
            }
        }

        private void UpdateStatus(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    break;
                case NotifyType.ErrorMessage:
                    break;
            }
        }

        //private async void ConnectButton_Click()
        //{
        //    BtnConnect.IsEnabled = false;

        //    if (!await ClearBluetoothLEDeviceAsync())
        //    {
        //        BtnConnect.IsEnabled = false;
        //        return;
        //    }

        //    try
        //    {
        //        // BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
        //        int tempCounter = ResultsListView.SelectedItems.Count;
        //        foreach (var item in ResultsListView.SelectedItems)
        //        {
        //            NetworkNode tempNode = null;
        //            ServiceCollection = null;
        //            CharacteristicCollection = null;

        //            var bleDeviceDisplay = item as BluetoothLEDeviceDisplay;
        //            if (bleDeviceDisplay != null)
        //                bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(bleDeviceDisplay.Id);

        //            if (bluetoothLeDevice != null)
        //            {
        //                // connection_ = new _03b1MuseConnection_BLE();

        //                // Note: BluetoothLEDevice.GattServices property will return an empty list for unpaired devices. For all uses we recommend using the GetGattServicesAsync method.
        //                // BT_Code: GetGattServicesAsync returns a list of all the supported services of the device (even if it's not paired to the system).
        //                // If the services supported by the device are expected to change during BT usage, subscribe to the GattServicesChanged event.
        //                GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);

        //                if (result.Status == GattCommunicationStatus.Success)
        //                {
        //                    ServiceCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();

        //                    var services = result.Services;
        //                    foreach (var service in services)
        //                        ServiceCollection.Add(new BluetoothLEAttributeDisplay(service));

        //                    if (ServiceCollection.Count == 3)
        //                    {
        //                        // Select the 3rd element of the services list by default
        //                        CharacteristicCollection = new ObservableCollection<BluetoothLEAttributeDisplay>();
        //                        int charsCounter = await GetCharacteristics(ServiceCollection[2]);
        //                        if (charsCounter == 1)
        //                        {
        //                            // Select only command characteristic (BOOTLOADER MODE)
        //                            tempNode = new NetworkNode(bluetoothLeDevice, ServiceCollection, CharacteristicCollection);

        //                            if (tempNode != null)
        //                            {
        //                                tempNode.SelectedService = ServiceCollection[2].service;

        //                                // EnableCharacteristicPanels(CharacteristicCollection[0].characteristic.CharacteristicProperties);
        //                                tempNode.SelectedCharacteristic_cmd = CharacteristicCollection[0].characteristic;

        //                                selectedCharacteristic_cmd = tempNode.SelectedCharacteristic_cmd;

        //                                CmdCharacteristicSubscribeButton_Click(tempNode);
        //                            }
        //                        }
        //                        else if (charsCounter == 2)
        //                        {
        //                            // Select the first characteristic for command and the second for data (APP / NORMAL MODE)
        //                            tempNode = new NetworkNode(bluetoothLeDevice, ServiceCollection, CharacteristicCollection);

        //                            if (tempNode != null)
        //                            {
        //                                tempNode.SelectedService = ServiceCollection[2].service;

        //                                // EnableCharacteristicPanels(CharacteristicCollection[0].characteristic.CharacteristicProperties);
        //                                tempNode.SelectedCharacteristic_cmd = CharacteristicCollection[0].characteristic;
        //                                selectedCharacteristic_cmd = tempNode.SelectedCharacteristic_cmd;

        //                                // EnableCharacteristicPanels(CharacteristicCollection[1].characteristic.CharacteristicProperties);
        //                                tempNode.SelectedCharacteristic_data = CharacteristicCollection[1].characteristic;
        //                                selectedCharacteristic_data = tempNode.SelectedCharacteristic_data;

        //                                CmdCharacteristicSubscribeButton_Click(tempNode);
        //                                DataCharacteristicSubscribeButton_Click(tempNode);
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            if (tempNode != null)
        //            {
        //                LV_NetworkNodes.Items.Add(tempNode.BLEDevice.Name);
        //                BLENetwork.Add(tempNode);
        //            }
        //            BtnConnect.IsEnabled = true;
        //        }
        //        StopBleDeviceWatcher();
        //    }
        //    catch (Exception) // when (ex.HResult == E_DEVICE_NOT_AVAILABLE)
        //    {
        //    }
        //}

        #endregion

        #region SERVICES / CHARACTERISTICS ENUMERATION AND SELECTION

        private async Task<bool> ClearBluetoothLEDeviceAsync()
        {
            // Clear command notification subscription
            if (subscribedForNotifications_cmd)
            {
                // Need to clear the CCCD from the remote device so we stop receiving notifications
                foreach (var rc in registeredCharacteristic_cmd)
                {
                    var result = await rc.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                    if (result != GattCommunicationStatus.Success)
                    {
                        return false;
                    }
                    else
                    {
                        // selectedCharacteristic_cmd.ValueChanged -= CmdCharacteristic_ValueChanged;
                        subscribedForNotifications_cmd = false;
                    }
                }
            }

            // Clear data notification subscription
            if (subscribedForNotifications_data)
            {
                // Need to clear the CCCD from the remote device so we stop receiving notifications
                foreach (var rc in registeredCharacteristic_data)
                {
                    var result = await rc.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                    if (result != GattCommunicationStatus.Success)
                    {
                        return false;
                    }
                    else
                    {
                        // selectedCharacteristic_data.ValueChanged -= DataCharacteristic_ValueChanged;
                        subscribedForNotifications_data = false;
                    }
                }
            }
            bluetoothLeDevice?.Dispose();
            bluetoothLeDevice = null;

            return true;

            /*
            // Clear command notification subscription
            if (subscribedForNotifications_cmd)
            {
                // Need to clear the CCCD from the remote device so we stop receiving notifications
                for (int i=0; i<BLENetwork.Count; i++) // foreach (var rc in registeredCharacteristic_cmd)
                {
                    var result = await BLENetwork[i].SelectedCharacteristic_cmd.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                    if (result != GattCommunicationStatus.Success)
                    {
                        return false;
                    }
                    else
                    {
                        // selectedCharacteristic_cmd.ValueChanged -= CmdCharacteristic_ValueChanged;
                        subscribedForNotifications_cmd = false;
                    }
                }
            }

            // Clear data notification subscription
            if (subscribedForNotifications_data)
            {
                // Need to clear the CCCD from the remote device so we stop receiving notifications
                for (int i = 0; i < BLENetwork.Count; i++) // foreach (var rc in registeredCharacteristic_data)
                {
                    var result = await BLENetwork[i].SelectedCharacteristic_data.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                    if (result != GattCommunicationStatus.Success)
                    {
                        return false;
                    }
                    else
                    {
                        // selectedCharacteristic_data.ValueChanged -= DataCharacteristic_ValueChanged;
                        subscribedForNotifications_data = false;
                    }
                }
            }
            bluetoothLeDevice?.Dispose();
            bluetoothLeDevice = null;

            return true;
            */
        }

        private async Task<int> GetCharacteristics(BluetoothLEAttributeDisplay selectedService)
        {
            // CharacteristicCollection.Clear();
            RemoveValueChangedHandlerCmd();
            RemoveValueChangedHandlerData();

            IReadOnlyList<GattCharacteristic> characteristics = null;
            try
            {
                // Ensure we have access to the device.
                var accessStatus = await selectedService.service.RequestAccessAsync();
                if (accessStatus == DeviceAccessStatus.Allowed)
                {
                    // BT_Code: Get all the child characteristics of a service. Use the cache mode to specify uncached characterstics only 
                    // and the new Async functions to get the characteristics of unpaired devices as well. 
                    var result = await selectedService.service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        characteristics = result.Characteristics;
                    }
                    else
                    {
                        // On error, act as if there are no characteristics.
                        characteristics = new List<GattCharacteristic>();
                    }
                }
                else
                {
                    // On error, act as if there are no characteristics.
                    characteristics = new List<GattCharacteristic>();
                }
            }
            catch (Exception)
            {
                // On error, act as if there are no characteristics.
                characteristics = new List<GattCharacteristic>();
            }

            foreach (GattCharacteristic c in characteristics)
            {
                CharacteristicCollection.Add(new BluetoothLEAttributeDisplay(c));
            }

            return CharacteristicCollection.Count;
        }

        // CHARACTERISTIC SELECTION

        private void AddValueChangedHandlerCmd(NetworkNode node)
        {
            if (!subscribedForNotifications_cmd)
            {
                // int len = BLENetwork.Count;
                // NetworkNode.SelectedCharacteristicNetwork_cmd[len-1].ValueChanged += CmdCharacteristic_ValueChanged;

                node.SelectedCharacteristic_cmd.ValueChanged += CmdCharacteristic_ValueChanged;
                subscribedForNotifications_cmd = true;

                registeredCharacteristic_cmd.Add(node.SelectedCharacteristic_cmd);
                int len = registeredCharacteristic_cmd.Count;
                registeredCharacteristic_cmd[len - 1].ValueChanged += CmdCharacteristic_ValueChanged;
                subscribedForNotifications_cmd = true;
            }
        }

        private void AddValueChangedHandlerData(NetworkNode node)
        {
            if (!subscribedForNotifications_data)
            {
                node.SelectedCharacteristic_data.ValueChanged += DataCharacteristic_ValueChanged;
                subscribedForNotifications_data = true;

                registeredCharacteristic_data.Add(node.SelectedCharacteristic_data);
                int len = registeredCharacteristic_data.Count;
                registeredCharacteristic_data[len - 1].ValueChanged += DataCharacteristic_ValueChanged;
                subscribedForNotifications_data = true;
            }
        }

        private void RemoveValueChangedHandlerCmd()
        {
            /*
            if (subscribedForNotifications_cmd)
            {
                for (int i=0; i<BLENetwork.Count; i++) // foreach (var rc in registeredCharacteristic_cmd)
                    BLENetwork[i].SelectedCharacteristic_cmd.ValueChanged -= CmdCharacteristic_ValueChanged;

                // registeredCharacteristic_cmd = new ObservableCollection<GattCharacteristic>();
                subscribedForNotifications_cmd = false;
            }
            */

            if (subscribedForNotifications_cmd)
            {
                foreach (var rc in registeredCharacteristic_cmd)
                    rc.ValueChanged -= CmdCharacteristic_ValueChanged;

                registeredCharacteristic_cmd = new ObservableCollection<GattCharacteristic>();
                subscribedForNotifications_cmd = false;
            }
        }

        private void RemoveValueChangedHandlerData()
        {
            /*
            if (subscribedForNotifications_data)
            {
                for (int i = 0; i < BLENetwork.Count; i++) // foreach (var rc in registeredCharacteristic_data)
                    BLENetwork[i].SelectedCharacteristic_data.ValueChanged -= DataCharacteristic_ValueChanged;

                // registeredCharacteristic_data = new ObservableCollection<GattCharacteristic>();
                subscribedForNotifications_data = false;
            }
            */

            if (subscribedForNotifications_data)
            {
                foreach (var rc in registeredCharacteristic_data)
                    rc.ValueChanged -= DataCharacteristic_ValueChanged;

                registeredCharacteristic_data = new ObservableCollection<GattCharacteristic>();
                subscribedForNotifications_data = false;
            }
        }

        private void SetVisibility(UIElement element, bool visible)
        {
            element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EnableCharacteristicPanels(GattCharacteristicProperties properties)
        {
            ////////////// COMMAND CHARACTERISTIC //////////////
            // BT_Code: Hide the controls which do not apply to this characteristic.
            // SetVisibility(btn_CmdCharacteristicRead, properties.HasFlag(GattCharacteristicProperties.Read));
            // CharacteristicWriteValue.Text = "";
        }

        #endregion

        //#region CHARACTERISTICS READ / SUBSCRIBE / WRITE

        //// READ
        //private async void CmdCharacteristicReadButton_Click()
        //{
        //    // Read the actual value from the device by using Uncached
        //    GattReadResult result = await selectedCharacteristic_cmd.ReadValueAsync(BluetoothCacheMode.Uncached);
        //    if (result.Status == GattCommunicationStatus.Success)
        //    {
        //        string formattedResult = FormatValueByPresentation(result.Value, presentationFormat);
        //        // CmdCharacteristicReadValue.Text = formattedResult;
        //        // rootPage.NotifyUser($"Read result: {formattedResult}", NotifyType.StatusMessage);
        //    }
        //    else
        //    {
        //        // rootPage.NotifyUser($"Read failed: {result.Status}", NotifyType.ErrorMessage);
        //    }
        //}

        // SUBSCRIBE
        private async void CmdCharacteristicSubscribeButton_Click(NetworkNode cNode)
        {
            if (!cNode.SubscribedForNotifications_cmd)
            {
                // Status initialization
                GattCommunicationStatus status = GattCommunicationStatus.Unreachable;
                var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
                if (cNode.SelectedCharacteristic_cmd.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                {
                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
                }

                else if (cNode.SelectedCharacteristic_cmd.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                {
                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
                }

                try
                {
                    // Must write the CCCD in order for server to send indications.
                    // We receive them in the ValueChanged event handler.
                    status = await cNode.SelectedCharacteristic_cmd.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);

                    if (status == GattCommunicationStatus.Success)
                        AddValueChangedHandlerCmd(cNode);
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
            else
            {
                try
                {
                    // Must write the CCCD in order for server to send notifications.
                    // We receive them in the ValueChanged event handler.
                    // This configures either Indicate or Notify, but not both.
                    var result = await
                            cNode.SelectedCharacteristic_cmd.WriteClientCharacteristicConfigurationDescriptorAsync(
                                GattClientCharacteristicConfigurationDescriptorValue.None);
                    if (result == GattCommunicationStatus.Success)
                    {
                        cNode.SubscribedForNotifications_cmd = false;
                        RemoveValueChangedHandlerCmd();
                    }
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        private async void DataCharacteristicSubscribeButton_Click(NetworkNode cNode)
        {
            if (!cNode.SubscribedForNotifications_data)
            {
                // Status initialization
                GattCommunicationStatus status = GattCommunicationStatus.Unreachable;
                var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
                if (cNode.SelectedCharacteristic_data.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                {
                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
                }

                else if (cNode.SelectedCharacteristic_data.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                {
                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
                }

                try
                {
                    // Must write the CCCD in order for server to send indications.
                    // We receive them in the ValueChanged event handler.
                    status = await cNode.SelectedCharacteristic_data.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);

                    if (status == GattCommunicationStatus.Success)
                        AddValueChangedHandlerData(cNode);
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
            else
            {
                try
                {
                    // Must write the CCCD in order for server to send notifications.
                    // We receive them in the ValueChanged event handler.
                    // This configures either Indicate or Notify, but not both.
                    var result = await
                            cNode.SelectedCharacteristic_data.WriteClientCharacteristicConfigurationDescriptorAsync(
                                GattClientCharacteristicConfigurationDescriptorValue.None);
                    if (result == GattCommunicationStatus.Success)
                        cNode.SubscribedForNotifications_data = false;
                    RemoveValueChangedHandlerData();
                }
                catch (UnauthorizedAccessException)
                {
                }
            }
        }

        private string FormatValueByPresentation(IBuffer buffer, GattPresentationFormat format)
        {
            byte[] data;
            CryptographicBuffer.CopyToByteArray(buffer, out data);

            string str = BitConverter.ToString(data).Replace("-", " ");
            if (str.Length == 0)
                str = "Empty data received";

            return str;
        }

        private int iterationCounter = 0;

        NetworkNode currentNode = null;

        private async void CmdCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            // string currentDevice = sender.Service.Device.Name;

            /*
            NetworkNode currentNode = null;
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                string namePg = App.GetCurrentPageName();
                if (App.GetCurrentPageName() == "HomePage")
                    currentNode = BLENetwork[PagesGateway.HomePg.LV_NetworkNodes.SelectedIndex];
                else if (App.GetCurrentPageName() == "ConfigurePage")
                    currentNode = BLENetwork[PagesGateway.ConfigPg.LV_NetworkNodes.SelectedIndex];
                else if (App.GetCurrentPageName() == "ViewPage")
                    currentNode = BLENetwork[PagesGateway.ViewPg.LV_NetworkNodes.SelectedIndex];
            });

            if (currentNode != null)
            {
            */
            // string cmd_output_string = "";
            // An Indicate or Notify reported that the value has changed. Display the new value with a timestamp.
            string newValue = FormatValueByPresentation(args.CharacteristicValue, presentationFormat);

            // Format characteristic value into array of HEX
            char[] whitespace = new char[] { ' ', '\t' };
            string[] ssizes = newValue.Split(whitespace);
            byte[] messageValue = new byte[ssizes.Length];

            for (int i = 0; i < ssizes.Length; i++)
            {
                int value = Convert.ToInt16(ssizes[i], 16);
                messageValue[i] = Convert.ToByte(value);
            }

            int packetDim = 20;
            byte[] buffer = new byte[packetDim];
            int chunk_parity_value = 0;
            byte[] chunk_id_bytes = null;
            int payload_size = 0;

            // Write to selected characteristic
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.BigEndian;
            var writeSuccessful = false;

            // Check characteristic value for fw upload application
            bool read = false;
            var message = "";
            byte[] tmp;
            bool ack_val = false;

            string cmd_output_string = "";

            // Copy received message as byte array
            string cmd_raw_output_string = newValue;

            switch (messageValue[2] & 0x7F)
            {
                case (byte)Mitch_HW.Command.CMD_ACK: // = 0x00,                  //!< CMD_ACK
                    break;
                case (byte)Mitch_HW.Command.CMD_SHUTDOWN: //  = 0x01,            //!< CMD_SHUTDOWN
                    break;
                case (byte)Mitch_HW.Command.CMD_STATE: //  = 0x02,               //!< CMD_STATE
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            switch (messageValue[4])
                            {
                                case (byte)Mitch_HW.SystemState.SYS_NULL: // = 0x00,    // Added only for consistency check on sw side
                                    cmd_output_string = "SYS_NULL";
                                    deviceState = "";
                                    break;
                                case (byte)Mitch_HW.SystemState.SYS_BOOT_STARTUP: //  = 0xF0,        //!< APP_STARTUP
                                    cmd_output_string = "SYS_BOOT_STARTUP";
                                    deviceState = "STATE: BOOT STARTUP";
                                    break;
                                case (byte)Mitch_HW.SystemState.SYS_BOOT_IDLE: //  = 0xF1,           //!< APP_BOOT_IDLE
                                    cmd_output_string = "SYS_BOOT_IDLE";
                                    deviceState = "STATE: BOOT IDLE";
                                    break;
                                case (byte)Mitch_HW.SystemState.SYS_BOOT_WRITE: //  = 0xF2,          //!< APP_BOOT_WRITE
                                    cmd_output_string = "SYS_BOOT_WRITE";
                                    deviceState = "STATE: BOOT WRITE";
                                    break;
                                case (byte)Mitch_HW.SystemState.SYS_ERROR: // = 0xFF,               //!< APP_ERROR
                                    cmd_output_string = "SYS_ERROR";
                                    deviceState = "STATE: ERROR";
                                    break;
                                case (byte)Mitch_HW.SystemState.SYS_STARTUP: //  = 0x01,             //!< SYS_STARTUP
                                    cmd_output_string = "SYS_STARTUP";
                                    deviceState = "STATE: STARTUP";
                                    break;
                                case (byte)Mitch_HW.SystemState.SYS_IDLE: //  = 0x02,                //!< SYS_IDLE
                                    cmd_output_string = "SYS_IDLE";
                                    deviceState = "STATE: IDLE";
                                    break;
                                case (byte)Mitch_HW.SystemState.SYS_STANDBY: //  = 0x03,             //!< SYS_STANDBY
                                    cmd_output_string = "SYS_STANDBY";
                                    deviceState = "STATE: STANDBY";
                                    break;
                                case (byte)Mitch_HW.SystemState.SYS_LOG: //  = 0x04,                 //!< SYS_RUN
                                    cmd_output_string = "SYS_LOG";
                                    deviceState = "STATE: RECORDING";
                                    break;
                                case (byte)Mitch_HW.SystemState.SYS_READOUT: //  = 0x05              //!< SYS_READOUT
                                    cmd_output_string = "SYS_READOUT";
                                    deviceState = "STATE: MEMORY READOUT";
                                    break;
                                case (byte)Mitch_HW.SystemState.SYS_TX: //  = 0xF8                  //!< SYS_TX
                                    cmd_output_string = "SYS_TX";
                                    deviceState = "STATE: STREAMING";
                                    break;
                                default:
                                    cmd_output_string = "N.A.";
                                    deviceState = "";
                                    break;
                            }
                        }
                        else
                        {
                            switch (lastSetState)
                            {
                                case NativeStates.NONE:
                                    break;
                                case NativeStates.STREAM:
                                    if (messageValue[1] == 5 && messageValue[4] == (byte)Mitch_HW.SystemState.SYS_TX)
                                    {
                                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                        {
                                            foreach (var item in BLENetwork)
                                            {
                                                if (item.BLEDevice.DeviceId == sender.Service.Device.DeviceId)
                                                    item.DeviceStatus = "STATE: STREAMING";
                                            }
                                        });

                                        // Get gyroscope resolution from full scale
                                        switch (messageValue[5])
                                        {
                                            case (byte)Mitch_HW.Gyroscope_FS.GYR_FS_NULL:
                                                current_gyr_resolution = 0;
                                                break;
                                            case (byte)Mitch_HW.Gyroscope_FS.GYR_FS_245_DPS:
                                                current_gyr_resolution = Mitch_HW.GYR_RESOLUTION_245dps;
                                                break;
                                            case (byte)Mitch_HW.Gyroscope_FS.GYR_FS_500_DPS:
                                                current_gyr_resolution = Mitch_HW.GYR_RESOLUTION_500dps;
                                                break;
                                            case (byte)Mitch_HW.Gyroscope_FS.GYR_FS_1000_DPS:
                                                current_gyr_resolution = Mitch_HW.GYR_RESOLUTION_1000dps;
                                                break;
                                            case (byte)Mitch_HW.Gyroscope_FS.GYR_FS_2000_DPS:
                                                current_gyr_resolution = Mitch_HW.GYR_RESOLUTION_2000dps;
                                                break;
                                            default:
                                                current_gyr_resolution = 0;
                                                break;
                                        }

                                        // Get accelerometer resolution from full scale
                                        switch (messageValue[6])
                                        {
                                            case (byte)Mitch_HW.Accelerometer_FS.AXL_FS_NULL:
                                                current_axl_resolution = 0;
                                                break;
                                            case (byte)Mitch_HW.Accelerometer_FS.AXL_FS_2_g:
                                                current_axl_resolution = Mitch_HW.AXL_RESOLUTION_2g;
                                                break;
                                            case (byte)Mitch_HW.Accelerometer_FS.AXL_FS_4_g:
                                                current_axl_resolution = Mitch_HW.AXL_RESOLUTION_4g;
                                                break;
                                            case (byte)Mitch_HW.Accelerometer_FS.AXL_FS_8_g:
                                                current_axl_resolution = Mitch_HW.AXL_RESOLUTION_8g;
                                                break;
                                            case (byte)Mitch_HW.Accelerometer_FS.AXL_FS_16_g:
                                                current_axl_resolution = Mitch_HW.AXL_RESOLUTION_16g;
                                                break;
                                            default:
                                                current_axl_resolution = 0;
                                                break;
                                        }

                                        // Mag resolution has been hard-coded on fw side
                                        current_mag_resolution = Mitch_HW.MAG_RESOLUTION;

                                        // Setup environment variables to manage data streaming
                                        iterationCounter = 0;
                                    }
                                    break;
                                case NativeStates.LOG:
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                            // deviceCounterCheckForSound++;

                                            foreach (var item in BLENetwork)
                                        {
                                            if (item.BLEDevice.DeviceId == sender.Service.Device.DeviceId)
                                                item.DeviceStatus = "STATE: RECORDING";
                                        }

                                            /*
                                            if (deviceCounterCheckForSound >= BLENetwork.Count)
                                            {
                                                foreach (var item in BLENetwork)
                                                {
                                                    if (item.DeviceStatus == "STATE: RECORDING")
                                                        stateCounterCheckForSound++;
                                                }

                                                if (stateCounterCheckForSound < BLENetwork.Count)
                                                {
                                                    // Error sound
                                                    MediaPlayer player = new MediaPlayer();
                                                    StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
                                                    Windows.Storage.StorageFile file = await folder.GetFileAsync("Microsoft_error.wav");
                                                    player.AutoPlay = false;
                                                    player.Source = MediaSource.CreateFromStorageFile(file);
                                                    player.Play();
                                                }

                                                deviceCounterCheckForSound = 0;
                                                stateCounterCheckForSound = 0;
                                            }
                                            */
                                    });
                                    break;
                                case NativeStates.IDLE:
                                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                                    {
                                            // deviceCounterCheckForSound++;

                                            foreach (var item in BLENetwork)
                                        {
                                            if (item.BLEDevice.DeviceId == sender.Service.Device.DeviceId)
                                            {
                                                item.DeviceStatus = "STATE: IDLE";
                                                    // stateCounterCheckForSound++;
                                                }
                                        }
                                    });

                                    /*
                                    if (deviceCounterCheckForSound >= BLENetwork.Count && stateCounterCheckForSound < BLENetwork.Count)
                                    {
                                        deviceCounterCheckForSound = 0;
                                        stateCounterCheckForSound = 0;

                                        // Error sound
                                        MediaPlayer player = new MediaPlayer();
                                        StorageFolder folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync(@"Assets");
                                        Windows.Storage.StorageFile file = await folder.GetFileAsync("Microsoft_error.wav");
                                        player.AutoPlay = false;
                                        player.Source = MediaSource.CreateFromStorageFile(file);
                                        player.Play();
                                    }
                                    */

                                    break;
                                default:
                                    break;

                            }
                        }
                        /*
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            if (PagesGateway.HomePg != null)
                            {
                                PagesGateway.HomePg.LblCurrentState.Text = cmd_output_string;

                                PagesGateway.HomePg.LblCurrentStream1.Text = "";
                                PagesGateway.HomePg.LblCurrentStream2.Text = "";
                            }
                        });
                        */
                    }
                    else
                    {
                        // ERROR CONDITION TO BE NOTIFIED
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_RESTART: //  = 0x03,             //!< CMD_RESTART

                    // Reset cache content 
                    // var cacheSize = ((Frame)Parent).CacheSize;
                    // ((Frame)Parent).CacheSize = 0;
                    // ((Frame)Parent).CacheSize = cacheSize;

                    subscribedForNotifications_cmd = false;
                    subscribedForNotifications_data = false;

                    // ResetButton_Click();

                    break;
                case (byte)Mitch_HW.Command.CMD_APP_CRC: //  = 0x04,             //!< CMD_APP_CRC
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;
                        if (read)
                        {
                            // Decode characteristic value
                            tmp = new byte[4];
                            tmp[0] = messageValue[7];
                            tmp[1] = messageValue[6];
                            tmp[2] = messageValue[5];
                            tmp[3] = messageValue[4];
                            var fw_CRC = BitConverter.ToUInt32(tmp, 0);

                            cmd_output_string = tmp[3].ToString("X") + tmp[2].ToString("X") + tmp[1].ToString("X") + tmp[0].ToString("X") + "\t(" + fw_CRC.ToString() + ")";

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.ConfigPg.LblCurrentFWCRC.Text = cmd_output_string);
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_FW_UPLOAD: //  = 0x05,           //!< CMD_FW_UPLOAD
                    break;
                case (byte)Mitch_HW.Command.CMD_START_APP: //  = 0x06,           //!< CMD_START_APP
                    break;
                case (byte)Mitch_HW.Command.CMD_BATTERY_CHARGE: //  = 0x07,      //!< CMD_BATTERY_CHARGE
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            cmd_output_string = messageValue[4].ToString() + "%";

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.HomePg.LblBatteryCharge.Text = cmd_output_string);
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_BATTERY_VOLTAGE: //  = 0x08,     //!< CMD_BATTERY_VOLTAGE
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            // Decode characteristic value
                            tmp = new byte[2];
                            tmp[0] = messageValue[5];
                            tmp[1] = messageValue[4];
                            var battery_voltage = BitConverter.ToUInt16(tmp, 0);

                            cmd_output_string = battery_voltage + "mV";

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.HomePg.LblBatteryVoltage.Text = cmd_output_string);
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_CHECK_UP: //  = 0x09,            //!< CMD_CHECK_UP
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;
                        if (read)
                        {
                            for (int i = 0; i < messageValue[1] - 2; i++)
                                cmd_output_string = cmd_output_string + messageValue[i + 4].ToString("X") + " ";

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                if (PagesGateway.ConfigPg != null)
                                    PagesGateway.ConfigPg.LblCheckUp.Text = cmd_output_string;
                            });
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_FW_VERSION: //  = 0x0A,          //!< CMD_FW_VERSION
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;
                        if (read)
                        {
                            // Decode characteristic value
                            payload_size = messageValue[1] - 2;
                            byte[] var_fw_ver = new byte[payload_size];
                            for (int i = 0; i < payload_size; i++)
                                var_fw_ver[i] = messageValue[4 + i];
                            cmd_output_string = System.Text.Encoding.UTF8.GetString(var_fw_ver);

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.ConfigPg.LblFirmwareVersion.Text = cmd_output_string);
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_TIME: //  = 0x0B,                //!< CMD_TIME
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            // Decode characteristic value
                            byte[] currentTime = new byte[4];
                            Array.Copy(messageValue, 4, currentTime, 0, 4);
                            UInt64 currentTime_val = BitConverter.ToUInt32(currentTime, 0);

                            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                            DateTime currentDateTime = epoch.AddSeconds(currentTime_val);

                            cmd_output_string = currentDateTime.ToString();

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                if (PagesGateway.HomePg != null)
                                    PagesGateway.HomePg.LblCurrentTime.Text = cmd_output_string;

                                if (PagesGateway.ConfigPg != null)
                                    PagesGateway.ConfigPg.LblDateTime.Text = cmd_output_string;
                            });
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_BLE_NAME: //  = 0x0C,            //!< CMD_BLE_NAME
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            tmp = new byte[16];
                            Array.Copy(messageValue, 4, tmp, 0, 16);
                            cmd_output_string = System.Text.Encoding.ASCII.GetString(tmp);

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.ConfigPg.LblCurrentBLEName.Text = cmd_output_string);
                        }
                        else
                        {
                            // Decode characteristic value
                            ack_val = !Convert.ToBoolean(messageValue[3]);
                            cmd_output_string = "Ble name set!";
                            if (!ack_val)
                                cmd_output_string = "ERROR!";

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.ConfigPg.LblNewBLEName.Text = cmd_output_string);
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_HW_VERSION: //  = 0x0D,          //!< CMD_HW_VERSION
                    break;

                case (byte)Mitch_HW.Command.CMD_DEVICE_ID:
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            tmp = new byte[2];
                            Array.Copy(messageValue, 6, tmp, 0, 2);
                            deviceID = BitConverter.ToUInt16(tmp, 0);
                        }
                        else
                        {
                            // Write mode not available for this command.
                        }
                    }
                    break;

                // memory
                case (byte)Mitch_HW.Command.CMD_MEM_CONTROL: //  = 0x20,         //!< CMD_MEMORY_CONTROL
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            // Available memory space in %
                            var available_memory = messageValue[4];

                            // Number of files saved
                            tmp = new byte[2];
                            Array.Copy(messageValue, 5, tmp, 0, 2);
                            var num_of_files = BitConverter.ToUInt16(tmp, 0);

                            cmd_output_string = "Free memory: " + available_memory + "%; Num of files: " + num_of_files;

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.HomePg.LblMemoryControl.Text = cmd_output_string);
                        }
                        else
                        {
                            // ERASE MEMORY
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_MEM_FILE_INFO: //  = 0x21,       //!< CMD_MEMORY_FILE_INFO
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            // Decode characteristic value

                            // File timestamp
                            tmp = new byte[4];
                            Array.Copy(messageValue, 4, tmp, 0, 4);
                            var timestamp = BitConverter.ToUInt32(tmp, 0);
                            DateTime tmpDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                            tmpDateTime = tmpDateTime.AddSeconds(timestamp);

                            //GYRO FS
                            var logMode = messageValue[8];
                            var logFreq = messageValue[9];
                            var axl_fs = messageValue[10];
                            var gyr_fs = messageValue[11];

                            // Dimension of files (bytes)
                            tmp = new byte[4];
                            Array.Copy(messageValue, 12, tmp, 0, 4);
                            var file_size = BitConverter.ToUInt32(tmp, 0);

                            string logModeStr = "";
                            switch (logMode)
                            {
                                case (byte)Mitch_HW.LogMode.LOG_MODE_NONE:                      
                                    logModeStr = "LOG_MODE_NONE";
                                    break;
                                case (byte)Mitch_HW.LogMode.LOG_MODE_IMU:      				    
                                    logModeStr = "LOG_MODE_IMU";
                                    break;
                                case (byte)Mitch_HW.LogMode.LOG_MODE_IMU_INSOLE:                
                                    logModeStr = "LOG_MODE_IMU_INSOLE";
                                    break;
                                case (byte)Mitch_HW.LogMode.LOG_MODE_ALL:                       
                                    logModeStr = "LOG_MODE_ALL";
                                    break;
                                case (byte)Mitch_HW.LogMode.LOG_MODE_IMU_TIMESTAMP:             
                                    logModeStr = "LOG_MODE_IMU_TIMESTAMP";
                                    break;
                                case (byte)Mitch_HW.LogMode.LOG_MODE_IMU_INSOLE_TIMPESTAMP:     
                                    logModeStr = "LOG_MODE_IMU_INSOLE_TIMPESTAMP";
                                    break;
                                case (byte)Mitch_HW.LogMode.LOG_MODE_ALL_TIMESTAMP:             
                                    logModeStr = "LOG_MODE_ALL_TIMESTAMP";
                                    break;
                                default:
                                    logModeStr = "N.A.";
                                    break;
                            }

                            string logFreqStr = "";
                            switch (logFreq)
                            {
                                case (byte)Mitch_HW.LogFrequency.LOG_FREQ_NONE:
                                    logFreqStr = "None";
                                    break;                           
                                case (byte)Mitch_HW.LogFrequency.LOG_FREQ_25HZ:
                                    logFreqStr = "25 Hz";
                                    break;
                                case (byte)Mitch_HW.LogFrequency.LOG_FREQ_50HZ:
                                    logFreqStr = "50 Hz";
                                    break;
                                case (byte)Mitch_HW.LogFrequency.LOG_FREQ_100HZ:
                                    logFreqStr = "100 Hz";
                                    break;
                                case (byte)Mitch_HW.LogFrequency.LOG_FREQ_200HZ:
                                    logFreqStr = "200 Hz";
                                    break;
                                case (byte)Mitch_HW.LogFrequency.LOG_FREQ_500HZ:
                                    logFreqStr = "500 Hz";
                                    break;
                                case (byte)Mitch_HW.LogFrequency.LOG_FREQ_1000HZ:
                                    logFreqStr = "1000 Hz";
                                    break;
                                default:
                                    logFreqStr = "N.A.";
                                    break;
                            }

                            string axl_fsStr = "";
                            switch (axl_fs)
                            {
                                case (byte)Mitch_HW.Accelerometer_FS.AXL_FS_NULL:
                                    axl_fsStr = "None";
                                    break;
                                case (byte)Mitch_HW.Accelerometer_FS.AXL_FS_2_g:
                                    axl_fsStr = "2 g";
                                    break;
                                case (byte)Mitch_HW.Accelerometer_FS.AXL_FS_4_g:
                                    axl_fsStr = "4 g";
                                    break;
                                case (byte)Mitch_HW.Accelerometer_FS.AXL_FS_8_g:
                                    axl_fsStr = "8 g";
                                    break;
                                case (byte)Mitch_HW.Accelerometer_FS.AXL_FS_16_g:
                                    axl_fsStr = "16 g";
                                    break;
                                default:
                                    axl_fsStr = "N.A.";
                                    break;
                            }

                            string gyr_fsStr = "";
                            switch (gyr_fs)
                            {
                                case (byte)Mitch_HW.Gyroscope_FS.GYR_FS_NULL:
                                    gyr_fsStr = "None";
                                    break;
                                case (byte)Mitch_HW.Gyroscope_FS.GYR_FS_245_DPS:
                                    gyr_fsStr = "245 dps";
                                    break;
                                case (byte)Mitch_HW.Gyroscope_FS.GYR_FS_500_DPS:
                                    gyr_fsStr = "500 dps";
                                    break;
                                case (byte)Mitch_HW.Gyroscope_FS.GYR_FS_1000_DPS:
                                    gyr_fsStr = "1000 dps";
                                    break;
                                case (byte)Mitch_HW.Gyroscope_FS.GYR_FS_2000_DPS:
                                    gyr_fsStr = "2000 dps";
                                    break;
                                default:
                                    gyr_fsStr = "N.A.";
                                    break;
                            }

                            cmd_output_string = "Timestamp: " + tmpDateTime.ToString() + "; Mode: " + logModeStr + "; Freq: " + logFreqStr + ";\nAxl FS: " + axl_fsStr + "; Gyr FS: " + gyr_fsStr + "; File Size: " + file_size;

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.HomePg.LblCurrentFileInfo.Text = cmd_output_string);
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_MEM_FILE_DOWNLOAD: //  = 0x22,   //!< CMD_MEMORY_FILE_DOWNLOAD
                                                                   // Command not available! TO BE IMPLEMENTED ON FW SIDE
                    break;

                // time sync
                case (byte)Mitch_HW.Command.CMD_CLK_DRIFT: //  = 0x30,           //!< CMD_CLK_DRIFT
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                        }
                        else
                        {
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_CLK_OFFSET: //  = 0x31,          //!< CMD_CLK_OFFSET
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            UInt64 CurrentOffsetSet = BitConverter.ToUInt64(messageValue, 4);
                            cmd_output_string = CurrentOffsetSet.ToString();
                        }
                        else
                        {
                            currentNode = null;
                            /*
                            tmp = new byte[4];
                            Array.Copy(messageValue, 4, tmp, 0, 4);
                            var timestamp = BitConverter.ToUInt32(tmp, 0);
                            DateTime tmpDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                            tmpDateTime = tmpDateTime.AddSeconds(timestamp);

                            cmd_output_string = "Offset correctly set!";
                            */
                        }

                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.ConfigPg.LblCurrentOffsetValue.Text = cmd_output_string);
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_TIME_SYNC: //  = 0x32,           //!< CMD_TIME_SYNC

                    if (currentNode == null)
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => currentNode = BLENetwork[PagesGateway.ConfigPg.LV_NetworkNodes.SelectedIndex]);

                    read = (messageValue[2] & 0x80) != 0;
                    if (read)
                    {
                        if (messageValue[3] == 0)
                        {
                            //T4 = HighResolutionDateTime.UtcNow; todo 
                            T4 = DateTime.Now;
                            result_estimate += T4.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                            result_estimate += T1.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                            result_estimate -= reference_epoch * 1000 * 2;
                            UInt64 timeStamp = BitConverter.ToUInt64(messageValue, 4);
                            result_estimate -= timeStamp;
                            System.Threading.Thread.Sleep(200);
                            current_estimate_iterations++;
                            if (current_estimate_iterations < estimate_iterations)
                            {
                                buffer[0] = (byte)(Mitch_HW.Command.CMD_TIME_SYNC + 0x80);
                                buffer[1] = 0x00;
                                // Padding
                                for (int i = 2; i < Mitch_HW.COMM_MESSAGE_LEN; i++)
                                    buffer[i] = 0;

                                //T1 = HighResolutionDateTime.UtcNow; todo 
                                T1 = DateTime.Now;

                                // Write to selected characteristic
                                writer.WriteBytes(buffer);
                                writeSuccessful = await WriteBufferToSelectedCharacteristicAsync(writer.DetachBuffer(), currentNode);
                            }
                            else
                            {
                                result_estimate /= estimate_iterations;
                                result_estimate /= 2;
                                System.Threading.Thread.Sleep(200);
                                //Exit time sync
                                buffer[0] = (byte)(Mitch_HW.Command.CMD_EXIT_TIME_SYNC);
                                buffer[1] = 0x00;
                                // Padding
                                for (int i = 2; i < Mitch_HW.COMM_MESSAGE_LEN; i++)
                                    buffer[i] = 0;
                                System.Threading.Thread.Sleep(200);
                                // Write to selected characteristic
                                writer.WriteBytes(buffer);
                                writeSuccessful = await WriteBufferToSelectedCharacteristicAsync(writer.DetachBuffer(), currentNode);
                            }
                        }
                    }
                    else
                    {
                        if (messageValue[3] == 0)
                        {
                            current_estimate_iterations = 0; // reset estimate iteration counter
                            result_estimate = 0; // reset result_estimate variable
                            System.Threading.Thread.Sleep(200);
                            buffer[0] = (byte)(Mitch_HW.Command.CMD_TIME_SYNC + 0x80);  // 0xB2
                            buffer[1] = 0x00;
                            // Padding
                            for (int i = 2; i < Mitch_HW.COMM_MESSAGE_LEN; i++)
                                buffer[i] = 0;

                            //T1 = HighResolutionDateTime.UtcNow; todo 
                            T1 = DateTime.Now;

                            // Write to selected characteristic
                            writer.WriteBytes(buffer);
                            writeSuccessful = await WriteBufferToSelectedCharacteristicAsync(writer.DetachBuffer(), currentNode);
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_EXIT_TIME_SYNC: //  = 0x33,      //!< CMD_EXIT_TIME_SYNC
                    if (messageValue[3] == 0)
                    {
                        // set clock estimation
                        int respLen = 8;
                        buffer[0] = (byte)(Mitch_HW.Command.CMD_CLK_OFFSET);
                        buffer[1] = Convert.ToByte(respLen);
                        UInt64 currentOffsetValue = (UInt64)result_estimate;

                        for (int i = 0; i < 8; i++)
                            buffer[2 + i] = (byte)((currentOffsetValue >> (i * 8)) & 0x00000000000000FF);

                        // Padding
                        for (int i = 2 + respLen; i < Mitch_HW.COMM_MESSAGE_LEN; i++)
                            buffer[i] = 0;

                        // Write to selected characteristic
                        writer.WriteBytes(buffer);
                        writeSuccessful = await WriteBufferToSelectedCharacteristicAsync(writer.DetachBuffer(), currentNode);

                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.ConfigPg.LblOffsetValue.Text = currentOffsetValue.ToString());
                    }
                    break;

                // mems
                case (byte)Mitch_HW.Command.CMD_FS_AXL_GYRO: //  = 0x40,         //!< CMD_FS_AXL_GYRO
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            switch ((Mitch_HW.Accelerometer_FS)messageValue[4])
                            {
                                case Mitch_HW.Accelerometer_FS.AXL_FS_2_g:
                                    cmd_output_string = "Axl FS: 2 g; ";
                                    break;
                                case Mitch_HW.Accelerometer_FS.AXL_FS_4_g:
                                    cmd_output_string = "Axl FS: 4 g; ";
                                    break;
                                case Mitch_HW.Accelerometer_FS.AXL_FS_8_g:
                                    cmd_output_string = "Axl FS: 8 g; ";
                                    break;
                                case Mitch_HW.Accelerometer_FS.AXL_FS_16_g:
                                    cmd_output_string = "Axl FS: 16 g; ";
                                    break;
                                default:
                                    cmd_output_string = "Axl FS: N.A.; ";
                                    break;
                            }

                            switch ((Mitch_HW.Gyroscope_FS)messageValue[5])
                            {
                                case Mitch_HW.Gyroscope_FS.GYR_FS_245_DPS:
                                    cmd_output_string = cmd_output_string + "Gyro FS: 245 dps;";
                                    break;
                                case Mitch_HW.Gyroscope_FS.GYR_FS_500_DPS:
                                    cmd_output_string = cmd_output_string + "Gyro FS: 500 dps;";
                                    break;
                                case Mitch_HW.Gyroscope_FS.GYR_FS_1000_DPS:
                                    cmd_output_string = cmd_output_string + "Gyro FS: 1000 dps;";
                                    break;
                                case Mitch_HW.Gyroscope_FS.GYR_FS_2000_DPS:
                                    cmd_output_string = cmd_output_string + "Gyro FS: 2000 dps;";
                                    break;
                                default:
                                    cmd_output_string = cmd_output_string + "Gyro FS: N.A.;";
                                    break;
                            }

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.ConfigPg.Lbl6DOFFS.Text = cmd_output_string);
                        }
                        else
                        {
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_FS_AXL: //  = 0x41,              //!< CMD_FS_AXL_GYRO
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            // Read mode not available for this command.
                        }
                        else
                        {

                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_FS_GYRO: //  = 0x42              //!< CMD_FS_AXL_GYRO
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            // Read mode not available for this command.
                        }
                        else
                        {
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_FS_DS1:
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            int selectedIndex = 0;
                            switch (messageValue[4])
                            {
                                case (byte)Mitch_HW.TOF_FS.TOF_FS_200mm:
                                    cmd_output_string = "TOF #1 FS: 200 mm";
                                    selectedIndex = 0;
                                    break;
                                case (byte)Mitch_HW.TOF_FS.TOF_FS_400mm:
                                    cmd_output_string = "TOF #1 FS: 400 mm";
                                    selectedIndex = 1;
                                    break;
                                case (byte)Mitch_HW.TOF_FS.TOF_FS_600mm:
                                    cmd_output_string = "TOF #1 FS: 600 mm";
                                    selectedIndex = 2;
                                    break;
                                default:
                                    cmd_output_string = "TOF #1 FS: N.A.";
                                    selectedIndex = -1;
                                    break;
                            }

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                if (selectedIndex >= 0)
                                    PagesGateway.ConfigPg.CBTOFFS_1.SelectedIndex = selectedIndex;
                            });
                        }
                        else
                        {
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_FS_DS2:
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            int selectedIndex = 0;
                            switch (messageValue[4])
                            {
                                case (byte)Mitch_HW.TOF_FS.TOF_FS_200mm:
                                    cmd_output_string = "TOF #2 FS: 200 mm";
                                    selectedIndex = 0;
                                    break;
                                case (byte)Mitch_HW.TOF_FS.TOF_FS_400mm:
                                    cmd_output_string = "TOF #2 FS: 400 mm";
                                    selectedIndex = 1;
                                    break;
                                case (byte)Mitch_HW.TOF_FS.TOF_FS_600mm:
                                    cmd_output_string = "TOF #2 FS: 600 mm";
                                    selectedIndex = 2;
                                    break;
                                default:
                                    cmd_output_string = "TOF #2 FS: N.A.";
                                    selectedIndex = -1;
                                    break;
                            }

                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                if (selectedIndex >= 0)
                                    PagesGateway.ConfigPg.CBTOFFS_2.SelectedIndex = selectedIndex;
                            });
                        }
                        else
                        {
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_OFFSET_DS1:
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            cmd_output_string = messageValue[4].ToString();
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.ConfigPg.Lbls1offsetVal.Text = cmd_output_string);
                        }
                        else
                        {
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_OFFSET_DS2:
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            cmd_output_string = messageValue[4].ToString();
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.ConfigPg.Lbls2offsetVal.Text = cmd_output_string);
                        }
                        else
                        {
                        }
                    }
                    break;
                case (byte)Mitch_HW.Command.CMD_MATRIX_CALIBRATION:
                    if (messageValue[3] == 0)
                    {
                        read = (messageValue[2] & 0x80) != 0;

                        if (read)
                        {
                            if (messageValue[1] == 0x12) // 18 Bytes length
                            {
                                CalibrationCoefficients.Clear();

                                // Receive, decode and process one line at a time
                                byte[] currentValue = new byte[4];
                                Array.Copy(messageValue, 4, currentValue, 0, 4);
                                float current_val = BitConverter.ToSingle(currentValue, 0);
                                CalibrationCoefficients.Add(current_val);

                                currentValue = new byte[4];
                                Array.Copy(messageValue, 8, currentValue, 0, 4);
                                current_val = BitConverter.ToSingle(currentValue, 0);
                                CalibrationCoefficients.Add(current_val);

                                currentValue = new byte[4];
                                Array.Copy(messageValue, 12, currentValue, 0, 4);
                                current_val = BitConverter.ToSingle(currentValue, 0);
                                CalibrationCoefficients.Add(current_val);

                                currentValue = new byte[4];
                                Array.Copy(messageValue, 16, currentValue, 0, 4);
                                current_val = BitConverter.ToSingle(currentValue, 0);
                                CalibrationCoefficients.Add(current_val);

                                cmd_output_string = String.Format("{0},{1},{2},{3}",
                                    CalibrationCoefficients[0], CalibrationCoefficients[1], CalibrationCoefficients[2], CalibrationCoefficients[3]);

                                switch (PagesGateway.ConfigPg.selectedLINE)
                                {
                                    case 0:     // NONE
                                        break;
                                    case 1:     // GYROSCOPE
                                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.ConfigPg.LblCalibrationCoeffsLine1.Text = cmd_output_string);
                                        break;
                                    case 2:     // ACCELEROMETER
                                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.ConfigPg.LblCalibrationCoeffsLine2.Text = cmd_output_string);
                                        break;
                                    case 3:     // MAGNETOMETER
                                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.ConfigPg.LblCalibrationCoeffsLine3.Text = cmd_output_string);
                                        break;
                                    default:    // DO NOTHING
                                        break;
                                }
                            }
                        }
                        else
                        {
                        }
                    }
                    break;
                default:
                    break;
            }

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (PagesGateway.HomePg != null)
                    PagesGateway.HomePg.LblRead.Text = cmd_raw_output_string;
            });

            // return cmd_output_string;
            // }
        }

        private async void DataCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            string currentDevice = sender.Service.DeviceId.ToString();

            // "BluetoothLE#BluetoothLE00:28:f8:2f:4e:13-e5:79:af:66:95:de#GATT:0000000c:{0e00f182-7a0c-4ce2-a657-ed0fe8e0cb0d}" // TECNE LEFT

            // string cmd_output_string = "";
            // An Indicate or Notify reported that the value has changed. Display the new value with a timestamp.
            string newValue = FormatValueByPresentation(args.CharacteristicValue, presentationFormat);

            // Format characteristic value into array of HEX
            char[] whitespace = new char[] { ' ', '\t' };
            string[] ssizes = newValue.Split(whitespace);
            byte[] messageValue = new byte[ssizes.Length];

            for (int i = 0; i < ssizes.Length; i++)
            {
                int value = Convert.ToInt16(ssizes[i], 16);
                messageValue[i] = Convert.ToByte(value);
            }

            int packetDim = 20;
            byte[] buffer = new byte[packetDim];
            int chunk_parity_value = 0;
            byte[] chunk_id_bytes = null;
            int payload_size = 0;

            // Write to selected characteristic
            DataWriter writer = new DataWriter();
            writer.ByteOrder = ByteOrder.BigEndian;
            var writeSuccessful = false;

            // Check characteristic value for fw upload application
            bool read = false;
            var message = "";
            byte[] tmp;
            bool ack_val = false;

            string cmd_output_string = "";

            Mitch_Data currentData = new Mitch_Data();

            switch (messageValue[0])
            {
                case (byte)Mitch_HW.StreamMode.STREAM_MODE_PRESSURE:

                    // Get
                    byte[] currentPayload = new byte[(byte)Mitch_HW.StreamPacketDimension.STREAM_PACKET_DIM_PRESSURE];
                    Array.Copy(messageValue, 2, currentPayload, 0, (byte)Mitch_HW.StreamPacketDimension.STREAM_PACKET_DIM_PRESSURE);

                    // Decode
                    currentData = DataTypePressure(currentPayload);

                    // Publish
                    // Mitch + Mitch general app
                    cmd_output_string = string.Format("{0}  {1}  {2}  {3}  {4}  {5}  {6}  {7}  {8}  {9}  {10}  {11}  {12}  {13}  {14}  {15}",
                        currentData.Pressure[5], currentData.Pressure[7], currentData.Pressure[3], currentData.Pressure[6],
                        currentData.Pressure[4], currentData.Pressure[0], currentData.Pressure[1], currentData.Pressure[2],
                        currentData.Pressure[13], currentData.Pressure[14], currentData.Pressure[15], currentData.Pressure[11],
                        currentData.Pressure[12], currentData.Pressure[10], currentData.Pressure[9], currentData.Pressure[8]);

                    // Malvestio membrane ordered as a keyboard
                    //cmd_output_string = string.Format("{0}  {1}  {2}  {3}  {4}  {5}  {6}  {7}  {8}  {9}  {10}  {11}  {12}  {13}  {14}  {15}",
                    //    currentData.Pressure[5], currentData.Pressure[7], currentData.Pressure[3], currentData.Pressure[6],
                    //    currentData.Pressure[4], currentData.Pressure[0], currentData.Pressure[1], currentData.Pressure[2],
                    //    currentData.Pressure[13], currentData.Pressure[15], currentData.Pressure[14], currentData.Pressure[12],
                    //    currentData.Pressure[11], currentData.Pressure[8], currentData.Pressure[9], currentData.Pressure[10]);



                    break;
                case (byte)Mitch_HW.StreamMode.STREAM_MODE_6DOF_TOF:

                    // Get
                    currentPayload = new byte[(byte)Mitch_HW.StreamPacketDimension.STREAM_PACKET_DIM_6DOF_TOF];
                    Array.Copy(messageValue, 2, currentPayload, 0, (byte)Mitch_HW.StreamPacketDimension.STREAM_PACKET_DIM_6DOF_TOF);

                    // Decode
                    currentData = DataType6DOFandTOF(currentPayload, current_gyr_resolution, current_axl_resolution);

                    // Publish
                    cmd_output_string = string.Format("{0}  {1}  {2}  {3}  {4}  {5}  {6}  {7}",
                        currentData.Gyr[0], currentData.Gyr[1], currentData.Gyr[2],
                        currentData.Axl[0], currentData.Axl[1], currentData.Axl[2],
                        currentData.TOF[0], currentData.TOF[1]);

                    break;
                case (byte)Mitch_HW.StreamMode.STREAM_MODE_TOF:

                    // Get
                    currentPayload = new byte[(byte)Mitch_HW.StreamPacketDimension.STREAM_PACKET_DIM_TOF];
                    Array.Copy(messageValue, 2, currentPayload, 0, (byte)Mitch_HW.StreamPacketDimension.STREAM_PACKET_DIM_TOF);

                    // Decode
                    currentData = DataTypeTOF(currentPayload);

                    // Publish
                    cmd_output_string = string.Format("{0}  {1} ", currentData.TOF[0], currentData.TOF[1]);

                    break;
                case (byte)Mitch_HW.StreamMode.STREAM_MODE_6DOF:

                    // Get
                    currentPayload = new byte[(byte)Mitch_HW.StreamPacketDimension.STREAM_PACKET_DIM_6DOF];
                    Array.Copy(messageValue, 2, currentPayload, 0, (byte)Mitch_HW.StreamPacketDimension.STREAM_PACKET_DIM_6DOF);

                    // Decode
                    currentData = DataType6DOF(currentPayload, current_gyr_resolution, current_axl_resolution);

                    // Publish
                    cmd_output_string = string.Format("{0}  {1}  {2}  {3}  {4}  {5}",
                        currentData.Gyr[0], currentData.Gyr[1], currentData.Gyr[2],
                        currentData.Axl[0], currentData.Axl[1], currentData.Axl[2]);

                    break;
                case (byte)Mitch_HW.StreamMode.STREAM_MODE_9DOF:

                    // Get
                    currentPayload = new byte[(byte)Mitch_HW.StreamPacketDimension.STREAM_PACKET_DIM_9DOF];
                    Array.Copy(messageValue, 2, currentPayload, 0, (byte)Mitch_HW.StreamPacketDimension.STREAM_PACKET_DIM_9DOF);

                    // Decode
                    currentData = DataType9DOF(currentPayload, current_gyr_resolution, current_axl_resolution, current_mag_resolution);

                    // Publish
                    cmd_output_string = string.Format("{0}  {1}  {2}  {3}  {4}  {5}  {6}  {7}  {8}",
                        currentData.Gyr[0], currentData.Gyr[1], currentData.Gyr[2],
                        currentData.Axl[0], currentData.Axl[1], currentData.Axl[2],
                        currentData.Mag[0], currentData.Mag[1], currentData.Mag[2]);

                    break;
                default:
                    break;
            }



            // await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PagesGateway.HomePg.LblCurrentStream1.Text = cmd_output_string);
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (PagesGateway.ViewPg != null && PagesGateway.ViewPg.viewPanelEnabled == true)
                {
                    PagesGateway.ViewPg.LblCurrentStream.Text = cmd_output_string;
                }
                else
                {
                    ////////////////////////////////////////////////////////////////////

                    // Temp for TECNE test only

                    //cmd_output_string = string.Format("{0}  {1}  {2}  {3}  {4}  {5}  {6}  {7}  {8}  {9}  {10}  {11}  {12}  {13}  {14}  {15}  {16}  {17}  {18}  {19}",
                    //    messageValue[0], messageValue[1], messageValue[2], messageValue[3],
                    //    messageValue[4], messageValue[5], messageValue[6], messageValue[7],
                    //    messageValue[8], messageValue[9], messageValue[10], messageValue[11],
                    //    messageValue[12], messageValue[13], messageValue[14], messageValue[15],
                    //    messageValue[16], messageValue[17], messageValue[18], messageValue[19]);

                    //if (currentDevice == "BluetoothLE#BluetoothLE00:28:f8:2f:4e:13-e5:79:af:66:95:de#GATT:0000000c:{0e00f182-7a0c-4ce2-a657-ed0fe8e0cb0d}")
                    //{
                    //    // Tecne LEFT
                    //    PagesGateway.HomePg.LblCurrentStream1.Text = cmd_output_string;
                    //}
                    //else
                    //{
                    //    // Tecne RIGHT
                    //    PagesGateway.HomePg.LblCurrentStream2.Text = cmd_output_string;
                    //}

                    ////////////////////////////////////////////////////////////////////

                    /*
                     * TO BE DECOMMENTED AFTER TECNE TEST IN ORDER TO RESTORE OFFICIAL MITCH APP RELEASE*/
                    if (messageValue[0] == (byte)Mitch_HW.StreamMode.STREAM_MODE_9DOF)
                        PagesGateway.HomePg.LblCurrentStream1.Text = cmd_output_string;
                    else
                    {
                        var node = BLENetwork.FirstOrDefault(j => j.DeviceID == currentData.DeviceID);

                        if (node != null)
                        {

                            // node
                            int idx = BLENetwork.IndexOf(node) + 1;

                            string str = String.Concat("LblCurrentStream", idx.ToString());
                            TextBox outTxtBox = PagesGateway.HomePg.FindName(str) as TextBox;
                            if (outTxtBox != null)
                                outTxtBox.Text = cmd_output_string;
                        }
                    }
                    
                }

                if (PagesGateway.ViewPg != null && PagesGateway.ViewPg.viewPanelEnabled == true && PagesGateway.ViewPg.LV_NetworkNodes_View.SelectedItems.Count == 1) //  && PagesGateway.ViewPg.enableView == true)
                {
                    if (iterationCounter > 10)
                    {
                        PagesGateway.ViewPg.CV_PressureTime_1.Add(new ObservableValue(currentData.Pressure[0]));
                        PagesGateway.ViewPg.CV_PressureTime_2.Add(new ObservableValue(currentData.Pressure[1]));
                        PagesGateway.ViewPg.CV_PressureTime_3.Add(new ObservableValue(currentData.Pressure[2]));
                        PagesGateway.ViewPg.CV_PressureTime_4.Add(new ObservableValue(currentData.Pressure[3]));
                        PagesGateway.ViewPg.CV_PressureTime_5.Add(new ObservableValue(currentData.Pressure[4]));
                        PagesGateway.ViewPg.CV_PressureTime_6.Add(new ObservableValue(currentData.Pressure[5]));
                        PagesGateway.ViewPg.CV_PressureTime_7.Add(new ObservableValue(currentData.Pressure[6]));
                        PagesGateway.ViewPg.CV_PressureTime_8.Add(new ObservableValue(currentData.Pressure[7]));
                        PagesGateway.ViewPg.CV_PressureTime_9.Add(new ObservableValue(currentData.Pressure[8]));
                        PagesGateway.ViewPg.CV_PressureTime_10.Add(new ObservableValue(currentData.Pressure[9]));
                        PagesGateway.ViewPg.CV_PressureTime_11.Add(new ObservableValue(currentData.Pressure[10]));
                        PagesGateway.ViewPg.CV_PressureTime_12.Add(new ObservableValue(currentData.Pressure[11]));
                        PagesGateway.ViewPg.CV_PressureTime_13.Add(new ObservableValue(currentData.Pressure[12]));
                        PagesGateway.ViewPg.CV_PressureTime_14.Add(new ObservableValue(currentData.Pressure[13]));
                        PagesGateway.ViewPg.CV_PressureTime_15.Add(new ObservableValue(currentData.Pressure[14]));
                        PagesGateway.ViewPg.CV_PressureTime_16.Add(new ObservableValue(currentData.Pressure[15]));
                        PagesGateway.ViewPg.CV_PressureTime_1.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_2.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_3.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_4.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_5.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_6.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_7.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_8.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_9.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_10.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_11.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_12.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_13.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_14.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_15.RemoveAt(0);
                        PagesGateway.ViewPg.CV_PressureTime_16.RemoveAt(0);

                        PagesGateway.ViewPg.CV_Gyroscope_X.Add(new ObservableValue(currentData.Gyr[0]));
                        PagesGateway.ViewPg.CV_Gyroscope_Y.Add(new ObservableValue(currentData.Gyr[1]));
                        PagesGateway.ViewPg.CV_Gyroscope_Z.Add(new ObservableValue(currentData.Gyr[2]));
                        PagesGateway.ViewPg.CV_Accelerometer_X.Add(new ObservableValue(currentData.Axl[0]));
                        PagesGateway.ViewPg.CV_Accelerometer_Y.Add(new ObservableValue(currentData.Axl[1]));
                        PagesGateway.ViewPg.CV_Accelerometer_Z.Add(new ObservableValue(currentData.Axl[2]));
                        PagesGateway.ViewPg.CV_Magnetometer_X.Add(new ObservableValue(currentData.Mag[0]));
                        PagesGateway.ViewPg.CV_Magnetometer_Y.Add(new ObservableValue(currentData.Mag[1]));
                        PagesGateway.ViewPg.CV_Magnetometer_Z.Add(new ObservableValue(currentData.Mag[2]));

                        PagesGateway.ViewPg.CV_Gyroscope_X.RemoveAt(0);
                        PagesGateway.ViewPg.CV_Gyroscope_Y.RemoveAt(0);
                        PagesGateway.ViewPg.CV_Gyroscope_Z.RemoveAt(0);
                        PagesGateway.ViewPg.CV_Accelerometer_X.RemoveAt(0);
                        PagesGateway.ViewPg.CV_Accelerometer_Y.RemoveAt(0);
                        PagesGateway.ViewPg.CV_Accelerometer_Z.RemoveAt(0);
                        PagesGateway.ViewPg.CV_Magnetometer_X.RemoveAt(0);
                        PagesGateway.ViewPg.CV_Magnetometer_Y.RemoveAt(0);
                        PagesGateway.ViewPg.CV_Magnetometer_Z.RemoveAt(0);

                        PagesGateway.ViewPg.CV_TimeOfFlight_Left.Add(new ObservableValue(currentData.TOF[0]));
                        PagesGateway.ViewPg.CV_TimeOfFlight_Right.Add(new ObservableValue(currentData.TOF[1]));
                        PagesGateway.ViewPg.CV_TimeOfFlight_Left.RemoveAt(0);
                        PagesGateway.ViewPg.CV_TimeOfFlight_Right.RemoveAt(0);
                    }
                    else
                    {
                        PagesGateway.ViewPg.CV_PressureTime_1.Add(new ObservableValue(currentData.Pressure[0]));
                        PagesGateway.ViewPg.CV_PressureTime_2.Add(new ObservableValue(currentData.Pressure[1]));
                        PagesGateway.ViewPg.CV_PressureTime_3.Add(new ObservableValue(currentData.Pressure[2]));
                        PagesGateway.ViewPg.CV_PressureTime_4.Add(new ObservableValue(currentData.Pressure[3]));
                        PagesGateway.ViewPg.CV_PressureTime_5.Add(new ObservableValue(currentData.Pressure[4]));
                        PagesGateway.ViewPg.CV_PressureTime_6.Add(new ObservableValue(currentData.Pressure[5]));
                        PagesGateway.ViewPg.CV_PressureTime_7.Add(new ObservableValue(currentData.Pressure[6]));
                        PagesGateway.ViewPg.CV_PressureTime_8.Add(new ObservableValue(currentData.Pressure[7]));
                        PagesGateway.ViewPg.CV_PressureTime_9.Add(new ObservableValue(currentData.Pressure[8]));
                        PagesGateway.ViewPg.CV_PressureTime_10.Add(new ObservableValue(currentData.Pressure[9]));
                        PagesGateway.ViewPg.CV_PressureTime_11.Add(new ObservableValue(currentData.Pressure[10]));
                        PagesGateway.ViewPg.CV_PressureTime_12.Add(new ObservableValue(currentData.Pressure[11]));
                        PagesGateway.ViewPg.CV_PressureTime_13.Add(new ObservableValue(currentData.Pressure[12]));
                        PagesGateway.ViewPg.CV_PressureTime_14.Add(new ObservableValue(currentData.Pressure[13]));
                        PagesGateway.ViewPg.CV_PressureTime_15.Add(new ObservableValue(currentData.Pressure[14]));
                        PagesGateway.ViewPg.CV_PressureTime_16.Add(new ObservableValue(currentData.Pressure[15]));

                        PagesGateway.ViewPg.CV_Gyroscope_X.Add(new ObservableValue(currentData.Gyr[0]));
                        PagesGateway.ViewPg.CV_Gyroscope_Y.Add(new ObservableValue(currentData.Gyr[1]));
                        PagesGateway.ViewPg.CV_Gyroscope_Z.Add(new ObservableValue(currentData.Gyr[2]));
                        PagesGateway.ViewPg.CV_Accelerometer_X.Add(new ObservableValue(currentData.Axl[0]));
                        PagesGateway.ViewPg.CV_Accelerometer_Y.Add(new ObservableValue(currentData.Axl[1]));
                        PagesGateway.ViewPg.CV_Accelerometer_Z.Add(new ObservableValue(currentData.Axl[2]));
                        PagesGateway.ViewPg.CV_Magnetometer_X.Add(new ObservableValue(currentData.Mag[0]));
                        PagesGateway.ViewPg.CV_Magnetometer_Y.Add(new ObservableValue(currentData.Mag[1]));
                        PagesGateway.ViewPg.CV_Magnetometer_Z.Add(new ObservableValue(currentData.Mag[2]));

                        PagesGateway.ViewPg.CV_TimeOfFlight_Left.Add(new ObservableValue(currentData.TOF[0]));
                        PagesGateway.ViewPg.CV_TimeOfFlight_Right.Add(new ObservableValue(currentData.TOF[1]));

                        iterationCounter++;
                    }
                }
            });
        }

        public async Task<bool> WriteBufferToSelectedCharacteristicAsync(IBuffer buffer, NetworkNode cNode)
        {
            try
            {
                // Writes the value from the buffer to the characteristic
                var result = await cNode.SelectedCharacteristic_cmd.WriteValueWithResultAsync(buffer);

                if (result.Status == GattCommunicationStatus.Success)
                    return true;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SendMessage(byte[] message, NetworkNode node)
        {
            var writer = new DataWriter();
            writer.ByteOrder = ByteOrder.BigEndian;

            writer.WriteBytes(message);
            return await WriteBufferToSelectedCharacteristicAsync(writer.DetachBuffer(), node);
        }

        Mitch_Data DataTypePressure(byte[] currentPayload)
        {
            // Pressure
            Mitch_Data currentData = new Mitch_Data();

            // Decode ID field (bytes 2 and 3)
            byte[] tmp = new byte[2];
            Array.Copy(currentPayload, 0, tmp, 0, 2);
            Array.Reverse(tmp);
            currentData.DeviceID = BitConverter.ToUInt16(tmp, 0);

            // Decode data
            for (int i = 2; i < (byte)Mitch_HW.StreamPacketDimension.STREAM_PACKET_DIM_PRESSURE; i++)
                currentData.Pressure[i - 2] = (int)(100 * (1 - (float)currentPayload[i] / 255));

            return currentData;
        }

        Mitch_Data DataType6DOFandTOF(byte[] currentPayload, float gyr_res, float axl_res)
        {
            // 6DOF + TOF
            Mitch_Data currentData = new Mitch_Data();
            byte[] tmp = new byte[2];

            // Decode ID field (bytes 2 and 3)
            Array.Copy(currentPayload, 0, tmp, 0, 2);
            Array.Reverse(tmp);
            currentData.DeviceID = BitConverter.ToUInt16(tmp, 0);

            // Decode data

            // Gyroscope
            float[] currentGyr = new float[3];
            Array.Copy(currentPayload, 2, tmp, 0, 2);
            currentGyr[0] = (float)(BitConverter.ToInt16(tmp, 0) * gyr_res) / 1000;
            Array.Copy(currentPayload, 4, tmp, 0, 2);
            currentGyr[1] = (float)(BitConverter.ToInt16(tmp, 0) * gyr_res) / 1000;
            Array.Copy(currentPayload, 6, tmp, 0, 2);
            currentGyr[2] = (float)(BitConverter.ToInt16(tmp, 0) * gyr_res) / 1000;
            currentData.Gyr = currentGyr;

            // Accelerometer
            float[] currentAxl = new float[3];
            Array.Copy(currentPayload, 8, tmp, 0, 2);
            currentAxl[0] = (float)(BitConverter.ToInt16(tmp, 0) * axl_res);
            Array.Copy(currentPayload, 10, tmp, 0, 2);
            currentAxl[1] = (float)(BitConverter.ToInt16(tmp, 0) * axl_res);
            Array.Copy(currentPayload, 12, tmp, 0, 2);
            currentAxl[2] = (float)(BitConverter.ToInt16(tmp, 0) * axl_res);
            currentData.Axl = currentAxl;

            // TOF
            currentData.TOF[0] = currentPayload[14];
            currentData.TOF[1] = currentPayload[15];

            return currentData;
        }

        Mitch_Data DataTypeTOF(byte[] currentPayload)
        {
            // TOF
            Mitch_Data currentData = new Mitch_Data();

            // Decode ID field (bytes 2 and 3)
            byte[] tmp = new byte[2];
            Array.Copy(currentPayload, 0, tmp, 0, 2);
            Array.Reverse(tmp);
            currentData.DeviceID = BitConverter.ToUInt16(tmp, 0);

            // Decode data
            currentData.TOF[0] = currentPayload[2];
            currentData.TOF[1] = currentPayload[3];

            return currentData;
        }

        Mitch_Data DataType6DOF(byte[] currentPayload, float gyr_res, float axl_res)
        {
            // 6DOF
            Mitch_Data currentData = new Mitch_Data();
            byte[] tmp = new byte[2];

            // Decode ID field (bytes 2 and 3)
            Array.Copy(currentPayload, 0, tmp, 0, 2);
            Array.Reverse(tmp);
            currentData.DeviceID = BitConverter.ToUInt16(tmp, 0);

            // Decode data

            // Gyroscope
            float[] currentGyr = new float[3];
            Array.Copy(currentPayload, 2, tmp, 0, 2);
            currentGyr[0] = (float)(BitConverter.ToInt16(tmp, 0) * gyr_res) / 1000;
            Array.Copy(currentPayload, 4, tmp, 0, 2);
            currentGyr[1] = (float)(BitConverter.ToInt16(tmp, 0) * gyr_res) / 1000;
            Array.Copy(currentPayload, 6, tmp, 0, 2);
            currentGyr[2] = (float)(BitConverter.ToInt16(tmp, 0) * gyr_res) / 1000;
            currentData.Gyr = currentGyr;

            // Accelerometer
            float[] currentAxl = new float[3];
            float tempAxlVal = 0;
            Array.Copy(currentPayload, 8, tmp, 0, 2);
            tempAxlVal = BitConverter.ToInt16(tmp, 0);
            currentAxl[0] = tempAxlVal * axl_res;
            // currentAxl[0] = (float)(BitConverter.ToInt16(tmp, 0) * axl_res);

            Array.Copy(currentPayload, 10, tmp, 0, 2);
            tempAxlVal = BitConverter.ToInt16(tmp, 0);
            currentAxl[1] = tempAxlVal * axl_res;
            // currentAxl[1] = (float)(BitConverter.ToInt16(tmp, 0) * axl_res);

            Array.Copy(currentPayload, 12, tmp, 0, 2);
            tempAxlVal = BitConverter.ToInt16(tmp, 0);
            currentAxl[2] = tempAxlVal * axl_res;
            // currentAxl[2] = (float)(BitConverter.ToInt16(tmp, 0) * axl_res);

            currentData.Axl = currentAxl;

            return currentData;
        }

        Mitch_Data DataType9DOF(byte[] currentPayload, float gyr_res, float axl_res, float mag_res)
        {
            // 9DOF
            Mitch_Data currentData = new Mitch_Data();
            byte[] tmp = new byte[2];

            // Gyroscope
            float[] currentGyr = new float[3];
            Array.Copy(currentPayload, 0, tmp, 0, 2);
            currentGyr[0] = (float)(BitConverter.ToInt16(tmp, 0) * gyr_res) / 1000;
            Array.Copy(currentPayload, 2, tmp, 0, 2);
            currentGyr[1] = (float)(BitConverter.ToInt16(tmp, 0) * gyr_res) / 1000;
            Array.Copy(currentPayload, 4, tmp, 0, 2);
            currentGyr[2] = (float)(BitConverter.ToInt16(tmp, 0) * gyr_res) / 1000;
            currentData.Gyr = currentGyr;

            // Accelerometer
            float[] currentAxl = new float[3];
            Array.Copy(currentPayload, 6, tmp, 0, 2);
            currentAxl[0] = (float)(BitConverter.ToInt16(tmp, 0) * axl_res);
            Array.Copy(currentPayload, 8, tmp, 0, 2);
            currentAxl[1] = (float)(BitConverter.ToInt16(tmp, 0) * axl_res);
            Array.Copy(currentPayload, 10, tmp, 0, 2);
            currentAxl[2] = (float)(BitConverter.ToInt16(tmp, 0) * axl_res);
            currentData.Axl = currentAxl;

            // Magnetometer
            float[] currentMag = new float[3];
            Array.Copy(currentPayload, 12, tmp, 0, 2);
            currentMag[0] = (float)(BitConverter.ToInt16(tmp, 0) * mag_res);
            Array.Copy(currentPayload, 14, tmp, 0, 2);
            currentMag[1] = (float)(BitConverter.ToInt16(tmp, 0) * mag_res);
            Array.Copy(currentPayload, 16, tmp, 0, 2);
            currentMag[2] = (float)(BitConverter.ToInt16(tmp, 0) * mag_res);
            currentData.Mag = currentMag;

            return currentData;
        }

        /*
        Mitch_Data DataTypeOrientation(byte[] currentPayload, float gyr_res, float axl_res, float mag_res)
        {
            // 9DOF
            Mitch_Data currentData = new Mitch_Data();
            byte[] tmp = new byte[2];

            // Gyroscope
            float[] currentGyr = new float[3];
            Array.Copy(currentPayload, 0, tmp, 0, 2);
            currentGyr[0] = (float)(BitConverter.ToInt16(tmp, 0) * gyr_res) / 1000;
            Array.Copy(currentPayload, 2, tmp, 0, 2);
            currentGyr[1] = (float)(BitConverter.ToInt16(tmp, 0) * gyr_res) / 1000;
            Array.Copy(currentPayload, 4, tmp, 0, 2);
            currentGyr[2] = (float)(BitConverter.ToInt16(tmp, 0) * gyr_res) / 1000;
            currentData.Gyr = currentGyr;

            // Accelerometer
            float[] currentAxl = new float[3];
            Array.Copy(currentPayload, 6, tmp, 0, 2);
            currentAxl[0] = (float)(BitConverter.ToInt16(tmp, 0) * axl_res);
            Array.Copy(currentPayload, 8, tmp, 0, 2);
            currentAxl[1] = (float)(BitConverter.ToInt16(tmp, 0) * axl_res);
            Array.Copy(currentPayload, 10, tmp, 0, 2);
            currentAxl[2] = (float)(BitConverter.ToInt16(tmp, 0) * axl_res);
            currentData.Axl = currentAxl;

            // Magnetometer
            float[] currentMag = new float[3];
            Array.Copy(currentPayload, 12, tmp, 0, 2);
            currentMag[0] = (float)(BitConverter.ToInt16(tmp, 0) * mag_res);
            Array.Copy(currentPayload, 14, tmp, 0, 2);
            currentMag[1] = (float)(BitConverter.ToInt16(tmp, 0) * mag_res);
            Array.Copy(currentPayload, 16, tmp, 0, 2);
            currentMag[2] = (float)(BitConverter.ToInt16(tmp, 0) * mag_res);
            currentData.Mag = currentMag;

            return currentData;
        }
        */
    }
}
