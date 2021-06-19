using _Tests_CSVToJSON.Utils.Interfaces;
using CSVToJSON.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using Xunit;

namespace _Tests_CSVToJSON
{
    public class _Tests_CSVParser
    {
        private ILogger<CSVParser> mockedLogger = new Mock<ILogger<CSVParser>>().Object;

        [Fact]
        public void ParseCsvFile_2_Rows_2_Cols_Only_Text_Values()
        {
            var csvContent =
@"a,b,
e,f";
            var fileUtils = new Mock<IFileUtils>();
            fileUtils.Setup(c => c.ReadAllText("")).Returns(csvContent);

            var csvParser = new CSVParser(mockedLogger, fileUtils.Object);

            var result = csvParser.ParseCsvFile("");

            Assert.Equal(2, result.Count());
            Assert.Equal("a", result.First().ElementAt(0));
            Assert.Equal("b", result.First().ElementAt(1));

            Assert.Equal("e", result.ElementAt(1).ElementAt(0));
            Assert.Equal("f", result.ElementAt(1).ElementAt(1));
        }
    }
}
