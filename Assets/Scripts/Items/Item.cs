using UnityEngine;

public class Item : ScriptableObject
{
    [Header("Item Info")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite itemIcon;
    [SerializeField][TextArea] private string itemDescription;
    [SerializeField] private int itemID;

    #region ENCAPSULATION

    public string ItemName
    {
        get => itemName;
        set => itemName = value;
    }
    public Sprite ItemIcon
    {
        get => itemIcon;
        set => itemIcon = value;
    }
    public string ItemDescription    
    {
        get => itemDescription;
        set => itemDescription = value;
    }
    public int ItemID    
    {
        get => itemID;
        set => itemID = value;
    }

    #endregion
}
