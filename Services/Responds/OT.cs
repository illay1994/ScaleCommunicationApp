namespace Services.Responds {
    public class OT {
        public OT(double value, string strValue, string unit) {
            Value = value;
            StrValue = strValue;
            Unit = unit;
        }

        public double Value { get; }
        public string StrValue { get; }
        public string Unit { get; }
    }
}