using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class S_SliderTMPLink : MonoBehaviour
{
    //! S_SliderTMPLink gère la liaison entre un slider et un input field dans un options choisi.

    public Slider slider;
    public TMP_InputField inputField;

    void Start()
    {
        inputField.text = slider.value.ToString("0.##", CultureInfo.InvariantCulture);

        slider.onValueChanged.AddListener(OnSliderChanged);
        inputField.onEndEdit.AddListener(OnInputChanged);
    }

    void OnSliderChanged(float value) //& Met à jour le input field en fonction du slider
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_option_click, S_FMODEvents.instance.target.position);

        inputField.text = value.ToString("0.##", CultureInfo.InvariantCulture);
    }

    void OnInputChanged(string value) //& Met à jour le slider en fonction du input field
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_option_click, S_FMODEvents.instance.target.position);

        if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float result))
        {
            result = Mathf.Clamp(result, slider.minValue, slider.maxValue);
            slider.value = result;
            inputField.text = result.ToString("0.##", CultureInfo.InvariantCulture);
        }
        else
        {
            inputField.text = slider.value.ToString("0.##", CultureInfo.InvariantCulture);
        }
    }
}
