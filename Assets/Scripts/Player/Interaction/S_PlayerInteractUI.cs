using UnityEngine;
using TMPro;

public class S_PlayerInteractUI : MonoBehaviour
{
    //~ Gestion de l'UI
    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private S_PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI interactText;

    void Update() //& PAS PHYSICS
    {
        if (playerInteract.GetInteractableObject() != null) // Est à portée d'une interaction
        {
            Show(playerInteract.GetInteractableObject());
        }
        else
        {
            Hide();
        }
    }

    //! --------------- Fonctions privés ---------------

    private void Show(SI_Interactable interactable) //& Affiche l'UI et change le texte en fonction de interactText
    {
        containerGameObject.SetActive(true);
        interactText.text = interactable.getInteractText();
    }
    
    private void Hide() //& Cache l'UI
    {
        containerGameObject.SetActive(false);
    }

}
