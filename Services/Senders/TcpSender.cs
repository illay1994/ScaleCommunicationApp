using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Senders {
    public class TcpSender : ISender {
        
        private readonly TcpClient _sender = new TcpClient();
        private readonly string _ipAddress;
        private readonly int _port;
        
        public bool IsConnected => _sender.Connected;
        
        public TcpSender(string ipAddress, int port) {
            _ipAddress = ipAddress;
            _port = port;
        }
        
        public void Dispose() {
            _sender.Dispose();
        }

        
        public bool Connect() {
            try {
                _sender.Connect(_ipAddress, _port);
                return _sender.Connected;
            }
            catch {
                return false;
            }
        }

        public Task<bool> ConnectAsync() {
            return Task.Run(Connect);
        }

        public void Disconnect() {
            _sender.Client.Disconnect(true);
        }

        public void Send(string message) {
            if (!IsConnected) throw new InvalidOperationException("Sender is not connected.");
            if (string.IsNullOrEmpty(message)) return;
            _sender.Client.Send(Encoding.UTF8.GetBytes(message));
        }

        public string SendAndWait(string message, int timeout = 1000) {
            if (!IsConnected) throw new InvalidOperationException("Sender is not connected.");
            if (string.IsNullOrEmpty(message)) throw new ArgumentNullException(nameof(message));
            _sender.ReceiveTimeout = timeout;
            _sender.SendTimeout = timeout;
            _sender.Client.Send(Encoding.UTF8.GetBytes(message));
            Thread.Sleep(5);
            return ReadMessage(timeout);
        }
        
        public string ReadMessage(int timeout = 1000) {
            var buffer = new byte[_sender.ReceiveBufferSize];
            var stream =_sender.GetStream();
            stream.Read(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer);
            return message;
        }
    }
}