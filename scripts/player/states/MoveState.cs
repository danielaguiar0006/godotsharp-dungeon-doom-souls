using Godot;
using static InputActions;
using Game.StateMachines;


public partial class MoveState : PlayerState
{
    public override PlayerState OnEnterState(Player player)
    {
        // HACK: Perform an immediate physics update to avoid delay in state transition
        // This help alleviate an issue of the player moving at half speed when constantly
        // and immediately transitioning between states in _PhysicsProcess() (This might cause
        // other issues, but it's a trade off for now)
        // NOTE: This has been resolved by forcing player movement every physics tick inside 
        // _PhysicsProcess() in the main Player script
        //
        //player._PhysicsProcess(Engine.GetPhysicsInterpolationFraction());

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

        return null;
    }

    public override PlayerState PhysicsProcess(Player player, ref Vector3 velocity, double delta)
    {
        player.ApplyMovementInputToVector(ref velocity);

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

        if (Input.IsActionPressed(s_MoveSprint))
        {
            return new SprintState();
        }

        // TODO: Implement walking state

        return null;
    }

    public override void OnExitState(Player player)
    {
    }
}
