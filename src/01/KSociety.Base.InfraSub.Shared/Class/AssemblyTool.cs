// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Class
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The AssemblyTool static class.
    /// </summary>
    public static class AssemblyTool
    {
        /// <summary>
        /// GetAssembly
        /// </summary>
        /// <returns></returns>
        public static string[] GetAssembly()
        {
            var assemblyLibPath = AppDomain.CurrentDomain.BaseDirectory;

            var assemblyLibList = Directory.EnumerateFiles(assemblyLibPath, "*.dll",
                    SearchOption.TopDirectoryOnly)
                .Where(filePath => Path.GetFileName(filePath).StartsWith("KSociety"))
                .ToList();

            return assemblyLibList.ToArray();
        }

        /// <summary>
        /// GetAssembly
        /// </summary>
        /// <param name="startWith"></param>
        /// <returns></returns>
        public static string[] GetAssembly(string startWith)
        {
            var assemblyLibPath = AppDomain.CurrentDomain.BaseDirectory;

            var assemblyLibList = Directory.EnumerateFiles(assemblyLibPath, "*.dll",
                    SearchOption.TopDirectoryOnly)
                .Where(filePath => Path.GetFileName(filePath).StartsWith(startWith))
                .ToList();

            return assemblyLibList.ToArray();
        }

        /// <summary>
        /// GetAssemblyByName
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name.Equals(name));
        }
    }
}
