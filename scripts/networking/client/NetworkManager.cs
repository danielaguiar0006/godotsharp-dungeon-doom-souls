using Godot;
using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading.Tasks;
using dds_shared_lib;


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
                await PacketManager.SendPacket(joinPacket, m_LocalPlayerClient, PROTOCOL_ID);

                // TODO: Add a timeout for the connection attempt

                // receive the packet from the server
                UdpReceiveResult receiveResult = await m_LocalPlayerClient.ReceiveAsync();
                Packet recievedPacket = PacketManager.GetPacketFromData(receiveResult.Buffer, PROTOCOL_ID); // NOTE: This can return null!
                if (recievedPacket == null || recievedPacket.m_PacketType != Packet.PacketType.GamePacket)
                {
                    GD.PrintErr("[ERROR] Failed to connect to server");
                    return;
                }

                // Getting the client ID and the server message from the recieved packet
                string serverMessage = System.Text.Encoding.UTF8.GetString(recievedPacket.m_Data);

                // Setting the client ID and printing the server message
                GD.Print("[INFO] Succesfully connected to server");
                GD.Print(serverMessage);

                // TODO: _ = ListenForMessages();
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

        public void DisconnectFromServer()
        {
            if (m_LocalPlayerClient != null)
            {
                // Creating a new disconnect packet
                GamePacket disconnectPacket = new GamePacket(GamePacket.OpCode.PlayerLeave);
                disconnectPacket.m_Data = new byte[] { (byte)0 };

                // send a message to the server to register the client
                _ = PacketManager.SendPacket(disconnectPacket, m_LocalPlayerClient, PROTOCOL_ID);

                m_LocalPlayerClient.Close();
                m_LocalPlayerClient.Dispose();
            }

            GD.PrintErr("m_LocalPlayerClient is null or m_LocalPlayer.m_ClientId is null");
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
