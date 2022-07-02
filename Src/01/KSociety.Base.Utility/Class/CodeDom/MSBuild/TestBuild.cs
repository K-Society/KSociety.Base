using KSociety.Base.Utility.Class.Csv;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Extensions.Logging;
using System;

namespace KSociety.Base.Utility.Class.CodeDom.MSBuild
{
    public class TestBuild : Task
    {
        private readonly ILoggerFactory _loggerFactory;
        //private readonly ILogger<TestBuild> _logger;
        private readonly CodeDomService _codeDomService;
        private ClassGenerator[] _classGenerators;

        //The name of the class which is going to be generated
        [Required]
        public string SettingClassName { get; set; }

        //The name of the namespace where the class is going to be generated
        [Required]
        public string SettingNamespaceName { get; set; }

        //List of files which we need to read with the defined format: 'propertyName:type:defaultValue' per line
        [Required]
        public ITaskItem[] SettingFiles { get; set; }

        //The filename where the class was generated
        [Output]
        public string ClassNameFile { get; set; }

        //private int number1;

        //[Required]
        //public int Number1
        //{
        //    get { return number1; }
        //    set { number1 = value; }
        //}

        //private int number2;

        //[Required]
        //public int Number2
        //{
        //    get { return number2; }
        //    set { number2 = value; }
        //}

        //private int sum;

        //[Output]
        //public int Sum
        //{
        //    get { return sum; }
        //    set { sum = value; }
        //}

        public TestBuild()
        {
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            //_logger = loggerFactory.CreateLogger<TestBuild>();

            _codeDomService = new CodeDomService();
            Log.LogMessageFromText("TestBuild", MessageImportance.High);
            //_classGenerators = ReadCsvClassMap<ClassGenerator, ClassMap.ClassGenerator>.Read(loggerFactory, "TestDto");
        }

        public override bool Execute()
        {
            Log.LogMessageFromText("Execute", MessageImportance.High);
            //ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            //{
            //    builder.AddConsole();
            //    builder.SetMinimumLevel(LogLevel.Trace);
            //});

            //_logger = loggerFactory.CreateLogger<TestBuild>();

            //_codeDomService = new CodeDomService();

            _classGenerators = ReadCsvClassMap<ClassGenerator, ClassMap.ClassGenerator>.Read(_loggerFactory, "TestDto");

            try
            {
                _codeDomService.Generator(_classGenerators);
                _codeDomService.GenerateClass("Test.cs");
            }
            catch (ArithmeticException e)
            {
                //_logger.LogError(e, "Execute: ");
                Log.LogErrorFromException(e, true);
                return false;
            }
            return true;
        }

        //public override bool Execute()
        //{
        //    //Read the input files and return a IDictionary<string, object> with the properties to be created.
        //    //Any format error it will return not succeed and Log.LogError properly
        //    var (success, settings) = ReadProjectSettingFiles();
        //    if (!success)
        //    {
        //        return !Log.HasLoggedErrors;
        //    }
        //    //Create the class based on the Dictionary
        //    success = CreateSettingClass(settings);

        //    return !Log.HasLoggedErrors;
        //}

        //private (bool, IDictionary<string, object>) ReadProjectSettingFiles()
        //{
        //var values = new Dictionary<string, object>();
        //foreach (var item in SettingFiles)
        //{
        //    int lineNumber = 0;

        //    var settingFile = item.GetMetadata("FullPath");
        //    foreach (string line in File.ReadLines(settingFile))
        //    {
        //        lineNumber++;

        //        var lineParse = line.Split(':');
        //        if (lineParse.Length != 3)
        //        {
        //            Log.LogError(subcategory: null,
        //                errorCode: "APPS0001",
        //                helpKeyword: null,
        //                file: settingFile,
        //                lineNumber: lineNumber,
        //                columnNumber: 0,
        //                endLineNumber: 0,
        //                endColumnNumber: 0,
        //                message: "Incorrect line format. Valid format prop:type:defaultvalue");
        //            return (false, null);
        //        }
        //        var value = GetValue(lineParse[1], lineParse[2]);
        //        if (!value.Item1)
        //        {
        //            return (value.Item1, null);
        //        }

        //        values[lineParse[0]] = value.Item2;
        //    }
        //}
        //return (true, values);
        //}
    }
}