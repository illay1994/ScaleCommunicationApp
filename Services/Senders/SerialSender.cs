using System;
using System.IO.Ports;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Senders {
    public class SerialSender : ISender {
        
        private readonly SerialPort _sender = new SerialPort();
        private readonly string _port;
        
        public bool IsConnected => _sender.IsOpen;
        
        public SerialSender(string port) {
            _port = port;
        }
        
        public void Dispose() {
            _sender.Dispose();
        }

        
        public bool Connect() {
            try {
                _sender.PortName = _port;
                _sender.Open();
                return _sender.IsOpen;
            }
            catch {
                return false;
            }
        }

        public Task<bool> ConnectAsync() {
            return Task.Run(Connect);
        }

        public void Disconnect() {
            _sender.Close();
        }

        public void Send(string message) {
            if (!IsConnected) throw new InvalidOperationException("Sender is not connected.");
            if (string.IsNullOrEmpty(message)) return;
            _sender.Write(message);
        }

        public string SendAndWait(string message, int timeout = 1000) {
            if (!IsConnected) throw new InvalidOperationException("Sender is not connected.");
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));
            _sender.ReadTimeout = timeout;
            _sender.WriteTimeout = timeout;
            _sender.Write(message);
            Thread.Sleep(5);
            return ReadMessage(timeout);
        }
        
        public string ReadMessage(int timeout = 1000) {
            var buffer = new byte[_sender.ReadBufferSize];
            var readCount =_sender.Read(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, readCount);
            return message;
        }
    }
}