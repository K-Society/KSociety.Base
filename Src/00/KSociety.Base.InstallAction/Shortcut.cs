using Microsoft.Deployment.WindowsInstaller;
using WixSharp;

namespace KSociety.Base.InstallAction
{
    public static class Shortcut
    {
        [CustomAction]
        public static ActionResult DirShortcut(Session session)
        {
            return session.HandleErrors(() =>
            {
                //ToDo
                //var data = session.CustomActionData;
                //string exeName = data["ExeName"];
                //string args = data["Args"];

                //IWshShortcut shortcut;
                //var wshShell = new WshShellClass();
                //shortcut = (IWshShortcut)wshShell.CreateShortcut(Path.Combine(desktop, @"Temp.lnk"));
                //shortcut.TargetPath = @"\\computername\sharename";
                //shortcut.Save();
            });
        }
    }
}
