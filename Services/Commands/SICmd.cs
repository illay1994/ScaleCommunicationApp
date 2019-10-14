using System.Linq;
using Services.Responds;

namespace Services.Commands {
    public class SICmd : CmdBase<SI> {
        public SICmd() : base("SI") {}


        protected override SI Parse(string respond) {
            bool isNegative = respond.Contains("-");
            bool isStable = !respond.Contains("?");
            string[] values = respond.Replace("\r\n", "").Replace("SI", "")
                .Replace("?", "").Replace("-", "").Trim().Split(' ').Where(x => !string.IsNullOrEmpty(x)).ToArray();

            return new SI((isNegative ? "-" : "") + values[0], double.Parse(values[0].Replace(".", ",")) * (isNegative ? -1 : 1),  values[1], isStable, (byte)(values[0].Length - values[0].IndexOf(".") - 1));
        }
    }
}