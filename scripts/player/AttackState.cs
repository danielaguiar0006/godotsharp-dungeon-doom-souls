using Godot;
using static InputActions;

// TODO: Finish implementing first version of
public class AttackState : PlayerState
{
    // Called when the state is entered - OnEnter
    public override void OnEnterState(Player player)
    {

    }

    // For gameplay input - That isn't already handled by Godot through the InputMap system.
    // If it is handled by the InputMap system, is should be accessible through the Input singleton.
    public override PlayerState HandleInput(Player player, InputEvent @event)
    {
        return null;
    }

    // Note: To handle keyboard events, use this function instead for performance reasons.
    public override PlayerState HandleKeyboardInput(Player player, InputEvent @event)
    {
        return null;
    }

    // Called during the physics processing step of the main loop.
    // Physics processing means that the frame rate is synced to the physics,
    // i.e. the `delta` variable should be constant (`delta` is in seconds).
    public override PlayerState PhysicsProcess(Player player, double delta)
    {
        return null;
    }
}
