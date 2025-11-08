using UnityEngine;
using TMPro;

public class S_PlayerInteractUI : MonoBehaviour
{
    //~ Gestion de l'UI
    [Header("Gestion de l'UI")]
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private S_PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI keybind;
    [SerializeField] private TextMeshProUGUI interactText;

    void Update() //& PAS PHYSICS
    {
        if (playerInteract.GetInteractableObject() != null && playerInteract.canInteract()) // Est à portée d'une interaction
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
        uiContainer.SetActive(true); // Active le visuel
        interactText.text = interactable.getInteractText();
    }

    private void Hide() //& Cache l'UI
    {
        uiContainer.SetActive(false); // Désactive le visuel
    }
    
    

}
