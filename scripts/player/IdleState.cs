using Godot;
using static InputActions;

public class IdleState : PlayerState
{
    public override void OnEnterState(Player player)
    {

    }

    public override PlayerState HandleInput(Player player, InputEvent @event)
    {
        // Checking mouse button events
        if (@event is InputEventMouseButton mouseButtonEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            // Transition to the attack state if the attack button is pressed
            if (mouseButtonEvent.IsActionPressed(s_AttackLight))
            {
                return new AttackState();
            }
        }

        return null;
    }

    public override PlayerState HandleKeyboardInput(Player player, InputEvent @event)
    {
        return null;
    }

    public override PlayerState Process(Player player, double delta)
    {
        if (Input.IsActionPressed(s_MoveForward) || Input.IsActionPressed(s_MoveBackward) || Input.IsActionPressed(s_MoveLeft) || Input.IsActionPressed(s_MoveRight))
        {
            return new MoveState();
        }
        else if (Input.IsActionPressed(s_MoveDodge))
        {
            return new DodgeState();
        }

        return null;
    }

    public override PlayerState PhysicsProcess(Player player, double delta)
    {
        return null;
    }
}
