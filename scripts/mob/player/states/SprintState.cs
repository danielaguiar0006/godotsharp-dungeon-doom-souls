using Godot;
using static InputActions;
using Game.StatsManager;
using Game.StateMachines;


public partial class SprintState : PlayerState
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
        if (Input.IsActionJustPressed(s_MoveJump) && player.IsOnFloor())
        {
            return new JumpState();
        }

        if (Input.IsActionJustPressed(s_MoveDodge))
        {
            return new DodgeState();
        }

        return null;
    }

    public override PlayerState HandleKeyboardInput(Player player, InputEvent @event)
    {
        return null;
    }

    public override PlayerState Process(Player player, double delta)
    {
        if (Input.IsActionJustReleased(s_MoveSprint))
        {
            return new MoveState();
        }

        return null;
    }

    public override PlayerState PhysicsProcess(Player player, ref Vector3 velocity, double delta)
    {
        player.ApplyMovementInputToVector(ref velocity, player.m_MobStats.m_SpecialStatTypeToAmountFactor[SpecialStatType.SprintSpeedFactor]);

        // Transition to the idle state if the player is not moving
        if (velocity.Length() == 0)
        {
            return new IdleState();
        }

        // Transition to the fall state if the player falling
        if (!player.IsOnFloor() && velocity.Y < 0.0f)
        {
            return new FallState();
        }

        return null;
    }

    public override void OnExitState(Player player)
    {
    }
}
