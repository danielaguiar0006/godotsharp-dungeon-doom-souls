using Godot;
using static InputActions;
using ActionTypes;

// NOTE: The reason for all the public variables is so that player data can be easily
// read and modified in the individual states (i.e. IdleState, MoveState, etc...).
public partial class Player : CharacterBody3D
{
    [ExportCategory("Movement")]
    // NOTE: both moveSpeed and moveSpeedFactor affect the speed of most movement related actions
    [Export]
    public float m_MovementSpeed = 5.0f;
    [Export]
    public float m_MovementSpeedFactor = 1.0f;

    [Export]
    public float m_SprintSpeedFactor = 1.75f;
    [Export]
    public DodgeType m_DodgeType = DodgeType.Dash;
    [Export]
    public float m_DodgeSpeedFactor = 1.0f;
    [Export]
    public float m_JumpVelocity = 4.0f;

    [ExportCategory("Camera")]
    [Export]
    public float m_MouseSensitivity = 0.1f;
    [Export]
    public Node3D m_CameraPivot;
    [Export]
    public Camera3D m_Camera;
    [Export]
    public RayCast3D m_Raycast;

    // Aiming/Camera input
    public float m_YawInput = 0.0f;
    public float m_PitchInput = 0.0f;
    public float m_PitchLowerLimit = 0.0f;
    public float m_PitchUpperLimit = 0.0f;

    // Get the gravity from the project settings to be synced with RigidBody nodes
    public float m_Gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    // Where the player is moving towards
    public Vector3 m_MovementDirection = Vector3.Zero;

    private PlayerState m_CurrentState = null;
    // Useful values
    private Vector3 m_CurrentVelocity = Vector3.Zero;
    private float m_CurrentMovementSeedFactor = 0.0f;


    public override void _EnterTree()
    {
    }

    public override void _Ready()
    {
        // For more accurate mouse input
        Input.UseAccumulatedInput = false;

        // Locks the mouse cursor to the window
        Input.MouseMode = Input.MouseModeEnum.Captured;

        // Set Limits for the camera pitch (up and down rotation)
        m_PitchLowerLimit = Mathf.DegToRad(-89);
        m_PitchUpperLimit = Mathf.DegToRad(89);

        // Make sure the view-port camera is set to the current camera
        m_Camera.Current = true;

        // Set the players initial state here
        m_CurrentState = new IdleState();
        m_CurrentState.OnEnterState(this);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        AimCamera(@event);

        // NOTE: newState is null if the state does not change, otherwise it is the new state
        PlayerState newState = m_CurrentState.HandleInput(this, @event);
        if (newState != null)
        {
            m_CurrentState.OnExitState(this);
            m_CurrentState = newState;
            m_CurrentState.OnEnterState(this);
        }
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        // NOTE: newState is null if the state does not change, otherwise it is the new state
        PlayerState newState = m_CurrentState.HandleKeyboardInput(this, @event);
        if (newState != null)
        {
            m_CurrentState.OnExitState(this);
            m_CurrentState = newState;
            m_CurrentState.OnEnterState(this);
        }
    }

