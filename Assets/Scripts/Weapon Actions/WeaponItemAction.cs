using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Action/Test Action")]
public class WeaponItemAction : ScriptableObject
{
    public enum AttackType
    {
        LightAttack_OneHand_01,
        LightAttack_OneHand_02,
        HeavyAttack_OneHanded_01,
        HeavyAttack_OneHanded_02,
        ChargedHeavyAttack_OneHanded_01,
        ChargedHeavyAttack_OneHanded_02,

    }
    
    [SerializeField] private int actionID;

    #region ENCAPSULATION

    public int ActionID
    {
        get => actionID;
        set => actionID = value;
    }

    #endregion

    public virtual void AttemptToPerformAction(PlayerManager performingPlayer, WeaponItem weapon)
    {
        if (performingPlayer.IsOwner)
        {
            performingPlayer.PlayerNetworkManager.WeaponInUseID.Value = weapon.ItemID;
        }
    }
}
