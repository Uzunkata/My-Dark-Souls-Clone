using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        base.Start();

        // TODO: WHEN WE MAKE THE CHARACTER CREATION MENUE, AND SET THE STATS BASED ON THE CLASS
        // IT WILL BE CALCULATED HERE.
        CalculateHealthBasedOnVitalityLevel(player.Vitality.Value);
        CalculateStaminaBasedOnEnduranceLevel(player.Endurance.Value);
    }
}
