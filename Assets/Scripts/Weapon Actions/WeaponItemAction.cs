using UnityEngine;

[CreateAssetMenu(menuName = "Character Actions/Weapon Action/Test Action")]
public class WeaponItemAction : ScriptableObject
{
    public enum AttackType
    {
        LightAttack01,
    }
    [SerializeField] private int actionID;

    #region ENCAPSULATION

    public int ActionID
    {
        get => actionID;
        set => actionID = value;
    }

    #endregion

    // public virtual void AttemptToPerformAction(PlayerManager performingPlayer, WeaponItem weapon)
    // {
    //     if (performingPlayer.IsOwner)
    //     {
    //         performingPlayer.PlayerNetworkManager.WeaponInUsedID.Value = weapon.ItemID;
    //     }
    // }
}
