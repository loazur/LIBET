using UnityEngine;

public class S_NPCInteractable : MonoBehaviour
{
    [SerializeField] private string interactText; // Texte qui sera affiché sur l'UI

    public void Interact()
    {
        //! LANCE LE DIALOGUE AVEC LE JOUEUR
        Debug.Log("Le joueur a intéragit avec moi !");
    }

    public string getInteractText()
    {
        return interactText;
    }
}
