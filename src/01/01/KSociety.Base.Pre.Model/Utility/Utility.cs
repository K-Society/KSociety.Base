// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.Pre.Model.Utility
{
    using System.Diagnostics;
    using System.Linq;

    public static class Utility
    {
        public static Process PriorProcess()
            // Returns a System.Diagnostics.Process pointing to
            // a pre-existing process with the same name as the
            // current one, if any; or null if the current process
            // is unique.
        {
            var currentProcess = Process.GetCurrentProcess();
            var processes = Process.GetProcessesByName(currentProcess.ProcessName);
            return processes.FirstOrDefault(p =>
                currentProcess.MainModule != null && p.MainModule != null && p.Id != currentProcess.Id &&
                p.MainModule.FileName == currentProcess.MainModule.FileName);
        }
    }
}