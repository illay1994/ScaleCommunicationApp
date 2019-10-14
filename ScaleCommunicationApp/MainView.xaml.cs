using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ScaleCommunicationApp.Annotations;
using Services.Commands;
using Services.Responds;
using Services.Senders;

namespace ScaleCommunicationApp {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainView : INotifyPropertyChanged {
        private string _ipAddress = "192.168.4.1";
        private string _status = "Disconnected";
        private ISender _sender;
        private WINFO _scaleInfo = new WINFO();
        private SI _mass;
        /// <summary>
        /// Padlock for sync threads.
        /// </summary>
        private readonly object _oPadLock = new object();

        private CancellationTokenSource _ctsMassThread;
        private OT _tare;
        
        
        /// <summary>
        /// Information about scale.
        /// </summary>
        public WINFO ScaleInfo {
            get => _scaleInfo;
            set {
                if (Equals(value, _scaleInfo)) return;
                _scaleInfo = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Current mass.
        /// </summary>
        public SI Mass {
            get => _mass;
            set {
                if (Equals(value, _mass)) return;
                _mass = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Currant tare.
        /// </summary>
        public OT Tare {
            get => _tare;
            set {
                if (Equals(value, _tare)) return;
                _tare = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Ip address of scale.
        /// </summary>
        public string IpAddress {
            get => _ipAddress;
            set {
                if (value == _ipAddress) return;
                _ipAddress = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Status of connection.
        /// </summary>
        public string Status {
            get => _status;
            set {
                if (value == _status) return;
                _status = value;
                OnPropertyChanged();
            }
        }
        
        /// <summary>
        /// Ctor
        /// </summary>
        public MainView() {
            InitializeComponent();
            Stability.Visibility = Visibility.Collapsed;
            DataContext = this;
        }
        
        /// <summary>
        /// Connect button click event.
        /// </summary>
        private async void btnConnect_OnClick (object sender, RoutedEventArgs e) {
            if (!Regex.IsMatch(IpAddress,
                @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$")) {
                Status = "Invalid Ip address format";
                return;
            }
            if (_sender == null || !_sender.IsConnected) {
                _sender = new TcpSender(IpAddress, 4001);
                Status = "Connecting...";
                if (await _sender.ConnectAsync()) {
                    InitializeConnection();
                }
                else {
                    Status = "Error";
                }
            }
            else {
                PerformDisconnection();
            }
        }
        /// <summary>
        /// Tare button click event.
        /// </summary>
        private async void BtnTare_OnClick(object sender, RoutedEventArgs e) {
            await Task.Run(() => {
                lock (_oPadLock) {
                    new TCmd().Execute(_sender);
                }
            });
        }
        /// <summary>
        /// Zero button click event.
        /// </summary>
        private async void BtnZero_OnClick(object sender, RoutedEventArgs e) {
            await Task.Run(() => {
                lock (_oPadLock) {
                    new ZCmd().Execute(_sender);
                }
            });
        }
        /// <summary>
        /// Performing operation after established connection with scale.
        /// </summary>
        private void InitializeConnection() {
            _ctsMassThread = new CancellationTokenSource();
            Status = "Connected";
            btnConnect.Content = "Disconnect";
            GetScaleInformation();
            ChangePanelVisibility(Visibility.Visible);
            Task.Run(MassUpdater);
            txtIpAddress.IsEnabled = false;
        }
        /// <summary>
        /// Performing disconnection with scale.
        /// </summary>
        private void PerformDisconnection() {
            _ctsMassThread.Cancel();
            Mass = null;
            DisposeSender();
            ChangePanelVisibility(Visibility.Hidden);
            Status = "Disconnected";
            btnConnect.Content = "Connect";
            txtIpAddress.IsEnabled = true;
        }
        
        /// <summary>
        /// Changing <see cref="Visibility"/> of panels.
        /// </summary>
        /// <param name="visibility">Visibility to set.</param>
        private void ChangePanelVisibility(Visibility visibility) {
            Stability.Visibility = visibility;
            DisplayPanel.Visibility = visibility;
            OperationPanel.Visibility = visibility;
        }
        
        /// <summary>
        /// Disposing sender.
        /// </summary>
        private void DisposeSender() {
            _sender.Dispose();
            _sender = null;
        }

        /// <summary>
        /// Updating mass and tare value in loop.
        /// </summary>
        private void MassUpdater() {
            while (!_ctsMassThread.IsCancellationRequested) {
                lock (_oPadLock) {
                    UpdateValue<SI, SICmd>(ref _mass);
                    Thread.Sleep(10);
                    UpdateValue<OT, OTCmd>(ref _tare);
                }
                Dispatcher?.Invoke(() => Stability.Visibility = Mass.IsStable.HasValue && Mass.IsStable.Value ? Visibility.Visible : Visibility.Hidden);
                OnPropertyChanged("");
            }
        }

        private void UpdateValue<T, I>(ref T updateItem) where I : CmdBase<T> {
            T item = default;
            try {
                item = Activator.CreateInstance<I>().Send(_sender);
            }
            catch {
                // ignored
            }

            if (item != null) updateItem = item;
        }
        
        /// <summary>
        /// Getting information from scale.
        /// </summary>
        private void GetScaleInformation() {
            try {
                ScaleInfo = new WINFOCmd().Send(_sender);
            }
            catch (TimeoutException) {
            }
        }
        /// <summary>
        /// Occurs when property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Invoker for PropertyChanged event.
        /// </summary>
        /// <param name="propertyName"></param>
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
