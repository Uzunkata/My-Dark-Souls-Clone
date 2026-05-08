using System;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Slider))]

[RequireComponent(typeof(RectTransform))]
public class UI_StatBar : MonoBehaviour
{
    
    //TODO: add second bar behind, that indicates in yellow how much stamina an action consumed (HP an enemy hit had consumed)
    private Slider slider;
    private RectTransform rectTransform;

    [Header("Bar Options")]
    [SerializeField] protected bool scaleBarLenghtWithStat = true;
    [SerializeField] protected float widthScaleMultiplier = 1;

    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
    }

    public virtual void SetStat(float newValue)
    {
        slider.value = Mathf.RoundToInt(newValue);
    }

    public virtual void SetMaxStat(float maxValue)
    {
        int value = Mathf.RoundToInt(maxValue);
         
        slider.maxValue = value;
        slider.value = value;

        if (scaleBarLenghtWithStat)
        {
            rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMultiplier, rectTransform.sizeDelta.y);
            PlayerUIManager.GetInstance.PlayerUIHudManager.RefreshHUD();
        }
    }
}
