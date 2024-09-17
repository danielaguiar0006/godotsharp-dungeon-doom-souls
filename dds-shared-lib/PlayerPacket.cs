using System.IO;


namespace dds_shared_lib;

public class PlayerPacket : Packet
{
    public enum OpCode
    {
        PlayerMove,
        PlayerDie
    }

    // Serialize the packet data into a byte array
    public override void Write(BinaryWriter writer)
    {

    }

    // Deserialize the packet data from a byte array
    public override void Read(BinaryReader reader)
    {

    }
}
