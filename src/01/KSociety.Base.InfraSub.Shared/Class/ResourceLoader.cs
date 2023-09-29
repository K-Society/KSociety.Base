// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Class
{
    using System.IO;
    using System.Reflection;

    public static class ResourceLoader
    {
        public static string LoadFileFromResource(Assembly assembly, string fileResourceName, string fileName)
        {
            using var stream = assembly.GetManifestResourceStream(fileResourceName);
            var data = new BinaryReader(stream).ReadBytes((int)stream.Length);

            var assemblyPath = Path.GetDirectoryName(assembly.Location);
            var tempFilePath = Path.Combine(assemblyPath, fileName);

            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
                File.WriteAllBytes(tempFilePath, data);
            }
            else
            {
                File.WriteAllBytes(tempFilePath, data);
            }

            return tempFilePath;
        }

        public static string LoadFileFromResourceToTemp(Assembly assembly, string fileResourceName, string fileName)
        {
            using var stream = assembly.GetManifestResourceStream(fileResourceName);
            var data = new BinaryReader(stream).ReadBytes((int)stream.Length);

            var assemblyPath = Path.GetTempPath();
            var tempFilePath = Path.Combine(assemblyPath, fileName);

            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
                File.WriteAllBytes(tempFilePath, data);
            }
            else
            {
                File.WriteAllBytes(tempFilePath, data);
            }

            return tempFilePath;
        }
    }
}
