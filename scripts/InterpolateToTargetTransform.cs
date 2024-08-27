using Godot;

// This script moves the attached node to the target node position and rotation smoothly.
// Originally created because objects tied to parents who update their position in physics_process()
// cause severe jittering.
public partial class InterpolateToTargetTransform : Node3D
{
    [Export]
    protected Node3D m_TargetNode;
    [Export]
    // NOTE: Values matching the same as the Camera script will make the camera and the object move at the same speed (They will be in sync and move smoothly)
    protected float m_LerpSpeed = 75.0f; // Adjust this value to your liking - 
    [Export]
    protected bool m_FlipXAxis = false;
    [Export]
    protected bool m_FlipYAxis = false;
    [Export]
    protected bool m_FlipZAxis = false;


    public override void _Process(double delta)
    {
        // Get the target Node3D position
        Transform3D targetNodeTransform = m_TargetNode.GlobalTransform;

        // Flip the axis if needed
        if (m_FlipXAxis) { targetNodeTransform.Basis.X = -targetNodeTransform.Basis.X; }
        if (m_FlipYAxis) { targetNodeTransform.Basis.Y = -targetNodeTransform.Basis.Y; }
        if (m_FlipZAxis) { targetNodeTransform.Basis.Z = -targetNodeTransform.Basis.Z; }

        // Interpolate position
        Vector3 newPosition = this.GlobalTransform.Origin.Lerp(targetNodeTransform.Origin, (float)delta * m_LerpSpeed);

        // Interpolate rotation
        Quaternion currentRotation = this.GlobalTransform.Basis.GetRotationQuaternion();
        Quaternion targetRotation = targetNodeTransform.Basis.GetRotationQuaternion();
        Quaternion newRotation = currentRotation.Slerp(targetRotation, (float)delta * m_LerpSpeed);


        // Convert quaternion to basis
        Basis newBasis = new Basis(newRotation);

        // Apply the new transform
        this.GlobalTransform = new Transform3D(newBasis, newPosition);
    }
}
