using Godot;
using static InputActions;
using Game.ActionTypes;
using Game.StatsAndAttributes;

// NOTE: The reason for all the public variables is so that player data can be easily
// read and modified in the individual states (i.e. IdleState, MoveState, etc...).
public partial class Player : Mob
{
    [ExportCategory("Movement")]
    // NOTE: both moveSpeed and moveSpeedFactor affect the speed of most movement related actions
    [Export]
    public float m_MovementSpeed = 5.0f;
    [Export]
    public DodgeType m_DodgeType = DodgeType.Dash;
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
    private float m_CurrentMovementSpeedFactor = 0.0f;

    // TODO: Turn this into an inventory class which handles storing items, equiping, and unequiping items:
    public Item[] m_EquipedItems;

    private bool m_ShowDebugInfo = false;

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

        if (Input.IsActionPressed(s_ToggleDebugInfo))
        {
            m_ShowDebugInfo = !m_ShowDebugInfo;
        }
    }

    public override void _Process(double delta)
    {
        // TODO: MAKE THIS NICER/CLEANUP
        // DEBUG: Show debug info
        if (m_ShowDebugInfo)
        {
            ShowDebugInfo();

            float movementSpeedFactor = m_Stats.GetSpecialStatAmountFactors()[SpecialStatType.MovementSpeedFactor];
            if (Input.IsPhysicalKeyPressed(Key.Up))
            {
                m_Stats.SetSpecialStatAmountFactor(SpecialStatType.MovementSpeedFactor, movementSpeedFactor + 0.001f);
            }
            else if (Input.IsPhysicalKeyPressed(Key.Down))
            {
                m_Stats.SetSpecialStatAmountFactor(SpecialStatType.MovementSpeedFactor, movementSpeedFactor - 0.001f);
            }
        }
        else
        {
            this.GetNode<Label>("CurrentStateDebugInfo").Text = "";
            this.GetNode<Label>("IsOnFloorDebugInfo").Text = "";
            this.GetNode<Label>("StatsDebugInfo").Text = "";
        }

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
    public void ApplyMovementInputToVector(ref Vector3 velocity, float movementSpeedFactor = 1.0f, bool applyStatMovementSpeedFactor = true)
    {
        if (applyStatMovementSpeedFactor)
        {
            movementSpeedFactor *= base.m_Stats.GetSpecialStatAmountFactors()[SpecialStatType.MovementSpeedFactor];
        }

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

        m_CurrentMovementSpeedFactor = movementSpeedFactor;
        m_CurrentVelocity = velocity;
    }

    // This does not apply movement direction directly to the player's velocity, but instead to a target vector
    // This also does not apply the player's stat: MovementSpeedFactor on top of the local movementSpeedFactor,
    // if you do want to apply the player's MovementSpeedFactor, manually apply it or use ApplyMovementInputToVector instead.
    public void ApplyMovementDirectionToVector(ref Vector3 velocity, Vector3 wishDirection, float movementSpeedFactor = 1.0f, bool applyStatMovementSpeedFactor = true)
    {
        if (applyStatMovementSpeedFactor)
        {
            movementSpeedFactor *= base.m_Stats.GetSpecialStatAmountFactors()[SpecialStatType.MovementSpeedFactor];
        }

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

        m_CurrentMovementSpeedFactor = movementSpeedFactor;
        m_CurrentVelocity = velocity;
    }

    private void ShowDebugInfo()
    {
        // DEBUG: Show the player's current velocity
        // TODO: this.GetNode<Label>("VelocityDebugInfo").Text = "Velocity: " + m_CurrentVelocity;

        // DEBUG: Show the player's current movement speed factor
        // TODO: this.GetNode<Label>("MovementSpeedFactorDebugInfo").Text = "Movement Speed Factor: " + m_CurrentMovementSpeedFactor;

        // DEBUG: Information about the current state on the screen
        this.GetNode<Label>("CurrentStateDebugInfo").Text = "Current State: " + m_CurrentState.GetType().Name;
        // DEBUG: Show if the player is on the floor currently
        var floorDebugLabel = this.GetNode<Label>("IsOnFloorDebugInfo");
        if (this.IsOnFloor())
        {
            floorDebugLabel.AddThemeColorOverride("font_color", new Color(0, 1, 0));
            floorDebugLabel.Text = "On Floor: True";
        }
        else
        {
            floorDebugLabel.AddThemeColorOverride("font_color", new Color(1, 0, 0));
            floorDebugLabel.Text = "On Floor: False";
        }

        // Debug: Show the Player's Stats info
        string statsInfo = "PLAYER STATS:\n";
        foreach (var stat in m_Stats.GetCurrentBaseStatValues())
        {
            statsInfo += stat.Key.ToString() + ": " + stat.Value + "\n";
        }
        foreach (var stat in m_Stats.GetCurrentAttributeLevels())
        {
            statsInfo += stat.Key.ToString() + ": " + stat.Value + "\n";
        }
        foreach (var stat in m_Stats.GetSpecialStatAmountFactors())
        {
            statsInfo += stat.Key.ToString() + ": " + stat.Value + "\n";
        }
        this.GetNode<Label>("StatsDebugInfo").Text = statsInfo;
    }
}
