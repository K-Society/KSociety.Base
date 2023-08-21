namespace KSociety.Base.InfraSub.Shared.Class
{
    using System;
    using System.Linq;

    public static class Converter
    {
        public static byte[] StringToByteArray(string bytes)
        {
            if (String.IsNullOrEmpty(bytes)) {return Array.Empty<byte>();}

            string[] splitResult = bytes.Contains('-') ? bytes.Split('-') : bytes.SplitInParts(2).ToArray();

            byte[] pathArray = new byte[splitResult.Length];

            for (int i = 0; i < splitResult.Length; i++)
            {
                pathArray[i] = Convert.ToByte(splitResult[i], 16);
            }

            return pathArray;
        }
    }
}
