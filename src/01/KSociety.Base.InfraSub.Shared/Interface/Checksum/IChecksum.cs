// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Interface.Checksum
{
    public interface IChecksum
    {
        byte GetChecksum(byte[] buf);
        byte GetChecksum(byte[] buf, int checksumIndex);

        byte[] GetChecksumArray(byte[] buffer);

        bool VerifyChecksum(byte[] buf);
    }
}
