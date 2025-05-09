using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Shiftless.Common.Serialization
{
    public class ByteStream(string path) : IDisposable
    {
        // Constants
        const byte DEF_END_OF_STRING = 0;


        // Values
        private FileStream _stream = new(path, FileMode.Open);


        // Properties
        public long Position
        {
            get => _stream.Position;
            set => _stream.Position = value;
        }

        public long Length => _stream.Length;
        public long Remaining => _stream.Length - Position;

        public bool IsAtEnd => Position == Length;

        public string Name => _stream.Name;


        // Finalizer
        ~ByteStream()
        {
            _stream.Dispose();
        }


        // Func
        public byte Read()
        {
            if(TryRead(out byte value))
                return value;

            throw new EndOfStreamException("Byte stream has reached EOF!");
        }
        public bool TryRead(out byte value)
        {
            int v = _stream.ReadByte();

            if(v == -1)
            {
                value = default;
                return false;
            }
            else
            {
                value = (byte)v;
                return true;
            }
        }

        public byte Peek()
        {
            byte value = Read();
            _stream.Position--;

            return value;
        }
        public bool TryPeek(out byte value)
        {
            try
            {
                value = Peek();
                return true;
            }
            catch(EndOfStreamException)
            {
                value = default;
                return false;
            }
        }

        public byte[] Read(int length)
        {
            byte[] bytes = new byte[length];

            _stream.ReadExactly(bytes, 0, length);
            return bytes;
        }
        public bool TryRead(int length, [NotNullWhen(true)] out byte[]? result)
        {
            long pos = _stream.Position;

            try
            {
                result = Read(length);
                return true;
            }
            catch
            {
                _stream.Position = pos;

                result = null;
                return false;
            }
        }

        public ushort ReadUInt16() => ByteConverter.ToUInt16(Read(sizeof(ushort)));
        public bool TryReadUInt16(out ushort value)
        {
            long pos = _stream.Position;

            try
            {
                value = ReadUInt16();
                return true;
            }
            catch(EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public short ReadInt16() => ByteConverter.ToInt16(Read(sizeof(short)));
        public bool TryReadInt16(out short value)
        {
            long pos = _stream.Position;

            try
            {
                value = ReadInt16();
                return true;
            }
            catch(EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public uint ReadUInt32() => ByteConverter.ToUInt32(Read(sizeof(uint)));
        public bool TryReadUInt32(out uint value)
        {
            long pos = _stream.Position;

            try
            {
                value = ReadUInt32();
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public int ReadInt32() => ByteConverter.ToInt32(Read(sizeof(int)));
        public bool TryReadInt32(out int value)
        {
            long pos = _stream.Position;

            try
            {
                value = ReadInt32();
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public ulong ReadUInt64() => ByteConverter.ToUInt64(Read(sizeof(ulong)));
        public bool TryReadUInt64(out ulong value)
        {
            long pos = _stream.Position;


            try
            {
                value = ReadUInt64();
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public long ReadInt64() => ByteConverter.ToInt64(Read(sizeof(long)));
        public bool TryReadInt64(out long value)
        {
            long pos = _stream.Position;

            try
            {
                value = ReadInt64();
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public float ReadSingle() => ByteConverter.ToSingle(Read(sizeof(float)));
        public bool TryReadSingle(out float value)
        {
            long pos = _stream.Position;

            try
            {
                value = ReadSingle();
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public double ReadDouble() => ByteConverter.ToDouble(Read(sizeof(double)));
        public bool TryReadDouble(out double value)
        {
            long position = _stream.Position;

            try
            {
                value = ReadDouble();
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = position;

                value = default;
                return false;
            }
        }

        public string ReadString(byte eos = DEF_END_OF_STRING)
        {
            StringBuilder value = new();

            while(TryRead(out byte b))
            {
                if (b == eos)
                    return value.ToString();
                value.Append((char)b);
            }

            throw new EndOfStreamException("EOF reached before end of string!");
        }
        public bool TryReadString([NotNullWhen(true)] out string? value, byte eos = DEF_END_OF_STRING)
        {
            long pos = _stream.Position;

            try
            {
                value = ReadString(eos);
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public string ReadString(int length)
        {
            StringBuilder value = new(length);

            for(int i = 0; i < length; i++)
                value.Append((char)Read());

            return value.ToString();
        }
        public bool TryReadString([NotNullWhen(true)] out string? value, int length)
        {
            long pos = Position;

            try
            {
                value = ReadString(length);
                return true;
            }
            catch(EndOfStreamException)
            {
                Position = pos;
                value = null;
                return false;
            }
        }

        public byte[] ReadUntil(byte[] pattern, bool returnAtEndOfStream = false)
        {
            // Create some of the storage stuff
            List<byte> bytes = [];
            byte[] buffer = new byte[pattern.Length];

            // Pre fill the buffer
            for (int i = 0; i < pattern.Length; i++)
                buffer[i] = Read();

            // We use a rolling buffer so we store a current offset
            int curOffset = 0;
            while(Position < Length)
            {
                // First check if we are currently in a match :)
                bool match = true;
                for(int i = 0; i < pattern.Length; i++)
                {
                    int patternOffset = (curOffset + i) % pattern.Length;

                    if (buffer[patternOffset] != pattern[i])
                    {
                        match = false;
                        break;
                    }
                }

                // If it is a match, rewind to the position where the pattern starts
                if(match)
                {
                    Position -= pattern.Length;
                    return [.. bytes];
                }

                // Now we add the current byte to the bytes list
                bytes.Add(buffer[curOffset]);

                // Set the value in the buffer to the next byte
                buffer[curOffset] = Read();

                // And increment the cur offset so we can use the rolling buffer :)
                curOffset = (curOffset + 1) % pattern.Length;
            }

            // If we get here we did not match the pattern, but if we want to still return said data, we do that here
            if(returnAtEndOfStream)
                return [.. bytes];

            // So sad nothing to return, so we throw an error :)
            throw new EndOfStreamException("EOS reached before pattern was matched!");
        }

        public byte[] ReadRemaining()
        {
            byte[] buffer = new byte[Remaining];

            int i = 0;
            while(Position < Length)
                buffer[i++] = Read();

            return buffer;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _stream.Dispose();
        }
    }
}
