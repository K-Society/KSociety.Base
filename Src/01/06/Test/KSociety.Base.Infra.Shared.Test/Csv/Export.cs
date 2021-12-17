using KSociety.Base.Infra.Shared.Csv;
using KSociety.Base.Infra.Shared.Test.Csv.Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace KSociety.Base.Infra.Shared.Test.Csv;

public class Export
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;
    private readonly List<Dto.TestClassPrivateSetter> _list = new List<TestClassPrivateSetter>(); 

    public Export()
    {
        _loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Trace);
        });
        _logger = _loggerFactory.CreateLogger<ReadCsvTest>();
        _list.Add(new TestClassPrivateSetter(Guid.NewGuid(), 1, "Pippo", "0.0.0.0", true, "Test"));
    }

    [Fact]
    public void ExportCsv()
    {
        var result = WriteCsv<Dto.TestClassPrivateSetter>.Export(_loggerFactory, @"C:\temp\Test.csv", _list);

        Assert.NotNull(result);

    }

    [Fact]
    public void ExportCsvPrivateSetter()
    {
        var result = WriteCsv<Dto.TestClassPrivateSetter>.Export(_loggerFactory, @"C:\temp\Test.csv", _list);

        Assert.NotNull(result);

    }

    [Fact]
    public void ExportCsvClassMap()
    {
        var result = WriteCsvClassMap<Dto.TestClassPrivateSetter, ClassMap.TestClassPrivateSetter>.Export(_loggerFactory, @"C:\temp\Test.csv", _list);

        Assert.NotNull(result);

    }
}