    public override void _Process(double delta)
    {
        // DEBUG: Information about the current state on the screen
        this.GetNode<Label>("CurrentStateDebugInfo").Text = "Current State: " + m_CurrentState.GetType().Name;
        // DEBUG: Show if the player is on the floor currently
        this.GetNode<Label>("IsOnFloorDebugInfo").Text = "On Floor: " + this.IsOnFloor().ToString();

        PlayerState newState = m_CurrentState.Process(this, delta);
        if (newState != null)
        {
            m_CurrentState.OnExitState(this);
            m_CurrentState = newState;
            m_CurrentState.OnEnterState(this);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // NOTE: Instead of constantly making calls to this.Velocity cache it for better 
        // performance and work with the new velocity variable instead
        Vector3 velocity = this.Velocity;

        PlayerState newState = m_CurrentState.PhysicsProcess(this, ref velocity, delta);
        if (newState != null)
        {
            m_CurrentState.OnExitState(this);
            m_CurrentState = newState;
            m_CurrentState.OnEnterState(this);
        }

        // Apply gravity and movement
        ApplyGravityToVector(ref velocity, delta);
        ApplyPlayerMovement(ref velocity);
    }

    // Helper funciton to aim the camera through mouse/controller input
    private void AimCamera(InputEvent @event)
    {
        // Checking if the mouse is active
        if (@event is InputEventMouseMotion mouseInputEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            m_YawInput = -mouseInputEvent.ScreenRelative.X;
            m_PitchInput = -mouseInputEvent.ScreenRelative.Y;
        }
        else
        { // Reset the mouse input if the mouse is not active
          // InputEventMouseMotion.Relative is not reset to 0 when the mouse is not active
            m_YawInput = 0.0f;
            m_PitchInput = 0.0f;
        }

        // Rotate player
        RotateY(Mathf.DegToRad(m_YawInput * m_MouseSensitivity));
        // Rotate camera pivot and clamp the pitch
        m_CameraPivot.RotateX(Mathf.DegToRad(m_PitchInput * m_MouseSensitivity));

        // HACK: This does not work for some reason:
        // m_CameraPivot.Rotation.X = Mathf.Clamp(m_CameraPivot.Rotation.X, m_PitchLowerLimit, m_PitchUpperLimit);

        Vector3 cameraRotation = m_CameraPivot.Rotation;
        cameraRotation.X = Mathf.Clamp(cameraRotation.X, m_PitchLowerLimit, m_PitchUpperLimit);
        m_CameraPivot.Rotation = cameraRotation;
    }


    // moveSpeedFactor can be used for slower or faster movement (walking, running, etc...)
    public void ApplyPlayerMovement(ref Vector3 velocity)
    {
        // Move the player
        this.Velocity = velocity;
        this.MoveAndSlide();
    }

    // Helper function to apply gravity to a vector in different states
    // This does not apply gravity directly to the player's velocity, but instead to a target vector
    public void ApplyGravityToVector(ref Vector3 velocity, double delta)
    {
        // Add the gravity
        if (!this.IsOnFloor())
            velocity.Y -= this.m_Gravity * (float)delta;
    }

    // This does not apply input movement directly to the player's velocity, but instead to a target vector
    public void ApplyMovementInputToVector(ref Vector3 velocity, float movementSpeedFactor = 1.0f)
    {
        // Get the input direction and handle the movement/deceleration.
        Vector2 inputDir = Input.GetVector(s_MoveLeft, s_MoveRight, s_MoveForward, s_MoveBackward);
        this.m_MovementDirection = (this.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (this.m_MovementDirection != Vector3.Zero)
        {
            velocity.X = this.m_MovementDirection.X * this.m_MovementSpeed * movementSpeedFactor;
            velocity.Z = this.m_MovementDirection.Z * this.m_MovementSpeed * movementSpeedFactor;
        }
        else  // If the player is not moving, decelerate
        {
            velocity.X = Mathf.MoveToward(velocity.X, 0, this.m_MovementSpeed * movementSpeedFactor);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0, this.m_MovementSpeed * movementSpeedFactor);
        }

        m_CurrentMovementSeedFactor = movementSpeedFactor;
        m_CurrentVelocity = velocity;
    }

    // This does not apply movement direction directly to the player's velocity, but instead to a target vector
    public void ApplyMovementDirectionToVector(ref Vector3 velocity, Vector3 wishDirection, float movementSpeedFactor = 1.0f)
    {
        if (wishDirection != Vector3.Zero)
        {
            velocity.X = wishDirection.X * this.m_MovementSpeed * movementSpeedFactor;
            velocity.Z = wishDirection.Z * this.m_MovementSpeed * movementSpeedFactor;
        }
        else
        {
            velocity.X = Mathf.MoveToward(this.Velocity.X, 0, this.m_MovementSpeed * movementSpeedFactor);
            velocity.Z = Mathf.MoveToward(this.Velocity.Z, 0, this.m_MovementSpeed * movementSpeedFactor);
        }

        m_CurrentMovementSeedFactor = movementSpeedFactor;
        m_CurrentVelocity = velocity;
    }
}
