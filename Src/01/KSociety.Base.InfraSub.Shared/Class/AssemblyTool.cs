using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace KSociety.Base.InfraSub.Shared.Class
{
    public static class AssemblyTool
    {
        public static string[] GetAssembly()
        {
            var assemblyLibPath = AppDomain.CurrentDomain.BaseDirectory;

            List<string> assemblyLibList = Directory.EnumerateFiles(assemblyLibPath, "*.dll",
                    SearchOption.TopDirectoryOnly)
                .Where(filePath => Path.GetFileName(filePath).StartsWith("KSociety"))
                .ToList();

            return assemblyLibList.ToArray();
        }

        public static string[] GetAssembly(string startWith)
        {
            var assemblyLibPath = AppDomain.CurrentDomain.BaseDirectory;

            List<string> assemblyLibList = Directory.EnumerateFiles(assemblyLibPath, "*.dll",
                    SearchOption.TopDirectoryOnly)
                .Where(filePath => Path.GetFileName(filePath).StartsWith(startWith))
                .ToList();

            return assemblyLibList.ToArray();
        }

        public static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name.Equals(name));
        }
    }
}
