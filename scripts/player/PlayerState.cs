using Godot;

public class PlayerState
{
    // Called when the state is entered
    public virtual void OnEnterState(Player player)
    {
        // Change the player's animation state here for example
    }

    // For gameplay input - That isn't already handled by Godot through the InputMap system.
    // If it is handled by the InputMap system, is should be accessible through the Input singleton.
    // returns the new state if the state changes, otherwise returns null
    public virtual PlayerState HandleInput(Player player, InputEvent @event)
    {
        return null;
    }

    // Note: To handle keyboard events, use this funciton instead for performance reasons.
    // returns the new state if the state changes, otherwise returns null
    public virtual PlayerState HandleKeyboardInput(Player player, InputEvent @event)
    {
        return null;
    }

    // Called during the processing step of the main loop. 
    // Processing happens at every frame and as fast as possible,
    // so the `delta` time since the previous frame is not constant (`delta` is in seconds).
    // returns the new state if the state changes, otherwise returns null
    public virtual PlayerState Process(Player player, double delta)
    {
        return null;
    }

    // Called during the physics processing step of the main loop.
    // Physics processing means that the frame rate is synced to the physics,
    // i.e. the `delta` variable should be constant (`delta` is in seconds).
    // returns the new state if the state changes, otherwise returns null
    public virtual PlayerState PhysicsProcess(Player player, double delta)
    {
        return null;
    }
}
