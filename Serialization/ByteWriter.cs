using System;
using System.Collections.Generic;
using System.Text;

namespace Shiftless.Common.Serialization
{
    public sealed class ByteWriter : IDisposable
    {
        // Values
        private MemoryStream _stream;


        // Constructors
        public ByteWriter() => _stream = new MemoryStream();
        public ByteWriter(int capacity) => _stream = new MemoryStream(capacity);
        public ByteWriter(byte[] buffer) => _stream = new MemoryStream(buffer);
        public ByteWriter(byte[] buffer, int index, int count) => _stream = new MemoryStream(buffer, index, count);


        // Properties
        public long Length => _stream.Length;


        // Func
        public void Write(byte value) => _stream.WriteByte(value);
        public void Write(byte[] values) => _stream.Write(values, 0, values.Length);

        public void Write(ushort value) => Write(ByteConverter.GetBytes(value));
        public void Write(IEnumerable<ushort> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public void Write(short value) => Write(ByteConverter.GetBytes(value));
        public void Write(IEnumerable<short> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public void Write(uint value) => Write(ByteConverter.GetBytes(value));
        public void Write(IEnumerable<uint> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public void Write(int value) => Write(ByteConverter.GetBytes(value));
        public void Write(IEnumerable<int> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public void Write(ulong value) => Write(ByteConverter.GetBytes(value));
        public void Write(IEnumerable<ulong> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public void Write(long value) => Write(ByteConverter.GetBytes(value));
        public void Write(IEnumerable<long> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public void Write(float value) => Write(ByteConverter.GetBytes(value));
        public void Write(IEnumerable<float> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public void Write(double value) => Write(ByteConverter.GetBytes(value));
        public void Write(IEnumerable<double> values)
        {
            foreach (var value in values)
                Write(value);
        }

        public void Write(string value, byte? escapeChar = ByteConverter.END_OF_STRING) => Write(ByteConverter.GetBytes(value, escapeChar));

        public void Dispose()
        {
            _stream.Dispose();
        }

        public void Save(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? "");
            using FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);

            // Go to the begining of the stream
            _stream.Seek(0, SeekOrigin.Begin);

            // Copy the stream to the file :)
            _stream.CopyTo(fileStream);
        }

        public byte[] ToArray() => _stream.ToArray();
    }
}
