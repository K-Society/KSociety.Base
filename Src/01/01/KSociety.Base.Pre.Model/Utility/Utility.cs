using System.Diagnostics;
using System.Linq;

namespace KSociety.Base.Pre.Model.Utility;

public static class Utility
{
    public static Process PriorProcess()
        // Returns a System.Diagnostics.Process pointing to
        // a pre-existing process with the same name as the
        // current one, if any; or null if the current process
        // is unique.
    {
        var curr = Process.GetCurrentProcess();
        var procs = Process.GetProcessesByName(curr.ProcessName);
        return procs.FirstOrDefault(p => curr.MainModule != null && p.MainModule != null && p.Id != curr.Id && p.MainModule.FileName == curr.MainModule.FileName);
    }
}