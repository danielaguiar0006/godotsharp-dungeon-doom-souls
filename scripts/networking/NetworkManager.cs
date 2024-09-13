using Godot;
using System;
using System.Net;
using System.Net.Sockets;


namespace Game.Networking
{
    public partial class NetworkManager : Node
    {
        private IPEndPoint m_RelayServerEndpoint;

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

            // NOTE: this assumes that the first and only player has been spawned and added to the active player list
            AssignPlayerUdpClient();

            // TODO: check for server online if so connect to it, else try and start a server
            string serverIp = System.Environment.GetEnvironmentVariable("SERVER_IP");
            string serverPortString = System.Environment.GetEnvironmentVariable("SERVER_PORT");

            if (string.IsNullOrEmpty(serverIp) || string.IsNullOrEmpty(serverPortString) || !int.TryParse(serverPortString, out int serverPort))
            {
                GD.PrintErr("SERVER_IP and SERVER_PORT environment variables aren't set, or are Invalid.");
                return;
            }

            m_RelayServerEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

            ConnectToServer();
        }

        private async void ConnectToServer()
        {
            GD.Print("[INFO] Connecting to server...");

            Player localPlayer = GameManager.Instance.m_ActivePlayers[0];
            localPlayer.m_UdpClient.Connect(m_RelayServerEndpoint);

            // send a message to the server to register the client
            // TODO: should be a player join packet or something
            localPlayer.m_UdpClient.Send(new byte[] { (byte)1 }, 1);

            // receive the client id from the server
            UdpReceiveResult receiveResult = await localPlayer.m_UdpClient.ReceiveAsync();
            localPlayer.SetClientId(receiveResult.Buffer[0]);

            if (localPlayer.m_ClientId != null)
            {
                GD.Print("[INFO] Succesfull, Client ID: " + localPlayer.m_ClientId);
            }
            else
            {
                GD.PrintErr("[ERROR] Client ID is null");
            }
        }

        public static void ServerUpdate(double delta)
        {
            // foreach (var player in GameManager.Instance.m_ActivePlayers)
            // {
            // }
        }

        private void AssignPlayerUdpClient()
        {
            GameManager.Instance.m_ActivePlayers[0].m_UdpClient = new UdpClient();
        }
    }
}
