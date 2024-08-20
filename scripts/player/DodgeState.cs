using Godot;
using static InputActions;

// TODO: Finish implementing first version of
public class DodgeState : PlayerState
{
    public override void OnEnterState(Player player)
    {

    }

    public override PlayerState HandleInput(Player player, InputEvent @event)
    {
        return null;
    }

    public override PlayerState HandleKeyboardInput(Player player, InputEvent @event)
    {
        return null;
    }

    public override PlayerState Process(Player player, double delta)
    {
        return null;
    }

    public override PlayerState PhysicsProcess(Player player, double delta)
    {
        Vector3 velocity = player.Velocity;

        // TODO: If the player is moving in a direction in the XZ plane, dodge in that direction, else just dodge wherever the player is looking (again still locked to the XZ plane)

        // TODO: Implement a dodge cooldown

        // DEBUG: instantly go back to MoveState for now
        return new IdleState();

        return null;
    }
}
