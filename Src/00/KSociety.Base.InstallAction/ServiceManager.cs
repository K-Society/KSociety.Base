using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

namespace KSociety.Base.InstallAction
{
    public static class ServiceManager
    {
        [CustomAction]
        public static ActionResult InstallService(Session session)
        {
            return session.HandleErrors(() =>
            {
                var data = session.CustomActionData;
                string installState = data["InstallState"];
                string serviceName = data["ServiceName"];
                installState = installState.Remove(installState.Length - 1);

                string args = "/i /LogFile= /LogToConsole=false /InstallStateDir=" + "\"" + installState + "\"";

                System.Diagnostics.Process process = System.Diagnostics.Process.Start(serviceName, args);
                process?.WaitForExit(1000 * 60 * 5);// Wait up to five minutes.

            });
        }

        [CustomAction]
        public static ActionResult UnInstallService(Session session)
        {
            return session.HandleErrors(() =>
            {
                var data = session.CustomActionData;
                string installState = data["InstallState"];
                string serviceName = data["ServiceName"];
                installState = installState.Remove(installState.Length - 1);

                string args = "/u /LogFile= /LogToConsole=false /InstallStateDir=" + "\"" + installState + "\"";

                System.Diagnostics.Process process = System.Diagnostics.Process.Start(serviceName, args);
                process?.WaitForExit(1000 * 60 * 5);// Wait up to five minutes.
            });
        }
    }
}
