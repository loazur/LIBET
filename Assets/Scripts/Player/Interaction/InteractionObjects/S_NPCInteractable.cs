using UnityEngine;

public class S_NPCInteractable : MonoBehaviour, SI_Interactable
{
    //~ Implémente l'interface d'interaction
    [SerializeField] private string interactText; // Texte à afficher en fonction du NPC

    //! Méthodes provenant de l'interface

    public void Interact()
    {
        //! LANCE LE DIALOGUE AVEC LE JOUEUR
        Debug.Log("Le joueur essaye de parler avec " + gameObject.name);
    }

    public string getInteractText() //& Texte affiché sur l'UI
    {
        return interactText;
    }

    public Transform getTransform() //& Position du NPC
    {
        return gameObject.transform;
    }
}
