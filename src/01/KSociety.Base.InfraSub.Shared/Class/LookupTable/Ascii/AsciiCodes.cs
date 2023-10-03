// Copyright © K-Society and contributors. All rights reserved. Licensed under the K-Society License. See LICENSE.TXT file in the project root for full license information.

namespace KSociety.Base.InfraSub.Shared.Class.LookupTable.Ascii
{
    public struct AsciiCodes
    {
        public byte Hex { get; }
        public string Symbol { get; }
        public string Description { get; }

        public AsciiCodes(byte hex, string symbol, string description)
        {
            this.Hex = hex;
            this.Symbol = symbol;
            this.Description = description;
        }
    }
}
