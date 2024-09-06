using Godot;
using System;
using System.Net;
using System.Net.Sockets;

namespace Game.Networking
{
    public partial class UdpRelayTest : Node
    {
        private UdpClient client;
        private IPEndPoint relayServerEndpoint;

        public override void _Ready()
        {
            string serverIp = System.Environment.GetEnvironmentVariable("SERVER_IP");
            string serverPortString = System.Environment.GetEnvironmentVariable("SERVER_PORT");

            if (string.IsNullOrEmpty(serverIp) || string.IsNullOrEmpty(serverPortString) || !int.TryParse(serverPortString, out int serverPort))
            {
                GD.Print("SERVER_IP and SERVER_PORT environment variables must be set or Invalid.");
                return;
            }

            StartClient(serverIp, serverPort);
        }

        public void StartClient(string serverIp, int serverPort)
        {
            client = new UdpClient();
            client.Client.Blocking = false;
            relayServerEndpoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

            // Send a message to the relay server
            string message = "Hello, Relay Server! (from client)";
            byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, relayServerEndpoint);
        }

        public override void _Process(double delta)
        {
            try // Receive a message from the relay server
            {
                // Unnecessary because we already know the server's IP and port
                // IPEndPoint relayEndpoint = new IPEndPoint(IPAddress.Any, 0);

                if (client.Available > 0)
                {
                    byte[] relayData = client.Receive(ref relayServerEndpoint);
                    GD.Print("Received: " + System.Text.Encoding.UTF8.GetString(relayData));
                }
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.WouldBlock)
                {
                    GD.Print("No received yet.");
                }
                else
                {
                    GD.Print("Socket error: " + ex.Message);
                }
            }
            // finally
            // {
            //     client.Close();
            // }
        }
    }
}
