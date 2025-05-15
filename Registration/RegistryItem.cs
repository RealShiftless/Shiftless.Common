namespace Shiftless.Common.Registration
{
    public abstract class RegistryItem
    {
        // Values
        private Registry _registry = null!;
        private ushort _id;


        // Properties
        public Registry Registry => _registry;
        public ushort Id => _id;

        public string Name => _registry.GetItemName(Id);

        public string FullName => $"{_registry.Name}:{Name}";
        public uint FullId => (uint)(_registry.Id << 16 | _id);


        // Func
        internal void Initialize(Registry registry, ushort id)
        {
            _registry = registry;
            _id = id;
        }
    }
}
