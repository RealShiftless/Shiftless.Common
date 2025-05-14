using Shiftless.Common.Serialization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Common.Registration
{
    public sealed class RegistrarMap
    {
        // Constants
        private const int STRING_KEY = 432;


        // Values
        private readonly ReadOnlyDictionary<uint, uint> _map;


        // Indexer
        public uint this[uint mappedId] => GetId(mappedId);


        // Constructor
        private RegistrarMap(ReadOnlyDictionary<uint, uint> map)
        {
            _map = map;
        }


        // Func
        public uint GetId(uint mappedId) => _map[mappedId];

        public static void Save(string path, Registrar registrar)
        {
            ByteWriter writer = new ByteWriter();
            registrar.EnumerateItems(regItem =>
            {
                writer.Write(regItem.FullId);
                writer.Write(XorString(regItem.FullName));
            });
            writer.Save(path);
        }

        public static RegistrarMap Load(Registrar registrar, string path, out string[] missingItems)
        {
            List<string> missingItemList = [];
            Dictionary<uint, uint> map = [];

            ByteStream stream = new(path);

            while(!stream.IsAtEnd)
            {
                uint mappedId = stream.ReadUInt32();
                string fullName = XorString(stream.ReadString());

                if(!registrar.TryGetItemId(fullName, out uint actualId))
                {
                    missingItemList.Add(fullName);
                    continue;
                }

                map.Add(mappedId, actualId);
            }

            missingItems = [..missingItemList];
            return new(map.AsReadOnly());
        }

        private static string XorString(string str)
        {
            char[] result = new char[str.Length];
            for (int i = 0; i < str.Length; i++)
                result[i] = (char)(str[i] ^ STRING_KEY);
            return new string(result);
        }
    }
}
