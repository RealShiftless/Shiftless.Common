using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Common.Serialization
{
    public class ByteReaderOld
    {
        private readonly byte[] _data;

        private int _current = 0;

        public bool IsComplete => _current >= _data.Length;

        public ByteReaderOld(byte[] data)
        {
            _data = data;
        }

        public void Skip(int length) => _current += length;

        public bool SkipUntil(byte[] pattern, bool skipOver)
        {
            int oldCurrent = _current;

            while(_current < _data.Length)
            {
                bool match = true;
                for(int i = 0; i < pattern.Length; i++)
                {
                    if (pattern[i] == _data[_current + i])
                        continue;

                    match = false;
                    break;
                }

                if (match)
                {
                    if(skipOver)
                        _current += pattern.Length;

                    return true;
                }

                _current++;
            }

            _current = oldCurrent;
            return false;
        }
        public bool SkipUntil(string pattern, bool skipOver) => SkipUntil(ByteConverter.GetBytes(pattern)[1..^1], skipOver);

        public string NextHeader()
        {
            byte[] data = _data[_current..(_current+4)];
            _current += 4;
            return ByteConverter.ToString(data);
        }

        public byte Next() => _data[_current++];
        public bool TryNext(out byte value)
        {
            if (_current < _data.Length)
            {
                value = _data[_current++];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public byte[] Remaining() => _data[_current..];

        public byte[] Next(int length)
        {
            byte[] bytes = _data[_current..(_current + length)];
            _current += length;
            return bytes;
        }
        public bool TryNext(int length, out byte[] bytes)
        {
            if (_current + length < _data.Length)
            {
                bytes = Next(length);
                return true;
            }
            else
            {
                bytes = Array.Empty<byte>();
                return false;
            }
        }

        public byte[] Next(char stopAt)
        {
            List<byte> bytes = new();

            while (TryNext(out byte b))
            {
                if (b == stopAt)
                    return bytes.ToArray();

                bytes.Add(b);
            }

            throw new Exception($"No {stopAt} byte was found!");
        }
        public bool TryNext(char stopAt, out byte[] bytes)
        {
            try
            {
                bytes = Next(stopAt);
                return true;
            }
            catch
            {
                bytes = Array.Empty<byte>();
                return false;
            }
        }

        public ushort NextUInt16() => ByteConverter.ToUInt16(Next(sizeof(ushort)));
        public bool TryNext(out ushort value)
        {
            if (TryNext(sizeof(ushort), out byte[] bytes))
            {
                value = ByteConverter.ToUInt16(bytes);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public short NextInt16() => ByteConverter.ToInt16(Next(sizeof(short)));
        public bool TryNext(out short value)
        {
            if (TryNext(sizeof(short), out byte[] bytes))
            {
                value = ByteConverter.ToInt16(bytes);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public uint NextUInt32() => ByteConverter.ToUInt32(Next(sizeof(uint)));
        public bool TryNext(out uint value)
        {
            if (TryNext(sizeof(uint), out byte[] bytes))
            {
                value = ByteConverter.ToUInt32(bytes);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public int NextInt32() => ByteConverter.ToInt32(Next(sizeof(int)));
        public bool TryNext(out int value)
        {
            if (TryNext(sizeof(int), out byte[] bytes))
            {
                value = ByteConverter.ToInt32(bytes);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public ulong NextUInt64() => ByteConverter.ToUInt64(Next(sizeof(ulong)));
        public bool TryNext(out ulong value)
        {
            if (TryNext(sizeof(ulong), out byte[] bytes))
            {
                value = ByteConverter.ToUInt64(bytes);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public long NextInt64() => ByteConverter.ToInt64(Next(sizeof(long)));
        public bool TryNext(out long value)
        {
            if (TryNext(sizeof(long), out byte[] bytes))
            {
                value = ByteConverter.ToInt64(bytes);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public float NextSingle() => ByteConverter.ToSingle(Next(sizeof(float)));
        public bool TryNext(out float value)
        {
            if (TryNext(sizeof(float), out byte[] bytes))
            {
                value = ByteConverter.ToSingle(bytes);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public double NextDouble() => ByteConverter.ToDouble(Next(sizeof(double)));
        public bool TryNext(out double value)
        {
            if (TryNext(sizeof(double), out byte[] bytes))
            {
                value = ByteConverter.ToDouble(bytes);
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public string NextString() => ByteConverter.ToString(Next((char)3));
        public bool TryNext(out string str)
        {
            if (Next() != 2)
                throw new Exception("Next value was not a string!");

            if (TryNext((char)3, out byte[] bytes))
            {
                str = ByteConverter.ToString(bytes);
                return true;
            }
            else
            {
                str = string.Empty;
                return false;
            }
        }

        public string NextLine() => ByteConverter.ToString(Next('\n'));
        public bool TryNextLine(out string str)
        {
            if (TryNext('\n', out byte[] bytes))
            {
                str = ByteConverter.ToString(bytes);
                return true;
            }
            else
            {
                str = string.Empty;
                return false;
            }
        }

        public byte[] ReadUntil(string pattern, bool retAtEnd) => ReadUntil(Encoding.UTF8.GetBytes(pattern), retAtEnd);
        public byte[] ReadUntil(byte[] pattern, bool retAtEnd)
        {
            List<byte> result = [];

            while(_current < _data.Length - pattern.Length)
            {
                bool match = true;
                for(int i = 0; i < pattern.Length; i++)
                {
                    if (_data[_current + i] == pattern[i])
                        continue;

                    match = false;
                    break;
                }

                if (match)
                    return [.. result];

                result.Add(_data[_current]);
                _current++;
            }

            if (retAtEnd)
                return [.. result];

            throw new Exception("Could not match pattern!");
        }

    }

}
