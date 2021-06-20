namespace CSVToJSON.Services.Models
{
    public class CSVIntValue : CSVBaseValue
    {
        private int castedValue;
        public CSVIntValue(int value) : base(value.ToString())
        {
            CastedValue = value;
        }

        public override dynamic CastedValue { get { return castedValue; } set { castedValue = value; } }
    }
}
