using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Health Damage")]
public class TakeHealthDamageEffect : InstantCharacterEffect
{

    [Header("Character Causing Damage")]
    [SerializeField] private CharacterManager characterCausingDamage;

    [Header("Damage")]
    [SerializeField] private Damage damage;
    private int finalDamage = 0;  // final calculation of the damage

    [Header("Poise")]
    [SerializeField] private float poiseDamage = 0;
    [SerializeField] private bool poiseIsBroken = false;

    //TODO: BUILD UPS (poison/bleed/...)

    [Header("Animation")]
    [SerializeField] private bool playDamageAniomation = true;
    [SerializeField] private bool manuallySelectDamageAnimation = false;
    [SerializeField] private string damageAnimation;

    [Header("sound FX")]
    [SerializeField] private bool willPlayDamageSFX = true;
    [SerializeField] private AudioClip elementalDamageSoundFX;

    [Header("Direction Damage Taken From")]
    [SerializeField] private float angleHitFrom;
    [SerializeField] private Vector3 contactPoint;

    #region ENCAPSULATION

    public Damage Damage
    {
        get => damage;
        set => damage = value;
    }

    public Vector3 ContactPoint
    {
        get => contactPoint;
        set => contactPoint = value;
    }

    #endregion

    public override void ProcessEffect(CharacterManager character)
    {
        base.ProcessEffect(character);

        if (character.IsDead.Value)
            return;

        // TODO: 
        // CHECK FOR INVULNERABILITY
        CalculateDamage(character);
        // CHECK WHICH DIRECTION DAMAGE CAME FROM
        // PLAY DAMAGE ANIMATION
        // CHECK FOR STATUS BUILD UPS
        // PLAY SOUND FX
        // PLAY VFX

        // IF CHARACTER IS AI, CHECK FOR NEW TARGET IF CHARACTER CAUSING DAMAGE IS PRESENT
    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner)
            return;

        if (characterCausingDamage != null)
        {
            // TODO: CHECK FOR MODIFIERS
        }

        // TODO:
        // CHECK FOR FLAT DEFENSES
        // CHECK FOR ARMOR ABSORPTION

        finalDamage = damage.CalculateFinalDamage();

        if (finalDamage <= 0)
        {
            finalDamage = 1;
        }

        character.CurrentHealth.Value -= finalDamage;

        // TODO: POISE DAMAGE
    }

}
