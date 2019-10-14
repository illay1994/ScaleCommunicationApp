using System.Collections.Generic;
using System.Linq;
using Services.Responds;

namespace Services.Commands {
    public class WINFOCmd : CmdBase<WINFO> {
        
        public WINFOCmd() : base("WINFO") {}

        protected override WINFO Parse(string respond) {
            IDictionary<string,string> keyValue = respond.Replace("WINFO", "").Replace("\r\n", "").
                Split('<').Skip(1).ToDictionary(x => x.Split('=')[0], y => y.Split('=')[1].Replace(">", ""));

            return new WINFO(keyValue["DEVICE_TYPE"], keyValue["SERIAL_NUMBER"], keyValue["DEVICE_NAME"]);
        }
    }
}