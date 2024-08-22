using Godot;
using static InputActions;
using ActionTypes;


public class DodgeState : PlayerState
{
    private float dodgeTimeSec; // How long the dodge will last

    // Dodge speeds
    private float rollSpeedFactor = 0.5f;
    private float dashSpeedFactor = 5.0f;


    public override void OnEnterState(Player player)
    {

        if (player.m_DodgeType == DodgeType.Roll)
        {
            dodgeTimeSec = 0.4f;
            rollSpeedFactor *= player.m_DodgeSpeedFactor;

            // Play the roll animation
        }
        else if (player.m_DodgeType == DodgeType.Dash)
        {
            dodgeTimeSec = 0.2f;
            dashSpeedFactor *= player.m_DodgeSpeedFactor;

            // Play the dash animation
        }
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
        // TODO: when the player is moveing in any direction, the dodge should be whever the player is looking/aiming

        // Dodge the player - Affected by player's dodge type, speed factor, regular movement speed, dodge time, and movement direction
        if (dodgeTimeSec > 0)
        {
            Vector3 velocity = player.Velocity;

            if (player.m_DodgeType == DodgeType.Roll)
            {
                if (player.IsOnFloor())
                {
                    // Apply rolling motion on the XZ plane
                    velocity.X = player.m_MovementDirection.X * player.m_MovementSpeed * rollSpeedFactor;
                    velocity.Z = player.m_MovementDirection.Z * player.m_MovementSpeed * rollSpeedFactor;
                    velocity.Y = 0; // Ensure no vertical movement
                }
            }
            else if (player.m_DodgeType == DodgeType.Dash)
            {
                // Apply dash motion on the XZY plane
                velocity.X = player.m_MovementDirection.X * player.m_MovementSpeed * dashSpeedFactor;
                velocity.Z = player.m_MovementDirection.Z * player.m_MovementSpeed * dashSpeedFactor;
                velocity.Y = 0; // Ensure no vertical movement
                // TODO: // To where the player is looking/aiming
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
