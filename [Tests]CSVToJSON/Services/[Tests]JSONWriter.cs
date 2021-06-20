using CSVToJSON.Services.CSVParser.Interfaces;
using CSVToJSON.Services.CSVParser.Models;
using CSVToJSON.Services.JSONWriter;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace _Tests_CSVToJSON
{
    public class _Tests_JSONWriter
    {
        private ILogger<JSONWriter> mockedJsonWriterLogger = new Mock<ILogger<JSONWriter>>().Object;

        [Fact]
        public void GetStructuredJsonDataFromCsvData()
        {
            var csvParserMock = new Mock<ICSVParser>();

            csvParserMock.Setup(c => c.ParseCsvFile("", ',')).Returns(new List<List<CSVBaseValue>>() {
                new List<CSVBaseValue>() {new CSVStringValue("a"), new CSVStringValue("b") },
                new List<CSVBaseValue>() {new CSVStringValue("e"), new CSVIntValue(1) }
            });

            var jsonWriter = new JSONWriter(mockedJsonWriterLogger);

            var csvData = csvParserMock.Object.ParseCsvFile("", ',');

            var result = jsonWriter.GetStructuredJsonDataFromCsvData(csvData);

            Assert.Equal("[{\"a\":\"e\",\"b\":1}]", JsonConvert.SerializeObject(result));
        }

        [Fact]
        public void GetFlatJsonDataFromCsvData()
        {
            var csvParserMock = new Mock<ICSVParser>();

            csvParserMock.Setup(c => c.ParseCsvFile("", ',')).Returns(new List<List<CSVBaseValue>>() {
                new List<CSVBaseValue>() {new CSVStringValue("a"), new CSVStringValue("b") },
                new List<CSVBaseValue>() {new CSVStringValue("e"), new CSVIntValue(1) }
            });

            var jsonWriter = new JSONWriter(mockedJsonWriterLogger);

            var csvData = csvParserMock.Object.ParseCsvFile("", ',');

            var result = jsonWriter.GetFlatJsonDataFromCsvData(csvData);

            Assert.Equal("[[\"a\",\"b\"],[\"e\",1]]", JsonConvert.SerializeObject(result));
        }

    }
}
