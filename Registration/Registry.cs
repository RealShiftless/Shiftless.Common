namespace Shiftless.Common.Registration
{
    public sealed class Registry
    {
        // Constants
        public const int MAX_ITEMS = ushort.MaxValue;


        // Values
        public readonly Registrar Registrar;
        public readonly ushort Id;

        private readonly IRegistryInitializer _initializer;

        private RegistryItem[] _items = null!;
        private string[] _itemNames = null!;



        // Properties
        public bool IsInitialized => _items != null;

        public string Name => Registrar.GetRegistryName(Id);

        public IEnumerable<RegistryItem> Items => _items;


        // Constructor
        internal Registry(Registrar registrar, ushort id, IRegistryInitializer initializer)
        {
            Registrar = registrar;
            Id = id;

            _initializer = initializer;
        }


        // Func
        internal void Initialize()
        {
            RegistryBuilder builder = new(this);
            _initializer.Initialize(builder);

            (_items, _itemNames) = builder.Build();
        }


        // Getters
        public RegistryItem GetItem(int id) => _items[id];
        public T GetItem<T>(int id) where T : RegistryItem => (T)GetItem(id);

        public RegistryItem GetItem(string name) => GetItem(GetItemId(name));
        public T GetItem<T>(string name) where T : RegistryItem => (T)GetItem(name);

        public ushort GetItemId(string name)
        {
            int id = Array.FindIndex(_itemNames, itemName => itemName == name);

            if (id == -1)
                throw new ArgumentException($"No registry of name {name} found!");

            return (ushort)id;
        }
        public bool TryGetItemId(string name, out ushort id)
        {
            try
            {
                id = GetItemId(name);
                return true;
            }
            catch (ArgumentException)
            {
                id = default;
                return false;
            }
        }

        public string GetItemName(int id) => _itemNames[id];
    }
}
