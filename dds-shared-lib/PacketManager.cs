using Godot;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace dds_shared_lib;
public static class PacketManager
{
    // NOTE: The sever must include an client IPEndPoint to send packets to, Player clients
    // can run UdpClient.Connect() once to connect to the server without specifying a remote endpoint
    public static async Task SendPacket(Packet packet, UdpClient localClient, IPEndPoint? remoteEndpoint = null, uint protocolId = NetworkManager.PROTOCOL_ID)
    {
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(memoryStream))
            {
                packet.PrefixWithProtocolID(writer, protocolId);
                packet.Write(writer);
                byte[] serializedPacketData = memoryStream.ToArray();

                if (remoteEndpoint == null)
                {
                    await localClient.SendAsync(serializedPacketData, serializedPacketData.Length);
                }
                else
                {
                    await localClient.SendAsync(serializedPacketData, serializedPacketData.Length, remoteEndpoint);
                }
            }
        }
    }

#nullable enable
    public static Packet? GetPacketFromData(byte[] serializedPacketData, uint protocolId = NetworkManager.PROTOCOL_ID)
    {
        using (MemoryStream memoryStream = new MemoryStream(serializedPacketData))
        {
            using (BinaryReader reader = new BinaryReader(memoryStream))
            {
                // Check if the protocol ID is valid and matches
                uint receivedProtocolId = reader.ReadUInt32();
                if (receivedProtocolId != protocolId)
                {
#if GODOT
                    GD.PrintErr("[ERROR] Invalid protocol ID: " + receivedProtocolId);
#else
                        Console.WriteLine("[ERROR] Invalid protocol ID: " + receivedProtocolId);
#endif
                    return null;
                }

                // Read the packet type
                Packet.PacketType packetType = (Packet.PacketType)reader.ReadByte();
                // Create the appropriate packet based on the packet type
                switch (packetType)
                {
                    case Packet.PacketType.GamePacket:
                        GamePacket gamePacket = new GamePacket();
                        gamePacket.Read(reader);
                        return gamePacket;
                    case Packet.PacketType.PlayerPacket:
                        PlayerPacket playerPacket = new PlayerPacket();
                        playerPacket.Read(reader);
                        return playerPacket;
                    default:
#if GODOT
                        GD.PrintErr("[ERROR] Invalid packet type: " + packetType);
#else
                            Console.WriteLine("[ERROR] Invalid packet type: " + receivedProtocolId);
#endif
                        return null;
                }
            }
        }
    }

}
