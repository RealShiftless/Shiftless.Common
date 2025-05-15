namespace Shiftless.Common.Registration
{
    public sealed class RegistryBuilder
    {
        // Values
        public readonly Registry Registry;

        private bool _isLocked = false;

        private List<RegistryItem> _items = [];
        private List<string> _itemNames = [];


        // Properties
        public bool IsLocked => _isLocked;


        // Constructor
        internal RegistryBuilder(Registry registry) => Registry = registry;


        // Func
        public T Register<T>(string name, T item) where T : RegistryItem
        {
            if (_isLocked)
                throw new InvalidOperationException($"{nameof(RegistryItem)} was locked!");

            if (_items.Count >= Registry.MAX_ITEMS)
                throw new OutOfMemoryException($"Max registry items reached! ({Registry.MAX_ITEMS})");

            item.Initialize(Registry, (ushort)_itemNames.Count);

            _itemNames.Add(name);
            _items.Add(item);

            return item;
        }

        internal (RegistryItem[], string[]) Build()
        {
            _isLocked = true;

            return ([.. _items], [.. _itemNames]);
        }
    }
}
