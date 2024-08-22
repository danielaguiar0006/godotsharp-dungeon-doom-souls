using Godot;
using static InputActions;

public class MoveState : PlayerState
{
    public override PlayerState OnEnterState(Player player)
    {
        // HACK: Perform an immediate physics update to avoid delay in state transition
        // This help alleviate an issue of the player moving at half speed when constantly
        // and immediatly transitioning between states in PhysicsProcess (This might cause
        // other issues, but it's a tradeoff for now)
        player._PhysicsProcess(Engine.GetPhysicsInterpolationFraction());

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
        player.m_MovementDirection = (player.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (player.m_MovementDirection != Vector3.Zero)
        {
            velocity.X = player.m_MovementDirection.X * player.m_MovementSpeed;
            velocity.Z = player.m_MovementDirection.Z * player.m_MovementSpeed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(player.Velocity.X, 0, player.m_MovementSpeed);
            velocity.Z = Mathf.MoveToward(player.Velocity.Z, 0, player.m_MovementSpeed);
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

        // NOTE: needs this transition to the dodge state needs to be after player.MoveAndSlide()
        // To avoid the player's movemnet being frozen while holding the dodge button
        if (Input.IsActionJustPressed(s_MoveDodge))
        {
            return new DodgeState();
        }

        return null;
    }
}

