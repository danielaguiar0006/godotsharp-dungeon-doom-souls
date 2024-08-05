using Godot;

public partial class MainMenu : Node
{
    [Export]
    protected Node3D m_World;

    private PanelContainer m_MainMenu;
    private LineEdit m_AddressEntry;

    private const int PORT = 6969;
    ENetMultiplayerPeer m_EnetMultiplayerPeer = new ENetMultiplayerPeer();
    private PackedScene m_MercenaryPrefab;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        m_MainMenu = GetNode<PanelContainer>("CanvasLayer/MainMenu");
        m_AddressEntry = GetNode<LineEdit>("CanvasLayer/MainMenu/MarginContainer/VBoxContainer/AddressEntry");

        m_MercenaryPrefab = (PackedScene)ResourceLoader.Load("res://scenes/prefabs/mercenary.tscn");
    }

    public void _OnHostButtonPressed()
    {
        GD.Print("Host button pressed"); // For debugging

        m_MainMenu.Hide();

        m_EnetMultiplayerPeer.CreateServer(PORT); // TODO: May need to reduce the max number of clients
        Multiplayer.MultiplayerPeer = m_EnetMultiplayerPeer;

        Multiplayer.PeerConnected += id => AddPlayer((int)id);
        AddPlayer(Multiplayer.GetUniqueId());
    }

    public void _OnJoinButtonPressed()
    {
        GD.Print("Join button pressed"); // For debugging

        m_MainMenu.Hide();

        m_EnetMultiplayerPeer.CreateClient("localhost", PORT); // TODO: Change from localhost to the IP address of the server
        Multiplayer.MultiplayerPeer = m_EnetMultiplayerPeer;
    }

    public void AddPlayer(int peerId)
    {
        var player = m_MercenaryPrefab.Instantiate() as Player;
        player.Name = peerId.ToString();

        m_World.AddChild(player);
    }
}
