using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Shiftless.Common.Serialization
{
    public class ByteReader : IDisposable
    {
        // Constants
        public const int BLOCK_SIZE = 4096;

        public const byte DEF_END_OF_STRING = 0;

        public const bool DEF_SKIP_OVER = true;
        public const bool DEF_RETURN_AT_EOS = false;


        // Values
        //private FileStream _stream = new(path, FileMode.Open);
        private FileStream? _fileStream;
        private MemoryStream _stream;

        private int _curBlock = 0;
        private int _curBlockLength = 0;

        private byte[] _blockBuffer = new byte[BLOCK_SIZE];


        // Properties
        public long Position
        {
            get => (_fileStream != null ? _fileStream.Position - _curBlockLength : 0) + _stream.Position;
            set
            {
                // If the value is exactly the same we can skip tbh
                if (value == Position) 
                    return;

                // If we don't have a file stream, we can just directly set the streams position
                if(_fileStream == null)
                {
                    _stream.Position = value;
                    return;
                }

                // Here we get the current and next block
                int curBlockPos = (int)(Position / BLOCK_SIZE);
                int nextBlockPos = (int)(value / BLOCK_SIZE);

                // If we go to another block we can copy said block to the stream
                if(curBlockPos != nextBlockPos)
                    CopyBlock(nextBlockPos);

                // And here we set the position of the stream to its local position
                _stream.Position = value % BLOCK_SIZE;
            }
        }

        public long Length => _fileStream?.Length ?? _stream.Length;

        public long Remaining => Length - Position;


        // Constructors
        public ByteReader(string file)
        {
            _fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);

            _stream = new(BLOCK_SIZE);
            CopyBlock(0);
        }
        public ByteReader(byte[] buffer)
        {
            _stream = new(buffer, false);
        }


        // Finalizer
        ~ByteReader()
        {
            _stream.Dispose();

        }


        // Func
        private void CopyBlock(int blockIndex)
        {
            // First check if we can actually copy a block
            if (_fileStream == null)
                throw new InvalidOperationException("File stream was null when copy block was called!");

            // Calculate the offset based on the block index
            long offset = (long)blockIndex * BLOCK_SIZE;

            long fileLength = Length;

            // If the offset is longer than the length we are out of range
            if(offset >= fileLength)
                throw new ArgumentOutOfRangeException(nameof(offset));

            // Get the length of the current block, it might be smaller if we are reaching the end of the file
            long lengthTillEOS = fileLength - offset;
            int length = (int)Math.Min(BLOCK_SIZE, lengthTillEOS);
            
            // If it is smaller or is 0 we are at the EOS which should not happen, throw an error
            if (length <= 0)
                throw new EndOfStreamException("Unexpected EOS while copying block!");

            // Set the file streams position to the offset
            _fileStream.Position = offset;

            // Create a buffer and fill it
            int bytesRead = _fileStream.Read(_blockBuffer, 0, length);

            // If we did not read any bytes we should throw an error, because we are at an EOS, how? idk.
            if (bytesRead <= 0)
                throw new EndOfStreamException("Unexpected EOS while copying block!");

            // If the length of the block is smaller than the block size we should set its length
            if (length != BLOCK_SIZE)
            {
                _stream.SetLength(length);
            }

            // We reset the streams position
            _stream.Position = 0;

            // And we write
            _stream.Write(_blockBuffer, 0, length);

            _stream.Position = 0;

            _curBlockLength = length;
            _curBlock = blockIndex;
        }

        private void CopyNextBlock()
        {
            int index = (int)(Position / BLOCK_SIZE);

            if (index != _curBlock)
            {
                CopyBlock(index);
                return;
            }

            throw new EndOfStreamException("File stream reached EOS when copying next block!");
        }


        public byte Next()
        {
            int v = _stream.ReadByte();

            if (v == -1)
            {
                if(_fileStream != null)
                {
                    CopyNextBlock();
                    return Next();
                }

                throw new EndOfStreamException("Byte stream has reached EOS!");
            }

            return (byte)v;
        }
        public bool TryNext(out byte value)
        {
            try
            {
                value = Next();
                return true;
            }
            catch(EndOfStreamException)
            {
                value = default;
                return false;
            }
        }

        public byte Peek()
        {
            byte value = Next();
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

        public byte[] Next(int length)
        {
            byte[] buffer = new byte[length];

            for(int i = 0; i < length; i++)
                buffer[i] = Next();

            return buffer;
        }
        public bool TryNext(int length, [NotNullWhen(true)] out byte[]? result)
        {
            long pos = _stream.Position;

            try
            {
                result = Next(length);
                return true;
            }
            catch
            {
                _stream.Position = pos;

                result = null;
                return false;
            }
        }

        public ushort NextUInt16() => ByteConverter.ToUInt16(Next(sizeof(ushort)));
        public bool TryNextUInt16(out ushort value)
        {
            long pos = _stream.Position;

            try
            {
                value = NextUInt16();
                return true;
            }
            catch(EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public short NextInt16() => ByteConverter.ToInt16(Next(sizeof(short)));
        public bool TryNextInt16(out short value)
        {
            long pos = _stream.Position;

            try
            {
                value = NextInt16();
                return true;
            }
            catch(EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public uint NextUInt32() => ByteConverter.ToUInt32(Next(sizeof(uint)));
        public bool TryNextUInt32(out uint value)
        {
            long pos = _stream.Position;

            try
            {
                value = NextUInt32();
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public int NextInt32() => ByteConverter.ToInt32(Next(sizeof(int)));
        public bool TryNextInt32(out int value)
        {
            long pos = _stream.Position;

            try
            {
                value = NextInt32();
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public ulong NextUInt64() => ByteConverter.ToUInt64(Next(sizeof(ulong)));
        public bool TryNextUInt64(out ulong value)
        {
            long pos = _stream.Position;


            try
            {
                value = NextUInt64();
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public long NextInt64() => ByteConverter.ToInt64(Next(sizeof(long)));
        public bool TryNextInt64(out long value)
        {
            long pos = _stream.Position;

            try
            {
                value = NextInt64();
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public float NextSingle() => ByteConverter.ToSingle(Next(sizeof(float)));
        public bool TryNextSingle(out float value)
        {
            long pos = _stream.Position;

            try
            {
                value = NextSingle();
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public double NextDouble() => ByteConverter.ToDouble(Next(sizeof(double)));
        public bool TryNextDouble(out double value)
        {
            long position = _stream.Position;

            try
            {
                value = NextDouble();
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = position;

                value = default;
                return false;
            }
        }

        public string NextString(byte exitCharacter = DEF_END_OF_STRING)
        {
            StringBuilder value = new();

            while(TryNext(out byte b))
            {
                if (b == exitCharacter)
                    return value.ToString();
                value.Append(b);
            }

            throw new EndOfStreamException("EOF reached before end of string!");
        }
        public bool TryNextString([NotNullWhen(true)] out string? value, byte eos = DEF_END_OF_STRING)
        {
            long pos = _stream.Position;

            try
            {
                value = NextString(eos);
                return true;
            }
            catch (EndOfStreamException)
            {
                _stream.Position = pos;

                value = default;
                return false;
            }
        }

        public string NextString(int length)
        {
            StringBuilder value = new(length);

            for(int i = 0; i < length; i++)
                value.Append((char)Next());

            return value.ToString();
        }
        public bool TryNextString([NotNullWhen(true)] out string? value, int length)
        {
            long pos = Position;

            try
            {
                value = NextString(length);
                return true;
            }
            catch(EndOfStreamException)
            {
                Position = pos;
                value = null;
                return false;
            }
        }

        public void Skip(long count)
        {
            Position += count;
        }
        public int SkipUntil(byte[] pattern, bool skipOver = DEF_SKIP_OVER, bool returnAtEndOfSteam = DEF_RETURN_AT_EOS)
        {
            // First check if we have a valid pattern
            if (pattern.Length == 0)
                throw new ArgumentException("Pattern must not be empty", nameof(pattern));

            // First we create a byte[] to store a rolling buffer
            byte[] buffer = new byte[pattern.Length];
            int curOffset = 0;

            // We pre fill said buffer
            for (int i = 0; i < pattern.Length; i++)
                buffer[i] = Next();

            // Now we go over the bytes until a pattern is matched
            while (Position < Length)
            {
                // Check if we match the pattern
                bool match = true;
                for (int i = 0; i < pattern.Length; i++)
                {
                    int index = (curOffset + i) % pattern.Length;
                    if (buffer[index] == pattern[i])
                        continue;

                    match = false;
                    break;
                }

                // If we do return
                if (match)
                {
                    if(!skipOver)
                        Position -= pattern.Length;

                    return curOffset;
                }

                // If we dont match the pattern we add the next byte to the rolling buffer
                if (!TryNext(out byte value))
                    break;

                buffer[curOffset % pattern.Length] = value;
                curOffset++;
            }

            // If we get here the pattern was not matched, but no stress, if the user wants it to return at the end of a stream you can.
            if (returnAtEndOfSteam)
                return curOffset;

            // But if we get here, we sadly should throw an error :(
            throw new EndOfStreamException("EOS reached before pattern was matched!");
        }
        public int SkipUntil(string pattern, bool skipOver = DEF_SKIP_OVER, bool returnAtEndOfStream = DEF_RETURN_AT_EOS) => SkipUntil(Encoding.UTF8.GetBytes(pattern), skipOver, returnAtEndOfStream);

        public bool TrySkipUntil(out int length, byte[] pattern, bool skipOver = DEF_SKIP_OVER)
        {
            long pos = Position;

            try
            {
                length = SkipUntil(pattern, skipOver, false);
                return true;
            }
            catch (EndOfStreamException)
            {
                Position = pos;

                length = 0;
                return false;
            }
        }
        public bool TrySkipUntil(out int length, string pattern, bool skipOver = DEF_SKIP_OVER) => TrySkipUntil(out length, Encoding.UTF8.GetBytes(pattern), skipOver);

        public byte[] ReadUntil(byte[] pattern, bool skipOver = DEF_SKIP_OVER, bool returnAtEndOfStream = DEF_RETURN_AT_EOS)
        {
            long pos = Position;
            int length = SkipUntil(pattern, skipOver, returnAtEndOfStream);

            byte[] buffer = new byte[length];
            Position = pos;
            //_stream.ReadExactly(bytes, 0, length);

            for (int i = 0; i < length; i++)
                buffer[i] = Next();

            return buffer;
        }
        public byte[] ReadUntil(string pattern, bool skipOver = DEF_SKIP_OVER, bool returnAtEndOfStream = DEF_RETURN_AT_EOS) => ReadUntil(Encoding.UTF8.GetBytes(pattern), skipOver, returnAtEndOfStream);

        public bool TryReadUntil([NotNullWhen(true)] out byte[]? data, byte[] pattern, bool skipOver = DEF_SKIP_OVER)
        {
            try
            {
                data = ReadUntil(pattern, skipOver, false);
                return true;
            }
            catch(EndOfStreamException)
            {
                data = null;
                return false;
            }
        }
        public bool TryReadUntil([NotNullWhen(true)] out byte[]? data, string pattern, bool skipOver = DEF_SKIP_OVER) => TryReadUntil(out data, Encoding.UTF8.GetBytes(pattern), skipOver);
        
        public byte[] GetRemaining()
        {
            long lengthToEOS = Length - Position;

            if (lengthToEOS > int.MaxValue)
                throw new InvalidOperationException($"Length until EOS is greated than {int.MaxValue} and cannot be stored into an array!");

            return Next((int)lengthToEOS);
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _stream.Dispose();
            _fileStream?.Dispose();

            _blockBuffer = [];
        }
    }
}
