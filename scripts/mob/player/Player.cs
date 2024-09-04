using Godot;
using static InputActions;
using Game.ActionTypes;
using Game.StatsManager;
using Game.StateMachines;


public partial class Player : Mob
{
    [ExportCategory("Movement")]
    // NOTE: both moveSpeed and moveSpeedFactor affect the speed of most movement related actions
    [Export]
    public DodgeType m_DodgeType { get; private set; } = DodgeType.Dash;
    [Export]
    public float m_JumpVelocity { get; private set; } = 4.0f;

    [ExportCategory("Camera")]
    [Export]
    public float m_MouseSensitivity { get; private set; } = 0.1f;
    [Export]
    public Node3D m_CameraPivot { get; private set; }
    [Export]
    public Camera3D m_Camera { get; private set; }
    [Export]
    public RayCast3D m_Raycast { get; private set; }
    [Export]
    public PlayerState m_CurrentState { get; private set; } = null;

    // Aiming/Camera input
    private float m_YawInput = 0.0f;
    private float m_PitchInput = 0.0f;
    private float m_PitchLowerLimit = 0.0f;
    private float m_PitchUpperLimit = 0.0f;

    // Useful values
    private bool m_ShowDebugInfo = false;
    //private Vector3 m_CurrentVelocity = Vector3.Zero;
    //private float m_CurrentMovementSpeedFactor = 0.0f;


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

            float movementSpeedFactor = m_MobStats.m_SpecialStatTypeToAmountFactor[SpecialStatType.MovementSpeedFactor];
            if (Input.IsPhysicalKeyPressed(Key.Up))
            {
                m_MobStats.SetSpecialStatAmountFactor(SpecialStatType.MovementSpeedFactor, movementSpeedFactor + 0.001f);
            }
            else if (Input.IsPhysicalKeyPressed(Key.Down))
            {
                m_MobStats.SetSpecialStatAmountFactor(SpecialStatType.MovementSpeedFactor, movementSpeedFactor - 0.001f);
            }
            else if (Input.IsPhysicalKeyPressed(Key.Left))
            {
                m_MobStats.SetCurrentBaseStatValue(BaseStatType.Health, m_MobStats.m_BaseStatTypeToCurrentValue[BaseStatType.Health] - 1);
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
        ApplyMobMovement(ref velocity);
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
        foreach (var stat in m_MobStats.m_BaseStatTypeToCurrentValue)
        {
            statsInfo += stat.Key.ToString() + ": " + stat.Value + "\n";
        }
        foreach (var stat in m_MobStats.m_AttributeTypeToCurrentLevel)
        {
            statsInfo += stat.Key.ToString() + ": " + stat.Value + "\n";
        }
        foreach (var stat in m_MobStats.m_SpecialStatTypeToAmountFactor)
        {
            statsInfo += stat.Key.ToString() + ": " + stat.Value + "\n";
        }
        this.GetNode<Label>("StatsDebugInfo").Text = statsInfo;
    }
}
