using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WorldItemDatabase : MonoBehaviour
{
    private static WorldItemDatabase instance;
    public static WorldItemDatabase GetInstance     
    {
        get { return instance; }
    }
    
    [Header("Items")]
    [SerializeField] private List<Item> items = new();

    [Header("Weapons")]
    [SerializeField] private List<WeaponItem> weapons = new();
    public WeaponItem UnarmedWeapon => weapons[0];
    
    #region ENCAPSULATION

    public List<Item> Items => items;
    public List<WeaponItem> Weapons => weapons;

    #endregion
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            OnAwake();
        } 
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnAwake()
    {
        int id = 0;

        foreach (var item in items)
            item.ItemID = id++;
        
        foreach (var weapon in weapons)
        {
            items.Add(weapon);
            weapon.ItemID = id++;
        }
    }

    public WeaponItem GetWeaponByID(int id)
    {
        return weapons.FirstOrDefault(weapon => weapon.ItemID == id);
    }
}
