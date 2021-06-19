namespace CSVToJSON.Services.Models
{
    public abstract class CSVBaseValue
    {
        public CSVBaseValue(string value)
        {
            RawValue = value;
        }

        public string RawValue { get; private set; }

        public abstract dynamic CastedValue { get; set; }
    }
}
