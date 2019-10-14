using System.Linq;
using Services.Responds;

namespace Services.Commands {
    public class OTCmd : CmdBase<OT> {
        public OTCmd() : base("OT") {
        }

        protected override OT Parse(string respond) {
            string[] divided = respond.Replace("OT", "").Replace("\r\n", "")
                .Split(' ').Where(y => !string.IsNullOrEmpty(y)).ToArray();
            return new OT(double.Parse(divided[0].Replace(".", ",")), divided[0], divided[1]);
        }
    }
}