using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Globalization;

public class SliderTMPLink : MonoBehaviour
{
    public Slider slider;
    public TMP_InputField inputField;

    void Start()
    {
        inputField.text = slider.value.ToString("0.##", CultureInfo.InvariantCulture);

        slider.onValueChanged.AddListener(OnSliderChanged);
        inputField.onEndEdit.AddListener(OnInputChanged);
    }

    void OnSliderChanged(float value)
    {
        inputField.text = value.ToString("0.##", CultureInfo.InvariantCulture);
    }

    void OnInputChanged(string value)
    {
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
