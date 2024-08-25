using Godot;
using static InputActions;

public class FallState : PlayerState
{
    public override PlayerState OnEnterState(Player player)
    {
        return null;
    }

    public override PlayerState HandleInput(Player player, InputEvent @event)
    {
        // Checking mouse button events
        if (@event is InputEventMouseButton mouseButtonEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            // Transition to the attack state if the attack button is pressed
            if (mouseButtonEvent.IsActionPressed(s_AttackLight))
            {
                return new AttackLightState();
            }
        }

        // DEBUG: This printed out while not dodging!! so the input is being eaten
        // This prints out for both the keyboard key and the mouse button for dodge
        // if (Input.IsActionJustPressed(s_MoveDodge))
        // {
        //     GD.Print("Dodge pressed");
        // }

        return null;
    }

    public override PlayerState HandleKeyboardInput(Player player, InputEvent @event)
    {
        // DEBUG: This printed out while not dodging!! so the input is being eaten
        // This only prints out when I press the keyboard key for dodge, not the mouse button
        // if (Input.IsActionJustPressed(s_MoveDodge))
        // {
        //     GD.Print("Dodge pressed");
        // }

        return null;
    }

    public override PlayerState Process(Player player, double delta)
    {
        // DEBUG: This printed out while not dodging!! so the input is being eaten
        // if (Input.IsActionJustPressed(s_MoveDodge))
        // {
        //     GD.Print("Dodge pressed");
        // }

        return null;
    }

    public override PlayerState PhysicsProcess(Player player, ref Vector3 velocity, double delta)
    {
        float fallSpeedMovementFactor = 1.0f;
        player.ApplyMovementInputToVector(ref velocity, fallSpeedMovementFactor);

        // Transition to the idle state if the player is not moving
        if (velocity.Length() == 0)
        {
            return new IdleState();
        }

        // Transition to the move state if the player is on the floor and moving
        if (player.IsOnFloor() && (velocity.X != 0 || velocity.Z != 0))
        {
            return new MoveState();
        }

        // BUG: checking for Input.IsActionJustPressed in physics process causes inputs to sometimes be eaten
        if (Input.IsActionJustPressed(s_MoveDodge))
        {
            return new DodgeState();
        }

        return null;
    }

    public override void OnExitState(Player player)
    {
    }
}

