using Godot;

// Note: The reason for all the public variables is so that player data can be easily
// read and modified in the invividual states (i.e. IdleState, ShootingState, etc...).
public partial class Player : CharacterBody3D
{
    [ExportCategory("Movement")]
    [Export]
    public float m_Speed = 5.0f;
    [Export]
    public float m_JumpVelocity = 4.5f;

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

    PlayerState m_CurrentState = null;

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
        m_PitchLowerLimit = Mathf.DegToRad(-90);
        m_PitchUpperLimit = Mathf.DegToRad(90);

        // Make sure the view-port camera is set to the current camera
        m_Camera.Current = true;

        m_CurrentState = new IdleState();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // NOTE: newState is null if the state does not change, otherwise it is the new state
        PlayerState newState = m_CurrentState.HandleInput(this, @event);
        if (newState != null)
        {
            m_CurrentState = newState;
            m_CurrentState.OnEnterState(this);
        }
    }

    public override void _Process(double delta)
    {
        // NOTE: newState is null if the state does not change, otherwise it is the new state
        PlayerState newState = m_CurrentState.Process(this, delta);
        if (newState != null)
        {
            m_CurrentState = newState;
            m_CurrentState.OnEnterState(this);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // NOTE: newState is null if the state does not change, otherwise it is the new state
        PlayerState newState = m_CurrentState.PhysicsProcess(this, delta);
        if (newState != null)
        {
            m_CurrentState = newState;
        }
    }
}
