using UnityEngine;

public class Utility_DestroyAfterTime : MonoBehaviour
{
    [SerializeField] private float timeUntilDestruction = 5;

    private void Awake()
    {
        Destroy(gameObject, timeUntilDestruction);
    }
}
