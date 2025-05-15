using System.Text.RegularExpressions;

namespace Shiftless.Common.Registration
{
    public delegate void RegistrarInitiazationEventHandler(RegistrarBuilder builder);
    public delegate void RegistrarEnumerationEventHandler(RegistryItem item);

    public sealed partial class Registrar
    {
        // Constant values
        public const int MAX_REGISTRIES = ushort.MaxValue;


        // Static Values
        private static readonly Regex _itemNameRegex = ItemNameRegex();


        // Values
        private Registry[] _registries;
        private string[] _registryNames;

        private bool _isInitialized = false;


        // Constructor
        public Registrar(RegistrarInitiazationEventHandler action, bool storesMap = false)
        {
            RegistrarBuilder builder = new(this);
            action(builder);
            (_registries, _registryNames) = builder.Build();

            Initialize();
        }


        // Static Func
        public static bool IsNameValid(string fullName) => _itemNameRegex.IsMatch(fullName);

        private static (string, string) GetNameParts(string fullName)
        {
            if (!IsNameValid(fullName))
                throw new ArgumentException($"Name {fullName} was invalid!");

            string[] nameParts = fullName.Split(':');

            return (nameParts[0], nameParts[1]);
        }


        // Func
        private void Initialize()
        {
            if (_isInitialized)
                throw new InvalidOperationException($"{nameof(Registrar)} was already initialized!");

            _isInitialized = true;

            foreach (Registry registry in _registries)
                registry.Initialize();
        }

        public ushort GetRegistryId(string name)
        {
            int registryId = Array.FindIndex(_registryNames, n => n == name);

            if (registryId == -1)
                throw new ArgumentException($"No registry of name {name} found!");

            return (ushort)registryId;
        }
        public bool TryGetRegistryId(string name, out ushort registryId)
        {
            try
            {
                registryId = GetRegistryId(name);
                return true;
            }
            catch (ArgumentException)
            {
                registryId = default;
                return false;
            }
        }

        public string GetRegistryName(int id) => _registryNames[id];

        public uint GetItemId(string fullName)
        {
            (string registryName, string itemName) = GetNameParts(fullName);

            ushort registryId = GetRegistryId(registryName);
            ushort itemId = GetRegistry(registryId).GetItemId(itemName);

            return (uint)(registryId << 16 | itemId);
        }
        public bool TryGetItemId(string fullName, out uint id)
        {
            try
            {
                id = GetItemId(fullName);
                return true;
            }
            catch (ArgumentException)
            {
                id = default;
                return false;
            }
        }

        public Registry GetRegistry(int id) => _registries[id];
        public Registry GetRegistry(string name) => GetRegistry(GetRegistryId(name));

        public RegistryItem GetItem(string fullName)
        {
            (string registryName, string itemName) = GetNameParts(fullName);

            return GetRegistry(registryName).GetItem(itemName);
        }
        public T GetItem<T>(string fullName) where T : RegistryItem => (T)GetItem(fullName);

        public RegistryItem GetItem(ushort rId, ushort iId) => _registries[rId].GetItem(iId);
        public T GetItem<T>(ushort rId, ushort iId) where T : RegistryItem => (T)GetItem(rId, iId);

        public RegistryItem GetItem(uint packedId) => GetItem((ushort)(packedId >> 16), (ushort)(packedId & 0xFFFF));
        public T GetItem<T>(uint packedId) where T : RegistryItem => (T)GetItem(packedId);


        // Enumeration Func
        public void EnumerateItems(RegistrarEnumerationEventHandler action)
        {
            foreach (Registry registry in _registries)
            {
                foreach (RegistryItem item in registry.Items)
                {
                    action(item);
                }
            }
        }


        // Regex
        [GeneratedRegex(@"^[a-zA-Z_][a-zA-Z0-9_]*:[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled)]
        private static partial Regex ItemNameRegex();
    }
}
