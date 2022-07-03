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
            //Log.LogMessage(MessageImportance.High, "TestBuild");
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
    }
}