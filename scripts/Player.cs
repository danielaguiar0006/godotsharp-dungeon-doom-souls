using Godot;
//using System;

public partial class Player : CharacterBody3D
{
    [ExportCategory("Movement")]
    [Export]
    protected float m_Speed = 5.0f;
    [Export]
    protected float m_JumpVelocity = 4.5f;

    [ExportCategory("Camera")]
    [Export]
    protected float m_MouseSensitivity = 0.1f;
    [Export]
    protected Node3D m_CameraPivot;
    [Export]
    protected Camera3D m_Camera;
    [Export]
    protected RayCast3D m_Raycast;

    private float m_YawInput = 0.0f;
    private float m_PitchInput = 0.0f;
    private float m_PitchLowerLimit = 0.0f;
    private float m_PitchUpperLimit = 0.0f;

    // For Garbage Collection Optimization - Godot uses custom strings for actions which causes potential GC problems
    private static StringName s_MoveForward = new StringName("move_forward");
    private static StringName s_MoveBackward = new StringName("move_backward");
    private static StringName s_MoveLeft = new StringName("move_left");
    private static StringName s_MoveRight = new StringName("move_right");
    private static StringName s_MoveJump = new StringName("move_jump");

    // Get the gravity from the project settings to be synced with RigidBody nodes
    public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

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

        m_Camera.Current = true;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        // Checking mouse button events
        if (@event is InputEventMouseButton mouseButtonEvent && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            if (mouseButtonEvent.IsActionPressed("shoot"))
            {
                Shoot();
            }
        }

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
        this.RotateY(Mathf.DegToRad(m_YawInput * m_MouseSensitivity));
        // Rotate camera pivot and clamp the pitch
        m_CameraPivot.RotateX(Mathf.DegToRad(m_PitchInput * m_MouseSensitivity));

        // HACK: This does not work for some reason:
        // m_CameraPivot.Rotation.X = Mathf.Clamp(m_CameraPivot.Rotation.X, m_PitchLowerLimit, m_PitchUpperLimit);

        Vector3 cameraRotation = m_CameraPivot.Rotation;
        cameraRotation.X = Mathf.Clamp(cameraRotation.X, m_PitchLowerLimit, m_PitchUpperLimit);
        m_CameraPivot.Rotation = cameraRotation;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector3 velocity = this.Velocity;

        // Add the gravity
        if (!IsOnFloor())
            velocity.Y -= gravity * (float)delta;

        // Handle Jump
        if (Input.IsActionJustPressed(s_MoveJump) && IsOnFloor())
            velocity.Y = m_JumpVelocity;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions
        Vector2 inputDir = Input.GetVector(s_MoveLeft, s_MoveRight, s_MoveForward, s_MoveBackward);
        Vector3 direction = (this.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
        if (direction != Vector3.Zero)
        {
            velocity.X = direction.X * m_Speed;
            velocity.Z = direction.Z * m_Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, m_Speed);
            velocity.Z = Mathf.MoveToward(Velocity.Z, 0, m_Speed);
        }

        this.Velocity = velocity;
        MoveAndSlide();
    }

    public void Shoot()
    {
        // Play shooting animation, sound, etc...

        if (m_Raycast.IsColliding())
        {
        }
    }
}
