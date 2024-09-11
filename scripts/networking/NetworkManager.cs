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

            // TODO: check for server online if so connect to it, else try and start a server
            //
            string serverIp = System.Environment.GetEnvironmentVariable("SERVER_IP");
            string serverPortString = System.Environment.GetEnvironmentVariable("SERVER_PORT");

            if (string.IsNullOrEmpty(serverIp) || string.IsNullOrEmpty(serverPortString) || !int.TryParse(serverPortString, out int serverPort))
            {
                GD.Print("SERVER_IP and SERVER_PORT environment variables must be set or Invalid.");
                return;
            }

            m_RelayServerEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
        }

        public static void ServerUpdate(double delta)
        {
            // foreach (var player in GameManager.Instance.m_ActivePlayers)
            // {
            // }
        }
    }
}
