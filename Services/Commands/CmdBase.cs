using System;
using System.Linq;
using System.Threading;
using Services.Senders;

namespace Services.Commands {

    public abstract class CmdBase {
        public string CommandName { get; protected set; }
    }

    public abstract class Cmd : CmdBase {
        public Cmd(string commandName) {
            CommandName = commandName;
        }

        public bool Execute(ISender sender) {
            try {
                sender.Send($"{CommandName}\r\n");
                Thread.Sleep(100);
                return sender.ReadMessage().Replace("\r", "").Split('\n').Where(x => !string.IsNullOrEmpty(x)).ElementAt(1) == $"{CommandName} D";
            } catch (TimeoutException) {
                return false;
            }            
        }
    }
    
    public abstract class CmdBase<T> : CmdBase {
        public CmdBase(string commandName) {
            CommandName = commandName;
        }

        public T Send(ISender sender) {
            string result = sender.SendAndWait($"{CommandName}\r\n");
            return Parse(result);
        }

        protected abstract T Parse(string respond);
    }
}