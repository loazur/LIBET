using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_TextTranslator : MonoBehaviour
{
    //~ Permet de gérer la traduction en différent de la langue actuel
    [SerializeField] private List<string> textInEachLanguages = new List<string>();
    [SerializeField] private bool attachedToKeybind = false; // Si le script est attaché à une touche
    private Text textContainerKeybind;
    private TMPro.TMP_Text textContainer;

    void Start()
    {
        textContainer = GetComponent<TMPro.TMP_Text>();

        if (attachedToKeybind)
        {
            textContainerKeybind = GetComponent<Text>();
        }

        UpdateText();

        S_GameSettingsData.instance.OnLanguageChanged += UpdateText;
    }


    //! --------------- Fonctions principales ---------------

    private void UpdateText()
    {
        if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.French)
        {
            if (attachedToKeybind)
            {
                textContainerKeybind.text = textInEachLanguages[(int)S_GameSettingsData.Languages.French];
                return;
            }

            textContainer.text = textInEachLanguages[(int)S_GameSettingsData.Languages.French];
        }
        else if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.English)
        {   
            if (attachedToKeybind)
            {
                textContainerKeybind.text = textInEachLanguages[(int)S_GameSettingsData.Languages.English];
                return;
            }

            textContainer.text = textInEachLanguages[(int)S_GameSettingsData.Languages.English];
        }
    }

}
