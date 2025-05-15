using System.Text;

namespace Shiftless.Common.Serialization
{
    public static class ByteConverter
    {
        public const byte END_OF_STRING = 0;

        public static ushort ToUint16(byte[] bytes, int startIndex) => BitConverter.ToUInt16(CheckEndian(bytes, startIndex, sizeof(ushort)));
        public static short ToInt16(byte[] bytes, int startIndex) => BitConverter.ToInt16(CheckEndian(bytes, startIndex, sizeof(short)));
        public static uint ToUInt32(byte[] bytes, int startIndex) => BitConverter.ToUInt32(CheckEndian(bytes, startIndex, sizeof(uint)));
        public static int ToInt32(byte[] bytes, int startIndex) => BitConverter.ToInt32(CheckEndian(bytes, startIndex, sizeof(int)));
        public static ulong ToUInt64(byte[] bytes, int startIndex) => BitConverter.ToUInt64(CheckEndian(bytes, startIndex, sizeof(ulong)));
        public static long ToInt64(byte[] bytes, int startIndex) => BitConverter.ToInt64(CheckEndian(bytes, startIndex, sizeof(long)));
        public static float ToSingle(byte[] bytes, int startIndex) => BitConverter.ToSingle(CheckEndian(bytes, startIndex, sizeof(float)));
        public static double ToDouble(byte[] bytes, int startIndex) => BitConverter.ToDouble(CheckEndian(bytes, startIndex, sizeof(double)));

        public static uint ToUint24(byte[] bytes, int startIndex) => (uint)((bytes[startIndex + 2] << 16) | (bytes[startIndex + 1] << 8) | bytes[startIndex]);
        public static int ToInt24(byte[] bytes, int startIndex)
        {
            int val = (int)ToUint24(bytes, startIndex);
            if ((val & 0x800000) != 0)
                val |= unchecked((int)0xFF000000); // sign extend

            return val;
        }

        public static ushort ToUInt16(byte[] bytes) => ToUint16(bytes, 0);
        public static short ToInt16(byte[] bytes) => ToInt16(bytes, 0);
        public static uint ToUInt32(byte[] bytes) => ToUInt32(bytes, 0);
        public static int ToInt32(byte[] bytes) => ToInt32(bytes, 0);
        public static ulong ToUInt64(byte[] bytes) => ToUInt64(bytes, 0);
        public static long ToInt64(byte[] bytes) => ToInt64(bytes, 0);
        public static float ToSingle(byte[] bytes) => ToSingle(bytes, 0);
        public static double ToDouble(byte[] bytes) => ToDouble(bytes, 0);

        public static int ToInt24(byte[] bytes) => ToInt24(bytes, 0);

        public static string ToString(byte[] bytes, bool hasEscapeChar = false) => Encoding.ASCII.GetString(bytes[..^(hasEscapeChar ? 1 : 0)]);

        public static byte[] GetBytes(ushort value) => CheckEndian(BitConverter.GetBytes(value), 0, sizeof(ushort));
        public static byte[] GetBytes(short value) => CheckEndian(BitConverter.GetBytes(value), 0, sizeof(short));
        public static byte[] GetBytes(uint value) => CheckEndian(BitConverter.GetBytes(value), 0, sizeof(uint));
        public static byte[] GetBytes(int value) => CheckEndian(BitConverter.GetBytes(value), 0, sizeof(int));
        public static byte[] GetBytes(ulong value) => CheckEndian(BitConverter.GetBytes(value), 0, sizeof(ulong));
        public static byte[] GetBytes(long value) => CheckEndian(BitConverter.GetBytes(value), 0, sizeof(long));
        public static byte[] GetBytes(float value) => CheckEndian(BitConverter.GetBytes(value), 0, sizeof(float));
        public static byte[] GetBytes(double value) => CheckEndian(BitConverter.GetBytes(value), 0, sizeof(double));

        public static byte[] GetBytes(string value, byte? escapeChar = null) => Encoding.ASCII.GetBytes(value + (escapeChar != null ? (char)escapeChar : ""));

        public static byte[] CheckEndian(byte[] bytes, int startIndex, int length)
        {
            byte[] segment = new byte[length];
            Array.Copy(bytes, startIndex, segment, 0, length);

            if (!BitConverter.IsLittleEndian)
                return segment.Reverse().ToArray();
            else
                return segment;
        }
    }
}
