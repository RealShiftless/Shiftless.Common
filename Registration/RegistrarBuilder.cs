using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Common.Registration
{
    public sealed class RegistrarBuilder
    {
        // Values
        public readonly Registrar Registrar;

        private List<Registry> _registries = [];
        private List<string> _registryNames = [];

        private bool _isLocked = false;


        // Constructor
        internal RegistrarBuilder(Registrar registrar) => Registrar = registrar;


        // Func
        public Registry AddRegistry(string name, IRegistryInitializer initializer)
        {
            if (_isLocked)
                throw new InvalidOperationException($"{nameof(RegistrarBuilder)} was locked!");

            if (_registries.Count >= Registrar.MAX_REGISTRIES)
                throw new OutOfMemoryException($"Max registries reached! ({Registrar.MAX_REGISTRIES})");

            Registry registry = new(Registrar, (ushort)_registries.Count, initializer);

            _registryNames.Add(name);
            _registries.Add(registry);

            return registry;
        }

        internal (Registry[], string[]) Build()
        {
            _isLocked = true;

            return ([.. _registries], [.. _registryNames]);
        }
    }
}
