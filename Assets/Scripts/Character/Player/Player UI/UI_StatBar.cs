using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class UI_StatBar : MonoBehaviour
{
    //TODO: scale the bar size depending on stat (HP/FP/Stamina)
    //TODO: add second bat behind, that indicates in yellow how much stamina an action consumed (HP an enemy hit had consumed)
    private Slider slider;

    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public virtual void SetStat(float newValue)
    {
        slider.value =  Mathf.RoundToInt(newValue);
    }

    public virtual void SetMaxStat(float maxValue)
    {
        int value = Mathf.RoundToInt(maxValue);
         
        slider.maxValue = value;
        slider.value = value;

        Debug.Log(slider.maxValue + " " +  slider.value);
    }
}
