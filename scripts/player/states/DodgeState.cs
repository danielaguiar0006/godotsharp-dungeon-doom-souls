using Godot;
using static InputActions;
using ActionTypes;


public class DodgeState : PlayerState
{
    // How long the dodge will last, animations and all
    private float dodgeTimeSec;

    // Dodge speeds
    private float rollSpeedFactor = 1.25f;
    private float dashSpeedFactor = 5.0f;

    // The target camera transform, used to determine the direction the player is looking
    Transform3D targetCameraTransform;


    public override PlayerState OnEnterState(Player player)
    {
        switch (player.m_DodgeType)
        {
            case DodgeType.Roll:
                // NOTE: This is done here and not in the physics process because physics
                // process is called once every physics tick, not asap, which causes a delay bug
                if (!player.IsOnFloor())
                {
                    GD.Print("Rolling in the air is not allowed");
                    return new MoveState();
                }

                GD.Print("Rolling!!!");
                dodgeTimeSec = 0.5f;
                rollSpeedFactor *= player.m_DodgeSpeedFactor;

                // Play the roll animation
                break;
            case DodgeType.Dash:
                GD.Print("Dashing!!!");
                dodgeTimeSec = 0.2f;
                dashSpeedFactor *= player.m_DodgeSpeedFactor;

                // Play the dash animation
                break;
        }

        targetCameraTransform = player.GetNode<Node3D>("CameraPivot/TargetCamera").GlobalTransform;

        return null;
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
        dodgeTimeSec -= (float)delta;

        return null;
    }

    // Dodge the player - Affected by player's dodge type, speed factor, regular movement speed, dodge time, and movement direction
    public override PlayerState PhysicsProcess(Player player, ref Vector3 velocity, double delta)
    {
        if (dodgeTimeSec < 0.0f)
        {
            return new MoveState();
        }

        Vector3 wishDirection;
        if (player.m_MovementDirection != Vector3.Zero) // if player is moving, dodge in the direction the player is moving
        {
            wishDirection = player.m_MovementDirection; // NOTE: m_MovementDirection is already normalized
        }
        else  // else dodge in the direction the player is looking
        {
            wishDirection = -targetCameraTransform.Basis.Z.Normalized(); // Forward direction of the camera
        }


        switch (player.m_DodgeType)
        {
            case DodgeType.Roll:
                player.ApplyMovementDirectionToVector(ref velocity, wishDirection, rollSpeedFactor);
                // NOTE: Vertical velocity is not disabled to enable gravity, letting the player roll off ledges
                break;
            case DodgeType.Dash:
                player.ApplyMovementDirectionToVector(ref velocity, wishDirection, dashSpeedFactor);
                velocity.Y = 0; // Disables vertical movement (including gravity) - IDK: Maybe another dodge type that allows vertical movement
                break;
        }

        return null;
    }

    public override void OnExitState(Player player)
    {
    }
}
