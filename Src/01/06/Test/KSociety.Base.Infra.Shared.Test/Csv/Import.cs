using KSociety.Base.Infra.Shared.Csv;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using Xunit;

namespace KSociety.Base.Infra.Shared.Test.Csv
{
    public class Import
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        

        public Import()
        {
            _loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Trace);
            });
            _logger = _loggerFactory.CreateLogger<ReadCsvTest>();
        }

        [Fact]
        public void ImportCsv()
        {
            var result = ReadCsv<DtoTestClass>.Import(_loggerFactory, Path.Combine(
                Directory.GetCurrentDirectory(), "Csv", "DtoTestClass.csv"));

            Assert.NotNull(result);


            var length = result.Count();

            var row = result.FirstOrDefault();
            const int expected = 2;
            const string expectedName = "Pippo";
            Assert.Equal(expected, length);
            Assert.Equal(expectedName, row.Name);
        }

        [Fact]
        public void ImportCsvPrivateSetter()
        {
            var result = ReadCsv<Dto.TestClassPrivateSetter>.Import(_loggerFactory, Path.Combine(
                Directory.GetCurrentDirectory(), "Csv", "DtoTestClass.csv"));

            Assert.NotNull(result);


            var length = result.Count();

            var row = result.FirstOrDefault();
            const int expected = 2;
            const string expectedName = "Pippo";
            Assert.Equal(expected, length);
            Assert.Equal(expectedName, row.Name);
        }

        [Fact]
        public void ImportCsvClassMap()
        {
            var result = ReadCsvClassMap<DtoTestClass, ClassMap.DtoClassMap>.Import(_loggerFactory, Path.Combine(
                Directory.GetCurrentDirectory(), "Csv", "DtoTestClass.csv"));

            Assert.NotNull(result);


            var length = result.Count();

            var row = result.FirstOrDefault();
            const int expected = 2;
            const string expectedName = "Pippo";
            Assert.Equal(expected, length);
            Assert.Equal(expectedName, row.Name);
        }
    }
}
