// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Class
{
    using System.Diagnostics;
    using System.Linq;

    public static class ProcessUtil
    {

        #if NETSTANDARD2_0

        public static Process PriorProcess()
        {
            var current = Process.GetCurrentProcess();
            var process = Process.GetProcessesByName(current.ProcessName);
            return process.FirstOrDefault(p =>
                current.MainModule != null && p.MainModule != null && p.Id != current.Id &&
                p.MainModule.FileName == current.MainModule.FileName);
        }

        #elif NETSTANDARD2_1

        public static Process PriorProcess()
        {
            var current = Process.GetCurrentProcess();
            var process = Process.GetProcessesByName(current.ProcessName);
            return process.FirstOrDefault(p =>
                current.MainModule != null && p.MainModule != null && p.Id != current.Id &&
                p.MainModule.FileName == current.MainModule.FileName);
        }

        #endif

    }
}
