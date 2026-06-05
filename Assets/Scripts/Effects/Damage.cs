
using UnityEngine;
using Unity.Netcode;


[System.Serializable]
public class Damage : INetworkSerializable
{
    [Header("Physical")]
    [SerializeField] private float physicalDamage;  //TODO: define the following physical damage subtypes: "standard", "strike"/"blunt"(maybe?), "slash", "pierce"

    [Header("Elemental")]
    [SerializeField] private float magicDamage;
    [SerializeField] private float fireDamage;
    [SerializeField] private float lightningDamage;
    [SerializeField] private float holyDamage;

    [Header("Poise")]
    [SerializeField] private float poiseDamage;

    #region ENCAPSULATION
    public float PhysicalDamage
    {
        get => physicalDamage;
        set => physicalDamage = value;
    } 
    public float MagicDamage    
    {
        get => magicDamage;
        set => magicDamage = value;
    } 
    public float FireDamage    
    {
        get => fireDamage;
        set => fireDamage = value;
    } 
    public float LightningDamage    
    {
        get => lightningDamage;
        set => lightningDamage = value;
    } 
    public float HolyDamage    
    {
        get => holyDamage;
        set => holyDamage = value;
    } 
    public float PoiseDamage
    {
        get => poiseDamage;
        set => poiseDamage = value;
    }
    
    #endregion

    public void NetworkSerialize<T>(BufferSerializer<T> serializer)where T : IReaderWriter
    {
        serializer.SerializeValue(ref physicalDamage);
        serializer.SerializeValue(ref magicDamage);
        serializer.SerializeValue(ref fireDamage);
        serializer.SerializeValue(ref lightningDamage);
        serializer.SerializeValue(ref holyDamage);
        serializer.SerializeValue(ref poiseDamage);
    }

    public int CalculateFinalDamage()
    {
        return Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);
    }

    public Damage GetModifiedDamage(float modifier)
    {
        Damage result = new();

        result.physicalDamage = physicalDamage * modifier;
        result.magicDamage = magicDamage * modifier;
        result.fireDamage = fireDamage * modifier;
        result.lightningDamage = lightningDamage * modifier;
        result.holyDamage = holyDamage * modifier;
        result.poiseDamage = poiseDamage * modifier;

        return result;
    }
}