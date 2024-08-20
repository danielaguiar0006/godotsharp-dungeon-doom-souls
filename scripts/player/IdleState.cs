using Godot;
using static InputActions;

public class IdleState : PlayerState
{
    public override void OnEnterState(Player player)
    {

    }

    public override PlayerState HandleInput(Player player, InputEvent @event)
    {
        // TODO: Most of not all of the camera movement code will be duplicated in most states. Figure out a way to refactor this.

        // Checking mouse button events
        if (@event is InputEventMouseButton mouseButtonEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            // Transition to the attack state if the attack button is pressed
            if (mouseButtonEvent.IsActionPressed(s_AttackOne))
            {
                return new AttackState();
            }
        }

        // Checking if the mouse is active
        if (@event is InputEventMouseMotion mouseInputEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            player.m_YawInput = -mouseInputEvent.Relative.X;
            player.m_PitchInput = -mouseInputEvent.Relative.Y;
        }
        else
        { // Reset the mouse input if the mouse is not active
          // InputEventMouseMotion.Relative is not reset to 0 when the mouse is not active
            player.m_YawInput = 0.0f;
            player.m_PitchInput = 0.0f;
        }

        // Rotate player
        player.RotateY(Mathf.DegToRad(player.m_YawInput * player.m_MouseSensitivity));
        // Rotate camera pivot and clamp the pitch
        player.m_CameraPivot.RotateX(Mathf.DegToRad(player.m_PitchInput * player.m_MouseSensitivity));

        // HACK: This does not work for some reason:
        // m_CameraPivot.Rotation.X = Mathf.Clamp(m_CameraPivot.Rotation.X, m_PitchLowerLimit, m_PitchUpperLimit);

        Vector3 cameraRotation = player.m_CameraPivot.Rotation;
        cameraRotation.X = Mathf.Clamp(cameraRotation.X, player.m_PitchLowerLimit, player.m_PitchUpperLimit);
        player.m_CameraPivot.Rotation = cameraRotation;

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
