using System;
using System.Collections.Generic;
using System.ServiceProcess;

namespace KSociety.Base.Srv.Shared.Class
{
    public partial class KbServiceBase : ServiceBase
    {
        protected readonly bool DebugFlag;

        protected KbServiceBase()
        {

        }

        protected KbServiceBase(IEnumerable<string> args)
        {
            var list = new List<string>(args);
            DebugFlag = list.Contains("/debug");
        }

        public void RunAsConsole(string[] args)
        {
            OnStart(args);
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            Stop();
        }
    }
}
