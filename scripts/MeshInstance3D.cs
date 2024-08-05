using Godot;

public partial class MeshInstance3D : Godot.MeshInstance3D
{
    [Export]
    protected Node3D m_TargetMeshInstanceNode;
    private Transform3D m_TargetMeshInstanceTransform;
    private float lerpSpeed = 100.0f;


    // TODO: This is basically the same as Camera.cs, so we can maybe make a function for this - ("this" being the interpolation of movement and rotation)
    public override void _Process(double delta)
    {
        // Get the target mesh instance position
        m_TargetMeshInstanceTransform = m_TargetMeshInstanceNode.GlobalTransform;

        // Interpolate position
        Vector3 newPosition = this.GlobalTransform.Origin.Lerp(m_TargetMeshInstanceTransform.Origin, (float)delta * lerpSpeed);

        // Interpolate rotation
        Quaternion currentRotation = this.GlobalTransform.Basis.GetRotationQuaternion();
        Quaternion targetRotation = m_TargetMeshInstanceTransform.Basis.GetRotationQuaternion();
        Quaternion newRotation = currentRotation.Slerp(targetRotation, (float)delta * lerpSpeed);

        // Convert quaternion to basis
        Basis newBasis = new Basis(newRotation);

        this.GlobalTransform = new Transform3D(newBasis, newPosition);
    }
}
