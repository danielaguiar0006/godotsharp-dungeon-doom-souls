using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;


namespace Game.Networking
{
    public partial class NetworkManager : Node
    {
        public const uint PROTOCOL_ID = 13439;

        private IPEndPoint m_RelayServerEndpoint;
        private Player m_LocalPlayer;
        private UdpClient m_LocalPlayerClient;

        private static NetworkManager _instance;

        public static NetworkManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GD.PrintErr("NetworkManager instance is not initialized.");
                }
                return _instance;
            }
        }

        // private initializer to prevent external instantiation
        private NetworkManager() { }

        public override void _Ready()
        {
            if (_instance != null)
            {
                GD.PrintErr("Multiple instances of NetworkManager detected!");
                QueueFree();
                return;
            }

            _instance = this;
            GD.Print("NetworkManager Ready");


            // Getting the local player and its client
            // NOTE: this assumes that the first and only player has been spawned and added to the active player list
            m_LocalPlayer = GameManager.Instance.m_ActivePlayers[0];
            AssignPlayerUdpClient();
            m_LocalPlayerClient = m_LocalPlayer.m_UdpClient;

            // TODO: check for server online if so connect to it, else try and start a server

            string serverIp = System.Environment.GetEnvironmentVariable("SERVER_IP");
            string serverPortString = System.Environment.GetEnvironmentVariable("SERVER_PORT");

            if (string.IsNullOrEmpty(serverIp) || string.IsNullOrEmpty(serverPortString) || !int.TryParse(serverPortString, out int serverPort))
            {
                GD.PrintErr("SERVER_IP and SERVER_PORT environment variables aren't set, or are Invalid.");
                return;
            }

            m_RelayServerEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

            // Can't use await here because _Ready() is not async
            _ = ConnectToServer();
        }

        private async Task SendPacket(UdpClient localClient, Packet packet)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    packet.PrefixWithProtocolID(writer, NetworkManager.PROTOCOL_ID);
                    packet.Write(writer);
                    byte[] serializedPacketData = memoryStream.ToArray();
                    await localClient.SendAsync(serializedPacketData, serializedPacketData.Length);
                }
            }
        }

#nullable enable
        private Packet? GetPacketFromData(byte[] serializedPacketData)
        {
            using (MemoryStream memoryStream = new MemoryStream(serializedPacketData))
            {
                using (BinaryReader reader = new BinaryReader(memoryStream))
                {
                    // Check if the protocol ID is valid and matches
                    uint receivedProtocolId = reader.ReadUInt32();
                    if (receivedProtocolId != NetworkManager.PROTOCOL_ID)
                    {
                        GD.PrintErr("[ERROR] Invalid protocol ID: " + receivedProtocolId);
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
                            GD.PrintErr("[ERROR] Invalid packet type: " + packetType);
                            return null;
                    }
                }
            }
        }

        private async Task ConnectToServer()
        {
            try
            {
                // Creating a new join packet
                GamePacket joinPacket = new GamePacket(GamePacket.OpCode.PlayerJoin);
                joinPacket.m_Data = new byte[] { (byte)0 };

                GD.Print("[INFO] Connecting to server...");
                m_LocalPlayerClient.Connect(m_RelayServerEndpoint);

                // send a message to the server to register the client
                await SendPacket(m_LocalPlayerClient, joinPacket);

                // TODO: Add a timeout for the connection attempt

                // receive the client id from the server
                UdpReceiveResult receiveResult = await m_LocalPlayerClient.ReceiveAsync();
                Packet? recievedPacket = GetPacketFromData(receiveResult.Buffer);

                // TODO: Create a specific AssignClientId() method for this instead of managing the bits directly
                m_LocalPlayer.SetClientId(recievedPacket?.m_Data[0]); // TODO: this should probably be a guuid or something...

                if (m_LocalPlayer.m_ClientId != null && recievedPacket != null)
                {
                    GD.Print("[INFO] Succesfully connected to server, Client ID: " + m_LocalPlayer.m_ClientId);
                    // TODO: _ = ListenForMessages();
                }
                else
                {
                    GD.PrintErr("[ERROR] Failed to connect to server");
                }
            }
            catch (SocketException ex)
            {
                GD.PrintErr("[ERROR] Network error: " + ex.Message);
            }
            catch (Exception ex)
            {
                GD.PrintErr("[ERROR] An unexpected error occurred: " + ex.Message);
            }
        }

        private void AssignPlayerUdpClient()
        {
            m_LocalPlayer.m_UdpClient = new UdpClient();
        }

        // public static void ServerUpdate(double delta)
        // {
        //     foreach (var player in GameManager.Instance.m_ActivePlayers)
        //     {
        //     }
        // }
    }
}
