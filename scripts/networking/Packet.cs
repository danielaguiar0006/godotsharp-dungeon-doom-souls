using System;


namespace Game.Networking
{
    public class Packet
    {
        public PacketType m_PacketType;

        // Other properties and methods
        public enum PacketType
        {
            PlayerPacket,
            EnemyPacket,
            ItemPacket,
            GamePacket,
            SchemaPacket
            // Other general categories
        }

        // public enum GamePacketType
        // {
        //     Start,
        //     End,
        //     Over,
        //     State,
        //     ServerUpdate,
        //     // Other game-specific actions
        // }
    }
}
