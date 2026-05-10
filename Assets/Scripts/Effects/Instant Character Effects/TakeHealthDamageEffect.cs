using UnityEngine;
using UnityEngine.TextCore.Text;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Health Damage")]
public class TakeHealthDamageEffect : InstantCharacterEffect
{

    [Header("Character Causing Damage")]
    [SerializeField] private CharacterManager characterCausingDamage;

    [Header("Damage")]
    [SerializeField] private Damage damage;
    private int finalDamage = 0;  // final calculation of the damage

    [Header("Poise")]
    // [SerializeField] private float poiseDamage = 0;
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
    public float AngleHitFrom
    {
        get => angleHitFrom;
        set  => angleHitFrom = value;
    }
    public CharacterManager CharacterCausingDamage
    {
        get => characterCausingDamage;
        set => characterCausingDamage = value;
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
        PlayDirectionalBasedDamageAnimation(character);
        // CHECK FOR STATUS BUILD UPS
        PlayDamageVFX(character);
        PlayDamageSFX(character);

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

        Debug.Log("I have taken: " + finalDamage);

        // TODO: POISE DAMAGE
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        // TODO:
        // FIRE, LIGHTING, HOLY PARTICLE EFFECTS

        character.CharacterEffectsManager.PlayBloodSplatterVFX(contactPoint);
    }

    private void PlayDamageSFX(CharacterManager character)
    {
        // TODO:
        // FIRE, LIGHTING, HOLY PARTICLE EFFECTS

        AudioClip physicalDamageSFX = WorldSoundFXManager.GetInstance.ChooseRandomSFXFromArray(WorldSoundFXManager.GetInstance.PhysicalDamageSFX);

        character.CharacterSoundFXManager.PlaySoundFX(physicalDamageSFX);
    }

    private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
    {
        if (!character.IsOwner)
            return;

        // TODO:
        // CALCULATE IF POSIE IS BROKEN
        poiseIsBroken = true;

        damageAnimation = character.CharacterAnimatorManager.GetDirectionalDamageAnimation(angleHitFrom);

        if (poiseIsBroken)
        {
            character.CharacterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
        }
    }
}
