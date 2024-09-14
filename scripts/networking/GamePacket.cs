using System;
using System.IO;


namespace Game.Networking
{
    // Handles GameManager related packets
    public class GamePacket : Packet
    {
        public enum OpCode
        {
            GameStart,
            GameEnd,
            PlayerJoin,
            PlayerLeave
        }
        public OpCode m_OpCode;

        public GamePacket()
        {
            m_PacketType = PacketType.GamePacket;
        }

        public GamePacket(OpCode opCode)
        {
            m_PacketType = PacketType.GamePacket;
            m_OpCode = opCode;
        }

        // Serialize the packet data into a byte array
        public override void Write(BinaryWriter writer)
        {
            PrefixProtocolID(writer);
            writer.Write((byte)m_PacketType);
            writer.Write((byte)m_OpCode);
            writer.Write(m_Data.Length);
            writer.Write(m_Data);
        }

        // Deserialize the packet data from a byte array
        public override void Read(BinaryReader reader)
        {
            // Reading the packet Protocol ID & Packet Type are done in the NetworkManager.cs
            m_OpCode = (OpCode)reader.ReadByte();
            ushort dataLength = reader.ReadUInt16();
            m_Data = reader.ReadBytes(dataLength);
        }
    }
}
