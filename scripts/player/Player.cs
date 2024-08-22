using Godot;
using ActionTypes;

// Note: The reason for all the public variables is so that player data can be easily
// read and modified in the invividual states (i.e. IdleState, ShootingState, etc...).
public partial class Player : CharacterBody3D
{
    [ExportCategory("Movement")]
    [Export]
    public float m_MovementSpeed = 5.0f;
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

    private PlayerState m_CurrentState = null;

    public Vector3 m_MovementDirection = Vector3.Zero;

    public DodgeType m_DodgeType = DodgeType.Dash;
    public float m_DodgeSpeedFactor = 1.0f;


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

        // Set the players initial state here
        m_CurrentState = new IdleState();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        AimCamera(@event);

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
        // DEBUG: Information about the current state on the screen
        this.GetNode<Label>("CurrentStateDebugInfo").Text = m_CurrentState.GetType().Name;

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

    // Helper funciton to aim the camera/ get mouse input
    private void AimCamera(InputEvent @event)
    {
        // Checking if the mouse is active
        if (@event is InputEventMouseMotion mouseInputEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            m_YawInput = -mouseInputEvent.Relative.X;
            m_PitchInput = -mouseInputEvent.Relative.Y;
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
}
