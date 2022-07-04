using KSociety.Base.Utility.Class.CodeDom;
using KSociety.Base.Utility.Class.Csv;
using Microsoft.Extensions.Logging;
using Xunit;

namespace KSociety.Base.InfraSub.Test
{

    public class CodeDomService
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly KSociety.Base.Utility.Class.CodeDom.CodeDomService _codeDomService;
        private readonly ClassGenerator[] _classGenerators;

        public CodeDomService()
        {
            _loggerFactory = Microsoft.Extensions.Logging.LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Trace);
            });

            _codeDomService = new KSociety.Base.Utility.Class.CodeDom.CodeDomService();

            _classGenerators = ReadCsvClassMap<KSociety.Base.Utility.Class.CodeDom.ClassGenerator, KSociety.Base.Utility.Class.CodeDom.ClassMap.ClassGenerator>.Read(_loggerFactory, "TestDto");
            ;
        }

        [Fact]
        public void GenerateClass()
        {
            //_codeDomService.AddUsing("System.Collections.Generic");
            //_codeDomService.AddCodeNamespace("Dnw.Entity");
            //_codeDomService.AddBaseClass(new []{"Pippo", "Pippo"});

            _codeDomService.Generator(_classGenerators);
            _codeDomService.GenerateClass("Test.cs");
        }

    }
}
