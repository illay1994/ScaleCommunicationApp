namespace Services.Responds {
    public class WINFO {
        public WINFO(string deviceType, string serialNumber, string deviceName) {
            DeviceType = deviceType;
            SerialNumber = serialNumber;
            DeviceName = deviceName;
        }

        public WINFO() {
            DeviceType = "Not connected";
            SerialNumber = "Not connected";
            DeviceName = "Not connected";
        }

        public string DeviceType { get; }
        public string SerialNumber { get; }
        public string DeviceName { get; }
    }
}