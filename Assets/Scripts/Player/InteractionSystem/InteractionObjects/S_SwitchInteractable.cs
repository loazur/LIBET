using UnityEngine;

public class S_SwitchInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de l'interrupteur
    [Header("Gestion de l'interrupteur")]
    [SerializeField] private Light lightObject; // Object qui a la light
    [SerializeField] private float offLightIntensity; // Intensité de la lumière eteinte
    [SerializeField] private float onLightIntensity; // Intensité de la lumière allumé

    private string interactText = "Allumer";
    private bool isOn = false; // Allumé ou non
    
    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        if (isOn)
        {
            SwitchOff();
        }
        else
        {
            SwitchOn();
        }
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


    private void SwitchOn() //& Allumer l'interrupteur
    {
        // Changement des valeurs
        isOn = true;
        interactText = "Eteindre";

        lightObject.intensity = onLightIntensity;
    }

    private void SwitchOff() //& Eteindre l'interrupteur
    {
        // Changement des valeurs
        isOn = false;
        interactText = "Allumer";

        lightObject.intensity = offLightIntensity;
    }
}
