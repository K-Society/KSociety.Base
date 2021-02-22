using Microsoft.Deployment.WindowsInstaller;
using System.Diagnostics;
using WixSharp;

namespace InstallAction
{
    public static class InstallManager
    {
        [CustomAction]
        public static ActionResult SilentInstall(Session session)
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
        public static ActionResult SilentUninstall(Session session)
        {
            return session.HandleErrors(() =>
            {
                var data = session.CustomActionData;
                var exeName = data["ExeName"];
                var args = data["Args"];

                var process = new Process()
                {
                    StartInfo =
                    {
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
    }
}
