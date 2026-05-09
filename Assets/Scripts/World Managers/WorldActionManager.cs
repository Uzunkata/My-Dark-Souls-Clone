using System.Linq;
using UnityEngine;

public class WorldActionManager : MonoBehaviour
{

    private static WorldActionManager instance;
    public static WorldActionManager GetInstance
    {
        get { return instance; }
    }

    [Header("Weapon Item Actions")]
    [SerializeField] private WeaponItemAction[] weaponItemActions;

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

        DontDestroyOnLoad(gameObject);
    }

    private void OnAwake()
    {
        
    }

    private void Start()
    {
        for (int i = 0; i < weaponItemActions.Length; i++)
        {
            weaponItemActions[i].ActionID = i;
        }
    }

    public WeaponItemAction GetWeaponItemActionByID(int id)
    {
        return weaponItemActions.FirstOrDefault(action => action.ActionID == id);
    }
}
