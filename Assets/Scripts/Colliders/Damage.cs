
using UnityEngine;

[System.Serializable]
public class Damage
{
    [Header("Physical")]
    [SerializeField] private float physicalDamage;  //TODO: define the following physical damage subtypes: "standard", "strike"/"blunt"(maybe?), "slash", "pierce"

    [Header("Elemental")]
    [SerializeField] private float magicDamage;
    [SerializeField] private float fireDamage;
    [SerializeField] private float lightningDamage;
    [SerializeField] private float holyDamage;

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

    public int CalculateFinalDamage()
    {
        return Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);
    }
}