using System;
using System.Collections.Generic;
using System.Text;

namespace Shiftless.Common.Serialization
{
    public class BitReader
    {
        private readonly byte[] _data;

        private int _curBit;

        public BitReader(byte[] data)
        {
            _data = data;
        }

        public bool Read()
        {
            int curByte = _curBit / 8;
            int bitOffset = _curBit % 8;

            _curBit++;

            if (curByte >= _data.Length)
                return false;

            return ((_data[curByte] >> (7 - bitOffset)) & 0b1) == 0b1;
        }
        public uint Read(int bitLength)
        {
            if (bitLength > 32)
                throw new ArgumentException("Byte reader can not longer than 32 bits!");

            uint v = 0;
            for(int i = 0; i < bitLength; i++)
            {
                v <<= 1;

                if (Read())
                    v |= 0b1;
            }

            return v;
        }

        
    }
}
