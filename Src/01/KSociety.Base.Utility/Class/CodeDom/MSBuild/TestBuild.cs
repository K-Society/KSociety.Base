using KSociety.Base.Utility.Class.Csv;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Linq;

namespace KSociety.Base.Utility.Class.CodeDom.MSBuild
{
    public class TestBuild : Task
    {
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

        public TestBuild()
        {
            _codeDomService = new CodeDomService();
            //_classGenerators = ReadCsvClassMap<ClassGenerator, ClassMap.ClassGenerator>.Read(loggerFactory, "TestDto");
            Log.LogMessage(MessageImportance.High, "TestBuild");
        }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "TestBuild Execute...");

            try
            {
                if (SettingFiles.Any())
                {
                    _classGenerators =
                        ReadCsvClassMap<ClassGenerator, ClassMap.ClassGenerator>.Read(Log, SettingFiles[0]);
                    if (_classGenerators.Any())
                    {
                        _codeDomService.Generator(_classGenerators);
                        _codeDomService.GenerateClass("Test.cs");
                    }
                }
                else
                {
                    Log?.LogMessage(MessageImportance.High, "No file.");
                }
            }
            catch (Exception ex)
            {
                Log?.LogErrorFromException(ex, true);
                return false;
            }
            return !Log.HasLoggedErrors;
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
        //    var values = new Dictionary<string, object>();
        //    foreach (var item in SettingFiles)
        //    {
        //        int lineNumber = 0;

        //        var settingFile = item.GetMetadata("FullPath");
        //        foreach (string line in File.ReadLines(settingFile))
        //        {
        //            lineNumber++;

        //            var lineParse = line.Split(':');
        //            if (lineParse.Length != 3)
        //            {
        //                Log.LogError(subcategory: null,
        //                    errorCode: "APPS0001",
        //                    helpKeyword: null,
        //                    file: settingFile,
        //                    lineNumber: lineNumber,
        //                    columnNumber: 0,
        //                    endLineNumber: 0,
        //                    endColumnNumber: 0,
        //                    message: "Incorrect line format. Valid format prop:type:defaultvalue");
        //                return (false, null);
        //            }
        //            var value = GetValue(lineParse[1], lineParse[2]);
        //            if (!value.Item1)
        //            {
        //                return (value.Item1, null);
        //            }

        //            values[lineParse[0]] = value.Item2;
        //        }
        //    }
        //    return (true, values);
        //}

        //private void LogFormat(string message, params object[] args)
        //{
        //    if (this.BuildEngine != null)
        //    {
        //        this.Log.LogMessage(message, args);
        //    }
        //    else
        //    {
        //        Console.WriteLine(message);
        //        string[] lines = { message };
        //        System.IO.File.WriteAllLines(@"C:\JOB\ListView.txt", lines);
        //    }
        //}
    }
}