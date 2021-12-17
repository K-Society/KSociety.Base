using System;

namespace KSociety.Base.InfraSub.Shared.Class;

public static class Converter
{
    public static byte[] StringToByteArray(string bytes)
    {
        string[] splitResult = bytes.Split('-');

        byte[] pathArray = new byte[splitResult.Length];

        for (int i = 0; i < splitResult.Length; i++)
        {
            pathArray[i] = Convert.ToByte(splitResult[i], 16);
        }

        return pathArray;
    }
}