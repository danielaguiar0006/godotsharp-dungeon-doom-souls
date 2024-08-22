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
                // Note: This is done here and not in the physics process because physics
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

    public override PlayerState PhysicsProcess(Player player, double delta)
    {
        // Dodge the player - Affected by player's dodge type, speed factor, regular movement speed, dodge time, and movement direction
        if (dodgeTimeSec > 0)
        {
            Vector3 velocity = player.Velocity;

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
                    velocity.X = wishDirection.X * player.m_MovementSpeed * rollSpeedFactor;
                    velocity.Z = wishDirection.Z * player.m_MovementSpeed * rollSpeedFactor;
                    // If the player rolls off a ledge, they will continue to fall
                    if (!player.IsOnFloor())
                        velocity.Y -= player.m_Gravity * (float)delta;
                    break;
                case DodgeType.Dash:
                    velocity.X = wishDirection.X * player.m_MovementSpeed * dashSpeedFactor;
                    velocity.Z = wishDirection.Z * player.m_MovementSpeed * dashSpeedFactor;
                    velocity.Y = 0; // Ensure no vertical movement - IDK: Maybe another dodge type that allows vertical movement
                    break;
            }

            // Move the player
            player.Velocity = velocity;
            player.MoveAndSlide();
        }
        else
        {
            return new MoveState();
        }

        return null;
    }
}
