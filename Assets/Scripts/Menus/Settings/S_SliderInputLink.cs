using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderTMPLink : MonoBehaviour
{
    public Slider slider;
    public TMP_InputField inputField;

    void Start()
    {
        // Initialisation de l'input avec la valeur du slider
        inputField.text = slider.value.ToString("0.#");

        // Quand le slider change, mettre à jour l'input
        slider.onValueChanged.AddListener(OnSliderChanged);

        // Quand l'input change, mettre à jour le slider
        inputField.onEndEdit.AddListener(OnInputChanged);
    }

    void OnSliderChanged(float value)
    {
        inputField.text = value.ToString("0.#"); // supprime les décimales si tu veux un int
    }

    void OnInputChanged(string value)
    {
        if (float.TryParse(value, out float result))
        {
            // Limiter la valeur au min/max du slider
            result = Mathf.Clamp(result, slider.minValue, slider.maxValue);
            slider.value = result;
            inputField.text = result.ToString("0.#");
        }
        else
        {
            // Si entrée invalide, remettre la valeur du slider
            inputField.text = slider.value.ToString("0.#");
        }
    }
}
