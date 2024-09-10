using Godot;
using System.Diagnostics;
using System.Collections.Generic;
using Game.Networking;


public partial class GameManager : Node
{
    [Export]
    bool m_IsOnline = true;
    [Export]
    protected Node3D m_Level;

    // initialize a list of mob references
    //public List<Mob> m_ActiveMobs { get; private set; } = new List<Mob>();
    public List<Player> m_ActivePlayers { get; private set; } = new List<Player>();
    public List<Monster> m_ActiveMonsters { get; private set; } = new List<Monster>();
    public List<NPC> m_ActiveNPCs { get; private set; } = new List<NPC>();
    public List<Boss> m_ActiveBosses { get; private set; } = new List<Boss>();

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

        // TODO: check if isOnline and spawn in the network manager and in the network manager check if a/the server is online.
    }

    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        if (m_IsOnline) { NetworkManager.ServerUpdate(delta); }
    }

    public void spawnMob(Mob mob, Vector3 position)
    {
        if (mob == null)
        {
            GD.PrintErr("Unable to Spawn Mob: Mob is null");
            return;
        }

        // add the mob to the active mobs list
        if (mob.m_MobType == Mob.MobType.Player) { m_ActivePlayers.Add((Player)mob); }
        else if (mob.m_MobType == Mob.MobType.Monster) { m_ActiveMonsters.Add((Monster)mob); }
        else if (mob.m_MobType == Mob.MobType.NPC) { m_ActiveNPCs.Add((NPC)mob); }
        else if (mob.m_MobType == Mob.MobType.Boss) { m_ActiveBosses.Add((Boss)mob); }

        // spawn the mob
        mob.GlobalTransform = new Transform3D(new Basis(), position);
        m_Level.AddChild(mob);
    }

    public void despawnMob(Mob mob)
    {
        Debug.Assert(mob != null, "Unable to Despawn Mob: Mob is null");

        // remove the mob from the active mobs list
        if (mob.m_MobType == Mob.MobType.Player) { m_ActivePlayers.Remove((Player)mob); }
        else if (mob.m_MobType == Mob.MobType.Monster) { m_ActiveMonsters.Remove((Monster)mob); }
        else if (mob.m_MobType == Mob.MobType.NPC) { m_ActiveNPCs.Remove((NPC)mob); }
        else if (mob.m_MobType == Mob.MobType.Boss) { m_ActiveBosses.Remove((Boss)mob); }

        // despawn the mob
        m_Level.RemoveChild(mob);
    }
}
