// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Class.Checksum
{
    using System;
    using Interface.Checksum;
    using Microsoft.Extensions.Logging;

    public class Checksum : IChecksum
    {
        private readonly ILogger<Checksum> _logger;

        public Checksum(ILogger<Checksum> logger)
        {
            this._logger = logger;
        }

        public byte GetChecksum(byte[] buf)
        {
            var size = buf[2] + 3;

            //ToDo
            //if (buf.Length >= size) return 0x00;

            var temp = 0;
            for (var i = 0; i < size; i++)
            {
                temp += buf[i];
            }

            temp = (~temp & 0x000000FF) + 1;
            var res = (byte)(temp & 0x000000FF);

            //_logger.LogTrace("GetChecksum: " + BitConverter.ToString(new[] { res }));

            return res;
        }

        public byte GetChecksum(byte[] buf, int checksumIndex)
        {
            //var size = buf[2];
            var temp = 0;
            for (var i = 0; i < checksumIndex; i++)
            {
                temp += buf[i];
            }

            temp = (~temp & 0x000000FF) + 1;
            var res = (byte)(temp & 0x000000FF);

            //_logger.LogTrace("GetChecksum: " + BitConverter.ToString(new[] { res }));

            return res;
        }

        public byte[] GetChecksumArray(byte[] buffer)
        {
            var size = buffer[2] + 3;
            var bufferLength = size; //buffer.Length;
            var bufferToSendLength = size + 1; //buffer.Length + 1;
            var bufferToSend = new byte[bufferToSendLength];

            Buffer.BlockCopy(buffer, 0, bufferToSend, 0, bufferLength);
            bufferToSend[bufferLength] = this.GetChecksum(buffer);

            return bufferToSend;
        }

        public bool VerifyChecksum(byte[] buf)
        {
            try
            {
                if (buf.Length < 3)
                {
                    return false;
                }

                var size = buf[2] + 3;
                if (buf.Length <= size)
                {
                    return false;
                }

                var checkSum = this.GetChecksum(buf, size);
                var result = buf[size].Equals(checkSum);

                if (result)
                {

                }
                else
                {
                    this._logger.LogWarning("VerifyChecksum checksum does not match!");
                }

                return result;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "VerifyChecksum: ");
                return false;
            }
        }
    }
}
