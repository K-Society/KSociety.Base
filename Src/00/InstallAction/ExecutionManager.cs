using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using WixSharp;

namespace InstallAction
{
    public static class ExecutionManager
    {
        [CustomAction]
        public static ActionResult ExecuteCommand(Session session)
        {
            return session.HandleErrors(() =>
            {
                var data = session.CustomActionData;
                var exeName = data["ExeName"];
                var args = data["Args"];

                var process = new Process
                {
                    StartInfo =
                    {
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        FileName = exeName,
                        Arguments = args
                    }
                };

                process.Start();
                process.WaitForExit();
            });
        }

        [CustomAction]
        public static ActionResult ExecuteCommandNoArgs(Session session)
        {
            return session.HandleErrors(() =>
            {
                var data = session.CustomActionData;
                var exeName = data["ExeName"];

                var process = new Process
                {
                    StartInfo =
                    {
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        FileName = exeName
                        //Arguments = args
                    }
                };

                process.Start();
                process.WaitForExit();
            });
        }
    }
}
