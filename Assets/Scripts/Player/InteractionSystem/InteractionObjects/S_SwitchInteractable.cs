using UnityEngine;

public class S_SwitchInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de l'interrupteur
    [Header("Gestion de l'interrupteur")]
    private string interactText = "Allumer";
    private bool isOn = false;

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

    }

    private void SwitchOff() //& Eteindre l'interrupteur
    {
        // Changement des valeurs
        isOn = false;
        interactText = "Allumer";


    }
}
