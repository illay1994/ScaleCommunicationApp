using System;
using System.Threading.Tasks;

namespace Services.Senders {
    public interface ISender : IDisposable {
        bool IsConnected { get; }
        bool Connect();
        Task<bool> ConnectAsync();
        void Disconnect();
        void Send(string message);
        string SendAndWait(string message, int timeout = 1000);
        string ReadMessage(int timeout = 1000);
    }
}