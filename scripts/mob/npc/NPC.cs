using Godot;
using Game.ActionTypes;
using Game.StateMachines;


public partial class NPC : Mob
{
    NPC()
    {
        SetIsHostile(false);
        SetMobType(MobType.NPC);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
