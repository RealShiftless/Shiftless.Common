using Shiftless.Common.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Common.Mathematics
{
    public struct Color565(ushort packedValue)
    {
        // Constants
        private const ushort R_CLEAR_MASK = 0b0000011111111111; // Clear top 5 bits
        private const ushort G_CLEAR_MASK = 0b1111100000011111; // Clear middle 6 bits
        private const ushort B_CLEAR_MASK = 0b1111111111100000; // Clear bottom 5 bits


        // Values
        public ushort PackedValue = packedValue;


        // Properties
        public byte R
        {
            readonly get => MHelp.Map((byte)((PackedValue >> 11) & 31), 31, 255);
            set
            {
                byte mappedValue = MHelp.Map(value, 255, 31);
                PackedValue = (ushort)((PackedValue & R_CLEAR_MASK) | (mappedValue << 11));
            }
        }
        public byte G
        {
            readonly get => MHelp.Map((byte)((PackedValue >> 5) & 63), 63, 255);
            set
            {
                byte mappedValue = MHelp.Map(value, 255, 63);
                PackedValue = (ushort)((PackedValue & G_CLEAR_MASK) | (mappedValue << 5));
            }
        }
        public byte B
        {
            readonly get => MHelp.Map((byte)(PackedValue & 31), 31, 255);
            set
            {
                byte mappedValue = MHelp.Map(value, 255, 31);
                PackedValue = (ushort)((PackedValue & B_CLEAR_MASK) | mappedValue);
            }
        }


        // Constructor
        public Color565() : this(0b1111100000011111) { }
        public Color565(byte r, byte g, byte b) : this((ushort)((MHelp.Map(r, 255, 31) << 11) | (MHelp.Map(g, 255, 63) << 5) | (MHelp.Map(b, 255, 31)))) { }
        public Color565(uint packedColor) : this((byte)(packedColor >> 16 & 0xFF), (byte)(packedColor >> 8 & 0xFF), (byte)(packedColor & 0xFF))
        {
            if (packedColor > 0xFFFFFFu)
                throw new ArgumentException($"Color value out of range! (0x000000 ≤ {PackedValue:x6} ≤ 0xFFFFFF)");
        }
        

        // Cast Operators
        public static implicit operator ushort(Color565 color) => color.PackedValue;
    }
}
