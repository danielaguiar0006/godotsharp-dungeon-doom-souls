using Godot;
using System.Diagnostics;
using System.Collections.Generic;
using Game.Networking;


public partial class GameManager : Node
{
    [Export]
    public bool m_IsOnline { get; private set; } = true;
    [Export]
    public ushort m_CurrentLevel = 0;
    [Export]
    protected Node3D m_Level;

    // initialize a list of mob references
    //public List<Mob> m_ActiveMobs { get; private set; } = new List<Mob>();
    public Dictionary<string, Player> m_ActivePlayers { get; private set; } = new Dictionary<string, Player>();
    public List<Monster> m_ActiveMonsters { get; private set; } = new List<Monster>();
    public List<NPC> m_ActiveNPCs { get; private set; } = new List<NPC>();
    public List<Boss> m_ActiveBosses { get; private set; } = new List<Boss>();

    private PackedScene m_PlayerScene;
    private Player m_localPlayer;

    private static GameManager _instance;

    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GD.PrintErr("GameManager instance is not initialized.");
            }
            return _instance;
        }
    }

    // private initializer to prevent external instantiation
    private GameManager() { }

    public override void _Ready()
    {
        if (_instance != null)
        {
            GD.PrintErr("Multiple instances of GameManager detected!");
            QueueFree();
            return;
        }
        _instance = this;
        GD.Print("GameManager Ready");

        // NOTE: Must load resources before calling StartGame()
        m_PlayerScene = ResourceLoader.Load<PackedScene>("res://scenes/prefabs/player.tscn");

        if (m_CurrentLevel == 0)
        {
            StartGame();
        }

        if (m_IsOnline)
        {
            PackedScene networkManagerScene = ResourceLoader.Load<PackedScene>("res://scenes/network_manager.tscn");
            AddChild(networkManagerScene.Instantiate<NetworkManager>());
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        //if (m_IsOnline) { NetworkManager.ServerUpdate(delta); }
    }

    public void StartGame()
    {
        m_localPlayer = (Player)SpawnMob(Mob.MobType.Player, new Vector3(0, 0, 0));
    }

    // NOTE: playerId defaults to "0" for the player
    public Mob SpawnMob(Mob.MobType mobType, Vector3 position, string playerId = "0")
    {
        switch (mobType)
        {
            case Mob.MobType.Player:
                Player playerInstance = m_PlayerScene.Instantiate<Player>();
                m_ActivePlayers.Add(playerId, playerInstance);
                m_Level.AddChild(playerInstance);
                playerInstance.Position = position;
                GD.Print($"Spawning Player at {position}");
                return playerInstance;
            default:
                GD.PrintErr("Invalid mob type");
                return null;

                // TODO: implement spawn logic for other mob types
                // case Mob.MobType.Monster:
                //     // TODO: spawn a monster
                //     GD.Print("Spawning Monster");
                //     break;
                // case Mob.MobType.NPC:
                //     // TODO: spawn a NPC
                //     GD.Print("Spawning NPC");
                //     break;
                // case Mob.MobType.Boss:
                //     // TODO: spawn a Boss
                //     GD.Print("Spawning Boss");
                //     break;
        }
    }

    public void DespawnMob(Mob mob)
    {
        if (mob == null)
        {
            GD.PrintErr("Unable to Despawn Mob: Mob is null");
            return;
        }

        switch (mob.m_MobType)
        {
            case Mob.MobType.Player:
                GD.PrintErr("[ERROR] Unable to despawn player, use DespawnPlayer() instead");
                break;
            case Mob.MobType.Monster:
                m_ActiveMonsters.Remove((Monster)mob);
                break;
            case Mob.MobType.NPC:
                m_ActiveNPCs.Remove((NPC)mob);
                break;
            case Mob.MobType.Boss:
                m_ActiveBosses.Remove((Boss)mob);
                break;
        }

        // remove the mob from the level/scene
        m_Level.RemoveChild(mob);
    }

    public void DespawnPlayer(string playerId)
    {
        if (m_ActivePlayers.TryGetValue(playerId, out Player player))
        {
            m_ActivePlayers.Remove(playerId);
            m_Level.RemoveChild(player);
        }
        else
        {
            GD.PrintErr("[ERROR] Unable to despawn player, player not found");
        }
    }

    public override void _ExitTree()
    {
        GD.Print("GameManager exiting");
        NetworkManager.Instance.DisconnectFromServer();
    }
}
