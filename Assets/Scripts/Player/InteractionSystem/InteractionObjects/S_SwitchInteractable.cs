using UnityEngine;

public class S_SwitchInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de l'interrupteur
    [Header("Gestion de l'interrupteur")]
    [SerializeField] private Light lightObject; // Object qui a la light
    [SerializeField] private float offLightIntensity = 0f; // Intensité de la lumière eteinte
    [SerializeField] private float onLightIntensity; // Intensité de la lumière allumé

    private string interactText = "not_set";
    private bool isOn = false; // Allumé ou non

    void Start()
    {
        UpdateInteractText(); // Setup
        
        S_GameSettingsData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        if (isOn)
        {
            SwitchOnOff(false);
        }
        else
        {
            SwitchOnOff(true);
        }


        UpdateInteractText();

    }

    public string getInteractText() //& Texte affiché sur l'UI
    {
        return interactText;
    }

    public Transform getTransform() //& Position du l'interrupteur
    {
        return gameObject.transform;
    }

    //! --------------- Fonctions privés ---------------


    private void SwitchOnOff(bool isOnOrOff) //& Allumer/Eteindre l'interrupteur
    {
        if (isOnOrOff)
        {
            // Changement des valeurs
            isOn = true;

            lightObject.intensity = onLightIntensity;
        }
        else
        {
            // Changement des valeurs
            isOn = false;

            lightObject.intensity = offLightIntensity;
        }
    }

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (!isOn) // Si éteint
        {
            if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.French)
            {
                interactText = "Allumer";
            }
            else if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.English)
            {
                interactText = "Turn on";
            }
        }
        else // Si allumer
        {
            if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.French)
            {
                interactText = "Eteindre";
            }
            else if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.English)
            {
                interactText = "Turn off";
            }
        }
    }
}
