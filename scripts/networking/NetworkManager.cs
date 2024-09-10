using Godot;
using System;
using System.Net;
using System.Net.Sockets;


namespace Game.Networking
{
    public partial class NetworkManager : Node
    {
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
        }

        public static void ServerUpdate(double delta)
        {
            // foreach (var player in GameManager.Instance.m_ActivePlayers)
            // {
            // }
        }
    }
}
