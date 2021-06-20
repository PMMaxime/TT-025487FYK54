namespace CSVToJSON.Services.Models
{
    public class CSVStringValue : CSVBaseValue
    {
        private string castedValue;
        public CSVStringValue(string value) : base(value)
        {
            CastedValue = value;
        }

        public override dynamic CastedValue { get { return castedValue; } set { castedValue = value; } }
    }
}
