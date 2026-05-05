using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerUIPopUpManager : MonoBehaviour
{
    [Header("YOU DIED Pop Up")]
    [SerializeField] GameObject youDiedPopUpObject;
    [SerializeField] TextMeshProUGUI youDiedPopUpText;
    [SerializeField] TextMeshProUGUI youDiedPopUpBackroundText;
    // Allows us to set the alpha to fade over time for every canvas object in this group
    [SerializeField] CanvasGroup youDiedPopUpCanvasGroup;

    public void SendYouDiedPopUp()
    {
        youDiedPopUpObject.SetActive(true);
        youDiedPopUpBackroundText.characterSpacing = 0;
        StartCoroutine(StrechPopUpTextOverTime(youDiedPopUpBackroundText, 8, 8.32f));
        StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5));
        StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup, 2, 5));

    }

    private IEnumerator StrechPopUpTextOverTime(TextMeshProUGUI text, float duration, float strechAmount)
    {
        if (duration > 0f)
        {
            text.characterSpacing = 0;
            float timer = 0;
            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                text.characterSpacing = Mathf.Lerp(text.characterSpacing, strechAmount, duration * (Time.deltaTime / 20));
                yield return null;
            }
        }
    }

    private IEnumerator FadeInPopUpOverTime(CanvasGroup canvasGroup, float duration)
    {
        if (duration > 0f)
        {
            canvasGroup.alpha = 0;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1, duration * Time.deltaTime);
                yield return null;
            }
        }

        canvasGroup.alpha = 1;
        yield return null;
    }

    private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvasGroup, float duration, float delay)
    {
        if (duration > 0f)
        {
            while (delay > 0)
            {
                delay -= Time.deltaTime;
                yield return null;
            }

            canvasGroup.alpha = 1;
            float timer = 0;

            yield return null;

            while (timer < duration)
            {
                timer += Time.deltaTime;
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, duration * Time.deltaTime);
                yield return null;
            }
        }

        canvasGroup.alpha = 0;
        yield return null;
    }
}
