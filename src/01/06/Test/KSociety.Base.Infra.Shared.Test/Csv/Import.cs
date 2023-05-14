using KSociety.Base.Infra.Shared.Csv;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace KSociety.Base.Infra.Shared.Test.Csv;

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
        var result = ReadCsv<Dto.TestClass>.Import(_loggerFactory, Path.Combine(
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
    public async Task ImportCsvAsync()
    {
        var result = ReadCsv<Dto.TestClass>.ImportAsync(_loggerFactory, Path.Combine(
            Directory.GetCurrentDirectory(), "Csv", "DtoTestClass.csv"));

        Assert.NotNull(result);

        CancellationToken cancellationToken = new CancellationToken();
        var length = 0;
        Dto.TestClass row = null;
        await foreach (var item in result.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            if (length == 0)
            {
                row = item;
            }
            length++;
        }


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
    public void ImportCsvParameterlessConstructor()
    {
        var result = ReadCsv<Dto.TestClassParameterlessConstructor>.Import(_loggerFactory, Path.Combine(
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
        var result = ReadCsvClassMap<Dto.TestClass, Csv.ClassMap.TestClass>.Import(_loggerFactory, Path.Combine(
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
    public void ImportCsvClassMapPrivateSetter()
    {
        var result = ReadCsvClassMap<Dto.TestClassPrivateSetter, ClassMap.TestClassPrivateSetter>.Import(_loggerFactory, Path.Combine(
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
    public void ImportCsvClassMapParameterlessConstructor()
    {
        var result = ReadCsvClassMap<Dto.TestClassParameterlessConstructor, Csv.ClassMap.TestClassParameterlessConstructor>.Import(_loggerFactory, Path.Combine(
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