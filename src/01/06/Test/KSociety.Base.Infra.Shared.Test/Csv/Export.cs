// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Test.Csv;
using KSociety.Base.Infra.Shared.Csv;
using Dto;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

public class Export
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;
    private readonly List<TestClassPrivateSetter> _list = new List<TestClassPrivateSetter>(); 

    public Export()
    {
        this._loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Trace);
        });
        this._logger = this._loggerFactory.CreateLogger<ReadCsvTest>();
        this._list.Add(new TestClassPrivateSetter(Guid.NewGuid(), 1, "Pippo", "0.0.0.0", true, "Test"));
    }

    [Fact]
    public void ExportCsv()
    {
        var result = WriteCsv<TestClassPrivateSetter>.Export(this._loggerFactory, @"C:\temp\Test.csv", this._list);

        Assert.NotNull(result);

    }

    [Fact]
    public void ExportCsvPrivateSetter()
    {
        var result = WriteCsv<TestClassPrivateSetter>.Export(this._loggerFactory, @"C:\temp\Test.csv", this._list);

        Assert.NotNull(result);

    }

    [Fact]
    public void ExportCsvClassMap()
    {
        var result = WriteCsvClassMap<TestClassPrivateSetter, ClassMap.TestClassPrivateSetter>.Export(this._loggerFactory, @"C:\temp\Test.csv", this._list);

        Assert.NotNull(result);

    }
}
