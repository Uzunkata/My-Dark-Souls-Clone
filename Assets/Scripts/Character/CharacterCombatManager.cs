using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    [SerializeField] private WeaponItemAction.AttackType currentAttackType;

    #region ENCAPSULATION

    public WeaponItemAction.AttackType CurrentAttackType
    {
        get => currentAttackType;
        set => currentAttackType = value;
    }

    #endregion
    protected virtual void Awake()
    {
        
    }
}
