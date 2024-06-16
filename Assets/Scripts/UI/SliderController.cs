using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{   public string key;
    public float defaultValue;
    public Slider slider;
    public TMP_Text valueText;

    private void Start()
    {
        float val = PlayerPrefs.GetFloat(key, defaultValue);
        slider.value = val;
        valueText.text = val.ToString();
    }

    public void OnSliderChanged(float value)
    {
        valueText.text = value.ToString();
        PlayerPrefs.SetFloat(key, value);
    }
}
