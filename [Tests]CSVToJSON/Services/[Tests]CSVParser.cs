using CSVToJSON.Services.CSVParser;
using CSVToJSON.Services.CSVParser.Exceptions;
using CSVToJSON.Services.CSVParser.Models;
using CSVToJSON.Utils.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace _Tests_CSVToJSON
{
    public class _Tests_CSVParser
    {
        private ILogger<CSVParser> mockedLogger = new Mock<ILogger<CSVParser>>().Object;

        [Fact]
        public void ParseCsvFile_No_Typing_No_Quotation()
        {
            var csvContent =
@"a,b
e,f";
            var fileUtils = new Mock<IFileUtils>();
            fileUtils.Setup(c => c.ReadAllText("")).Returns(csvContent);

            var csvParser = new CSVParser(mockedLogger, fileUtils.Object);

            var result = csvParser.ParseCsvFile("");

            Assert.Equal(2, result.Count());
            Assert.Equal("a", result.First().ElementAt(0).CastedValue);
            Assert.Equal("b", result.First().ElementAt(1).CastedValue);

            Assert.Equal("e", result.ElementAt(1).ElementAt(0).CastedValue);
            Assert.Equal("f", result.ElementAt(1).ElementAt(1).CastedValue);
        }

        [Fact]
        public void ParseCsvFile_Explicit_Typing_No_Quotation()
        {
            var csvContent =
@"1,b
e,2";
            var fileUtils = new Mock<IFileUtils>();
            fileUtils.Setup(c => c.ReadAllText("")).Returns(csvContent);

            var csvParser = new CSVParser(mockedLogger, fileUtils.Object);

            var result = csvParser.ParseCsvFile("");

            Assert.Equal(2, result.Count());

            Assert.Equal(1, result.First().ElementAt(0).CastedValue);
            Assert.IsType<CSVIntValue>(result.First().ElementAt(0));

            Assert.Equal("b", result.First().ElementAt(1).CastedValue);
            Assert.IsType<CSVStringValue>(result.First().ElementAt(1));

            Assert.Equal("e", result.ElementAt(1).ElementAt(0).CastedValue);
            Assert.IsType<CSVStringValue>(result.ElementAt(1).ElementAt(0));

            Assert.Equal(2, result.ElementAt(1).ElementAt(1).CastedValue);
            Assert.IsType<CSVIntValue>(result.ElementAt(1).ElementAt(1));
        }

        [Fact]
        public void ParseCsvFile_Mixed_Typing_1_Quotation()
        {
            var csvContent =
@"1997,Ford,E350,""Super,
luxurious truck""";

            var fileUtils = new Mock<IFileUtils>();
            fileUtils.Setup(c => c.ReadAllText("")).Returns(csvContent);

            var csvParser = new CSVParser(mockedLogger, fileUtils.Object);

            var result = csvParser.ParseCsvFile("");

            Assert.Single(result);
            Assert.Equal("Super,luxurious truck", result.First().ElementAt(3).CastedValue);
        }

        [Fact]
        public void ParseCsvFile_Inconsistent_column_count()
        {
            var csvContent =
@"a,b,c
d,e
f,g,h,i";

            var fileUtils = new Mock<IFileUtils>();
            fileUtils.Setup(c => c.ReadAllText("")).Returns(csvContent);

            var csvParser = new CSVParser(mockedLogger, fileUtils.Object);

            Exception receivedException = null;

            try
            {
                var result = csvParser.ParseCsvFile("");

            }
            catch (Exception e)
            {
                receivedException = e;
            }

            Assert.NotNull(receivedException);
            Assert.IsType<CSVParserException>(receivedException);
            Assert.Equal(@"Invalid number of values at line 2 : 2 values found instead of 3 (top line being the reference). The CSV file might be inconsistent in its format.

Invalid number of values at line 3 : 4 values found instead of 3 (top line being the reference). The CSV file might be inconsistent in its format.", receivedException.Message);

        }
    }
}
