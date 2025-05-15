namespace Shiftless.Common.Serialization
{
    internal class ByteWriterOld()
    {
        private List<byte> _bytes = [];

        public byte[] GetBytes() => _bytes.ToArray();

        public void WriteHeader(string header)
        {
            if (header.Length > 4 || header.Length <= 0)
                throw new Exception("Header must be 5 > Header.Length > 0");

            for (int i = 0; i < 4; i++)
            {
                if (i < header.Length)
                    _bytes.Add((byte)header[i]);
                else
                    _bytes.Add((byte)' ');
            }
        }

        public void Save(string path) => File.WriteAllBytes(path, _bytes.ToArray());

        public void Write(byte value) => _bytes.Add(value);
        public void Write(byte[] bytes) => _bytes.AddRange(bytes);
        public void Write(int value) => Write(ByteConverter.GetBytes(value));
        public void Write(uint value) => Write(ByteConverter.GetBytes(value));
        public void Write(short value) => Write(ByteConverter.GetBytes(value));
        public void Write(ushort value) => Write(ByteConverter.GetBytes(value));
        public void Write(long value) => Write(ByteConverter.GetBytes(value));
        public void Write(ulong value) => Write(ByteConverter.GetBytes(value));
        public void Write(float value) => Write(ByteConverter.GetBytes(value));
        public void Write(double value) => Write(ByteConverter.GetBytes(value));
        public void Write(string value) => Write(ByteConverter.GetBytes(value));

        public void Write(IEnumerable<int> values) => values.ToList().ForEach(Write);
        public void Write(IEnumerable<uint> values) => values.ToList().ForEach(Write);
        public void Write(IEnumerable<short> values) => values.ToList().ForEach(Write);
        public void Write(IEnumerable<ushort> values) => values.ToList().ForEach(Write);
        public void Write(IEnumerable<long> values) => values.ToList().ForEach(Write);
        public void Write(IEnumerable<ulong> values) => values.ToList().ForEach(Write);
        public void Write(IEnumerable<float> values) => values.ToList().ForEach(Write);
        public void Write(IEnumerable<double> values) => values.ToList().ForEach(Write);
        public void Write(IEnumerable<string> values) => values.ToList().ForEach(Write);

    }
}
