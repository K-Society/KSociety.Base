using KSociety.Base.Infra.Shared.Csv;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using Xunit;

namespace KSociety.Base.Infra.Shared.Test.Csv
{
    public class ReadCsvTest
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _logger;
        public ReadCsvTest()
        {
            _loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Trace);
            });
            _logger = _loggerFactory.CreateLogger<ReadCsvTest>();
        }

        [Fact]
        public void ReadCsv()
        {
            var result = ReadCsv<DtoTestClass4>.Read(_loggerFactory, "DtoTestClass");

            Assert.NotNull(result);


            var length = result.Length;

            var row = result.FirstOrDefault();
            const int expected = 2;
            const string expectedName = "SuperPippo";
            Assert.Equal(expected, length);
            Assert.Equal(expectedName, row.Name);
        }

        [Fact]
        public void WriteCsv()
        {
            //var result = WriteCsv<DtoTestClass4>.Write(_loggerFactory, "DtoTestClass");

            //Assert.NotNull(result);

            var test = WriteCsv<DtoTestClass4>.Export(_loggerFactory, @"C:\temp\Test.csv", new []{new DtoTestClass4(Guid.NewGuid(), 0, "Test", "0.0.0.0", true, "ahhuhh")});

            Assert.True(test);
        }
    }
}
