using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(CharacterManager))]
public class CharacterCombatManager : NetworkBehaviour
{
    private CharacterManager character;

    [Header("Attack Type")]
    [SerializeField] private WeaponItemAction.AttackType currentAttackType;

    [Header("Attack Target")]
    [SerializeField] private CharacterManager currentLockedOnTarget;

    [Header("Lock On Transform")]
    [SerializeField] private Transform lockOnTransform;

    #region ENCAPSULATION

    public WeaponItemAction.AttackType CurrentAttackType
    {
        get => currentAttackType;
        set => currentAttackType = value;
    }

    public CharacterManager CurrentLockedOnTarget
    {
        get => currentLockedOnTarget;
        set => currentLockedOnTarget = value;
    }

    public Transform LockOnTransform => lockOnTransform;

    #endregion

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void SetTarget(CharacterManager newTarget)
    {
        if (!character.IsOwner)
            return;

        if (newTarget != null)
        {
            currentLockedOnTarget = newTarget;
            character.CharacterNetworkManager.CurrentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
        }
        else
        {
            currentLockedOnTarget = null;
        }
    }
}
