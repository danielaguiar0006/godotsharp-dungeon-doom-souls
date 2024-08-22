using Godot;

// This script moves the camera to the target position and rotation smoothly.
// Originally the camera was a child of the player, but it was causing some jittering when the player was moving. (Related to updating in _PhysicsProcess)
// So I decided to make the camera a separate node and move it smoothly to the target position.
public partial class Camera : Camera3D
{
    // TODO: Add Getters and Setters for the target position and lerp speed to extend the functionality of the main camera.

    [Export]
    protected float lerpSpeed = 75.0f; // Adjust this value to your liking
    [Export]
    public Node3D m_TargetCameraTransform;
    private Transform3D m_TargetCameraPosition;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        // Get the target camera position
        m_TargetCameraPosition = m_TargetCameraTransform.GlobalTransform;

        // Interpolate position
        Vector3 newPosition = this.GlobalTransform.Origin.Lerp(m_TargetCameraPosition.Origin, (float)delta * lerpSpeed);

        // Interpolate rotation
        Quaternion currentRotation = this.GlobalTransform.Basis.GetRotationQuaternion();
        Quaternion targetRotation = m_TargetCameraPosition.Basis.GetRotationQuaternion();
        Quaternion newRotation = currentRotation.Slerp(targetRotation, (float)delta * lerpSpeed);

        // Convert quaternion to basis
        Basis newBasis = new Basis(newRotation);

        this.GlobalTransform = new Transform3D(newBasis, newPosition);
    }
}
