using System;
using System.Collections.Generic;
using System.Text;

namespace Shiftless.Common.Serialization
{
    public interface IByteSerializable
    {
        byte[] Serialize();
        void Deserialize(ByteStream stream);
    }
}
