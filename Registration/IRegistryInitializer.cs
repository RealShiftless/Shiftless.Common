using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shiftless.Common.Registration
{
    public interface IRegistryInitializer
    {
        void Initialize(RegistryBuilder registry);
    }
}
