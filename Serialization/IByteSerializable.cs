namespace Shiftless.Common.Serialization
{
    public interface IByteSerializable
    {
        byte[] Serialize();
        void Deserialize(ByteStream stream);
    }
}
