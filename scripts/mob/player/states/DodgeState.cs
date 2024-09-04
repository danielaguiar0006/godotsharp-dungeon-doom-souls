using Godot;
using static InputActions;
using Game.ActionTypes;
using Game.StatsManager;
using Game.StateMachines;


public partial class DodgeState : PlayerState
{
    // How long the dodge will last, animations and all
    private float currentDodgeTimeSec;
    private float totalDodgeTimeSec;

    // Dodge speeds
    private float rollSpeedFactor = 1.75f;
    private float dashSpeedFactor = 10.0f;

    // The target camera transform, used to determine the direction the player is looking
    private Transform3D targetCameraTransform;


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
                currentDodgeTimeSec = 0.5f;
                totalDodgeTimeSec = currentDodgeTimeSec;
                rollSpeedFactor *= player.m_MobStats.m_SpecialStatTypeToAmountFactor[SpecialStatType.DodgeSpeedFactor];

                // Play the roll animation
                break;
            case DodgeType.Dash:
                GD.Print("Dashing!!!");
                currentDodgeTimeSec = 0.33f;
                totalDodgeTimeSec = currentDodgeTimeSec;
                dashSpeedFactor *= player.m_MobStats.m_SpecialStatTypeToAmountFactor[SpecialStatType.DodgeSpeedFactor];

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
        currentDodgeTimeSec -= (float)delta;

        return null;
    }

    // Dodge the player - Affected by player's dodge type, speed factor, regular movement speed, dodge time, and movement direction
    public override PlayerState PhysicsProcess(Player player, ref Vector3 velocity, double delta)
    {
        if (currentDodgeTimeSec < 0.0f)
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
                //rollSpeedFactor *= player.m_Stats.GetSpecialStatAmountFactors()[SpecialStatType.MovementSpeedFactor];
                player.ApplyMovementDirectionToVector(ref velocity, wishDirection, rollSpeedFactor);
                // NOTE: Vertical velocity is not disabled to enable gravity, letting the player roll off ledges
                break;
            case DodgeType.Dash:
                // ------ Calculate the easing out ------
                // Ease into sprint speed factor if sprint button is pressed or into the regular movement speed factor (usually 1.0f)
                float easeOutTo = Input.IsActionPressed(s_MoveSprint) ? player.m_MobStats.m_SpecialStatTypeToAmountFactor[SpecialStatType.SprintSpeedFactor] : 1.0f;
                float dashProgress = 1.0f - (currentDodgeTimeSec / totalDodgeTimeSec);
                float easedDashSpeedFactor = Mathf.Lerp(dashSpeedFactor, easeOutTo, 1.0f - Mathf.Pow(1.0f - dashProgress, 2));
                // --------------------------------------

                player.ApplyMovementDirectionToVector(ref velocity, wishDirection, easedDashSpeedFactor);
                velocity.Y = 0; // Disables vertical movement (including gravity) - IDK: Maybe another dodge type that allows vertical movement
                break;
        }

        return null;
    }

    public override void OnExitState(Player player)
    {
    }
}
