// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Infra.Shared.Test.Csv;
using KSociety.Base.Infra.Shared.Csv;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

public class ReadCsvTest
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;
    public ReadCsvTest()
    {
        this._loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Trace);
        });
        this._logger = this._loggerFactory.CreateLogger<ReadCsvTest>();
    }

    [Fact]
    public void ReadCsv()
    {
        var result = ReadCsv<DtoTestClass4>.Read(this._loggerFactory, "DtoTestClass");

        Assert.NotNull(result);


        var length = result.Length;

        var row = result.FirstOrDefault();
        const int expected = 2;
        const string expectedName = "SuperPippo";
        Assert.Equal(expected, length);
        Assert.Equal(expectedName, row.Name);
    }

    [Fact]
    public void ImportCsv()
    {
        var result = ReadCsv<DtoTestClass>.Import(this._loggerFactory, Path.Combine(
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
    public void WriteCsv()
    {
        //var result = WriteCsv<DtoTestClass4>.Write(_loggerFactory, "DtoTestClass");

        //Assert.NotNull(result);
        //var item1 = new DtoTestClass4(1, /*Guid.NewGuid(),*/ 2, "Test", "0.0.0.0", true, "ahh");
        //var item2 = new DtoTestClass4(2, /*Guid.NewGuid(),*/ 2, "Test", "0.0.0.0", true, "ahh");
        var array = "yyy";//new char[] {'i', 'i'};
        var item1 = new DtoTestClass5( array);
        //var item2 = new DtoTestClass5( array);
        var list = new List<DtoTestClass5> {item1/*, item2*/};
        var test = WriteCsv<DtoTestClass5>.Export(this._loggerFactory, @"C:\temp\Test.csv", list);

        Assert.True(test);
    }
}
