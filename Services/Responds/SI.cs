namespace Services.Responds {
    public class SI {
        public SI(string strValue, double? value, string unit, bool? isStable, byte precision) {
            StrValue = strValue;
            Value = value;
            Unit = unit;
            IsStable = isStable;
            Precision = precision;
        }

        public SI() {
            
        }
        public string StrValue { get; }
        public double? Value { get; } = null;
        public string Unit { get; } = null;
        public bool? IsStable { get; } = null;
        public byte Precision { get; } = 0;

        public override string ToString() {
            return StrValue;
        }
    }
}