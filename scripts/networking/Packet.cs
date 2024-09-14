using System.IO;


namespace Game.Networking
{
    public abstract class Packet
    {
        public enum PacketType
        {
            GamePacket,
            PlayerPacket,
        }
        public PacketType m_PacketType;
        public byte[] m_Data;

        // Serialize the packet data into a byte array
        public abstract void Write(BinaryWriter writer);

        // Deserialize the packet data from a byte array
        public abstract void Read(BinaryReader reader);

        public void PrefixProtocolID(BinaryWriter writer)
        {
            writer.Write(NetworkManager.PROTOCOL_ID);
        }
    }
}

// public struct Packet
// {
//     public PacketType m_PacketType;
//
//     // Other properties and methods
//     public enum PacketType
//     {
//         PlayerPacket,
//         EnemyPacket,
//         ItemPacket,
//         GamePacket,
//         SchemaPacket
//         // Other general categories
//     }
//
//     public enum GamePacketType
//     {
//         PlayerJoin
//         //Start,
//         //End,
//         //Over,
//         //State,
//         //ServerUpdate,
//         // Other game-specific actions
//     }
// }
