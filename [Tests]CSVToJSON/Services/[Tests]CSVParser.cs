using CSVToJSON.Services.Interfaces;
using System;
using System.IO;
using Xunit;

namespace _Tests_CSVToJSON
{
    public class _Tests_CSVParser
    {

        private readonly ICSVParser _csvParser;

        public _Tests_CSVParser(ICSVParser csvParser)
        {
            _csvParser = csvParser;
        }

        [Fact]
        public void GetsTextContentFromFilePath()
        {
            using StreamWriter outputFile = new StreamWriter(Path.Combine(@"./", "testcsv.csv"));
            outputFile.WriteLine("Text");

            var fileText = _csvParser.getFileText("./testcsv.csv");
            Assert.Equal(fileText, "Text");

        }
    }
}
