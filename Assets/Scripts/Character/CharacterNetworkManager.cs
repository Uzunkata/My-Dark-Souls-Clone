using UnityEngine;
using Unity.Netcode;

public class CharacterNetworkManager : NetworkBehaviour
{
    [Header("Position")]
    private readonly NetworkVariable<Vector3> networkPosition 
        = new (
            Vector3.zero, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Owner);

    [Header("Rotation")]
    private readonly NetworkVariable<Quaternion> networkRotation 
        = new (
            Quaternion.identity, 
            NetworkVariableReadPermission.Everyone, 
            NetworkVariableWritePermission.Owner);

    public Vector3 networkPositionVelocity;
    [SerializeField]
    private float networkPositionSmoothTime = 0.1f;
    [SerializeField]
    private float networkRotationSmoothTime = 0.1f;

    [Header("Animator")]
    private readonly NetworkVariable<float> verticalMovement = 
        new(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<float> horizontalMovement = 
        new(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

    private readonly NetworkVariable<float> moveAmount = 
        new(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Owner);

    public float NetworkPositionSmoothTime => networkPositionSmoothTime;
    public float NetworkRotationSmoothTime => networkRotationSmoothTime;

    public Vector3 GetNetworkPosition() { return networkPosition.Value; }
    public void SetNetworkPosition(Vector3 networkPosition) { this.networkPosition.Value = networkPosition; }

    public Quaternion GetNetworkRotation() { return networkRotation.Value; }
    public void SetNetworkRotation(Quaternion networkRotation) { this.networkRotation.Value = networkRotation; }

    public float GetVerticalMovement() { return verticalMovement.Value; }
    public void SetVerticalMovement(float verticalMovement) { this.verticalMovement.Value = verticalMovement; }

    public float GetHorizontalMovement() { return horizontalMovement.Value; }
    public void SetHorizontalMovement(float horizontalMovement) { this.horizontalMovement.Value = horizontalMovement; }

    public float GetMoveAmount() { return moveAmount.Value; }
    public void SetMoveAmount(float moveAmount) { this.moveAmount.Value = moveAmount; }
}
