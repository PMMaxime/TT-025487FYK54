using System;

namespace CSVToJSON.Services.CSVParser.Exceptions
{
    public class CSVParserException : Exception
    {
        public CSVParserException()
        {

        }

        public CSVParserException(string message) : base(message)
        {

        }

    }
}
