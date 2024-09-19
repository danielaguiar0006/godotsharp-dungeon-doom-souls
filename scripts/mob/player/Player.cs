using Godot;
using Game.ActionTypes;
using Game.StateMachines;
using System.Net.Sockets;

public partial class Player : Mob
{
    [ExportCategory("Movement")]
    // NOTE: both moveSpeed and moveSpeedFactor affect the speed of most movement related actions
    [Export]
    public DodgeType m_DodgeType { get; set; } = DodgeType.Dash;

    [ExportCategory("Camera")]
    [Export]
    public float m_MouseSensitivity = 0.1f;
    [Export]
    public Node3D m_CameraPivot { get; private set; }
    [Export]
    public Camera3D m_Camera { get; private set; }
    [Export]
    public RayCast3D m_Raycast { get; private set; }
    [Export]
    public PlayerState m_CurrentPlayerState { get; private set; } = null;

#nullable enable
    public UdpClient? m_UdpClient;

    // Aiming/Camera input
    private float m_YawInput = 0.0f;
    private float m_PitchInput = 0.0f;
    private float m_PitchLowerLimit = 0.0f;
    private float m_PitchUpperLimit = 0.0f;

    // Useful values
    //private Vector3 m_CurrentVelocity = Vector3.Zero;
    //private float m_CurrentMovementSpeedFactor = 0.0f;

    public Player()
    {
        m_Name = "Player";
        SetMobType(MobType.Player);

        // TODO: set some sort of id to tell between clients/players
    }

    public override void _EnterTree()
    {
    }

    public override void _Ready()
    {
        if (GameManager.Instance.m_IsOnline) { m_UdpClient = new UdpClient(); }

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
        TransitionToState(new IdleState());
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        AimCamera(@event);

        // NOTE: newState is null if the state does not change, otherwise it is the new state
        PlayerState newState = m_CurrentPlayerState.HandleInput(this, @event);
        TransitionToState(newState);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        // NOTE: newState is null if the state does not change, otherwise it is the new state
        PlayerState newState = m_CurrentPlayerState.HandleKeyboardInput(this, @event);
        TransitionToState(newState);

    }

    public override void _Process(double delta)
    {
        PlayerState newState = m_CurrentPlayerState.Process(this, delta);
        TransitionToState(newState);
    }

    public override void _PhysicsProcess(double delta)
    {
        // NOTE: Instead of constantly making calls to this.Velocity cache it for better 
        // performance and work with the new velocity variable instead
        Vector3 velocity = this.Velocity;

        PlayerState newState = m_CurrentPlayerState.PhysicsProcess(this, ref velocity, delta);
        TransitionToState(newState);

        // Apply gravity and movement
        ApplyGravityToVector(ref velocity, delta);
        ApplyMobMovement(ref velocity);
    }

    // Change the player's state
    public void TransitionToState(PlayerState newState)
    {
        if (newState != null)
        {
            if (m_CurrentPlayerState != null) { m_CurrentPlayerState.OnExitState(this); } // Exit current state 
            m_CurrentPlayerState = newState;        // Set the new state
            PlayerState nextState = m_CurrentPlayerState.OnEnterState(this); // Enter the new state

            // If the OnEnterState of the new state returns another state, transition again
            if (nextState != null)
            {
                TransitionToState(nextState);
            }
        }
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
}
