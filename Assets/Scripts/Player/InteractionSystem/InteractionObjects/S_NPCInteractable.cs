using UnityEngine;

public class S_NPCInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de l'interaction du NPC
    [SerializeField] private S_DialogueTrigger dialogueTrigger;
    [SerializeField] private string interactText; // Texte à afficher en fonction du NPC

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        //! LANCE LE DIALOGUE AVEC LE JOUEUR
        dialogueTrigger.TriggerDialogue();
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
