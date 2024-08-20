using Godot;
using static InputActions;

public class MoveState : PlayerState
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
        if (Input.IsActionPressed(s_MoveDodge))
        {
            return new DodgeState();
        }

        return null;
    }

    public override PlayerState PhysicsProcess(Player player, double delta)
    {
        // Instead of constantly making calls to this.Velocity, cache it for better performance and work with the new variable instead
        Vector3 velocity = player.Velocity;

        // TODO: Seperate jumping/falling into it's own state
        // Add the gravity
        if (!player.IsOnFloor())
            velocity.Y -= player.m_Gravity * (float)delta;

        // Handle Jump
        if (Input.IsActionJustPressed(s_MoveJump) && player.IsOnFloor())
            velocity.Y = player.m_JumpVelocity;

        // Get the input direction and handle the movement/deceleration.
        Vector2 inputDir = Input.GetVector(s_MoveLeft, s_MoveRight, s_MoveForward, s_MoveBackward);
        Vector3 direction = (player.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * player.m_Speed;
            velocity.Z = direction.Z * player.m_Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(player.Velocity.X, 0, player.m_Speed);
            velocity.Z = Mathf.MoveToward(player.Velocity.Z, 0, player.m_Speed);
        }

        // Move the player
        player.Velocity = velocity;
        player.MoveAndSlide();

        // TODO: Implement walking and running states

        // Transition to the idle state if the player is not moving
        if (player.Velocity.Length() == 0)
        {
            return new IdleState();
        }

        return null;
    }
}

