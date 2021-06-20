using CSVToJSON.Services.CSVParser;
using CSVToJSON.Services.JSONWriter;
using CSVToJSON.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
using Xunit;

namespace _Tests_CSVToJSON.e2e
{
    public class _Tests_EndToEnd
    {
        private ILogger<CSVParser> mockedLogger = new Mock<ILogger<CSVParser>>().Object;
        private ILogger<JSONWriter> mockedJsonWriterLogger = new Mock<ILogger<JSONWriter>>().Object;

        [Fact]
        public void ParseCsvFile_No_Typing_No_Quotation()
        {

            var csvParser = new CSVParser(mockedLogger, new FileUtils());
            var jsonWriter = new JSONWriter(mockedJsonWriterLogger);
            var csvContent =
@"Year,Car,Model,Description
1997,Ford,E350,""Super,
luxurious truck""
1997,Ford,E350,""Go get
one nowthey are going
fast""
1997,Ford,E350,""Super,
""""luxurious"""" truck""";

            File.WriteAllText("../../../Resources/teste2efile.csv", csvContent);

            var csvPasedData = csvParser.ParseCsvFile("../../../Resources/teste2efile.csv");

            jsonWriter.WriteCsvDataToJsonFile(csvPasedData, "../../../Resources/teste2efile.json", true);

            var result = File.ReadAllText("../../../Resources/teste2efile.json");

            Assert.Equal(@"[{""Year"":1997,""Car"":""Ford"",""Model"":""E350"",""Description"":""Super,luxurious truck""},{""Year"":1997,""Car"":""Ford"",""Model"":""E350"",""Description"":""Go getone nowthey are goingfast""},{""Year"":1997,""Car"":""Ford"",""Model"":""E350"",""Description"":""Super,\""luxurious\"" truck""}]", result);

        }
    }
}
