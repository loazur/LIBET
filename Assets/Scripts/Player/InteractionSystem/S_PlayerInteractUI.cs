using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class S_PlayerInteractUI : MonoBehaviour
{
    //! S_PlayerInteractUI gère l'affichage de l'UI lié à l'interaction mais aussi l'activation de l'outline de l'objet.

    //~ Gestion de l'UI
    [Header("Gestion de l'UI")]
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private S_PlayerInteract playerInteract;
    [SerializeField] private TextMeshProUGUI keybind;
    [SerializeField] private TextMeshProUGUI interactText;
    private string lastBinding; // Dernière touche

    //~ Gestion de l'outline de l'interaction
    private SI_Interactable lastInteractable;
    private S_InteractableOutline lastInteractableOutline;

    void Start() //& Change le texte pour que soit de la bonne touche
    {
        lastBinding = S_UserInput.instance._interactAction.GetBindingDisplayString();
        UpdateKeybindText();
    }

    void Update() //& Met à jour l'affichage de la touche d'interaction + Affiche l'UI ou non
    {
        // Vérifie si la touche n'a pas changé
        string currentBinding = S_UserInput.instance._interactAction.GetBindingDisplayString();

        if (currentBinding != lastBinding)
        {
            lastBinding = currentBinding;
            UpdateKeybindText();
        }

        // Gére l'affichage de l'UI si à portée d'une interaction
        if (playerInteract.GetInteractableObject() != null && playerInteract.canInteract())
        {
            Show(playerInteract.GetInteractableObject());
        }
        else
        {
            Hide();
        }
    }

    //! --------------- Fonctions privés ---------------

    private void Show(SI_Interactable interactable)
    {
        if (interactable != lastInteractable)
        {
            if (lastInteractableOutline != null)
                lastInteractableOutline.Disable();

            lastInteractable = interactable;

            // Récupération sécurisée de l'outline
            lastInteractableOutline = interactable.getTransform().GetComponent<S_InteractableOutline>();

            if (lastInteractableOutline != null)
                lastInteractableOutline.Enable();
        }

        if (uiContainer != null)
            uiContainer.SetActive(true);

        if (interactText != null)
            interactText.text = interactable.getInteractText();
    }


    private void Hide() //& Cache l'UI
    {
        if (uiContainer != null)
        uiContainer.SetActive(false);

        if (lastInteractableOutline) // Si possède une outline la désactive
            lastInteractableOutline.Disable();

        lastInteractable = null;
        lastInteractableOutline = null;
    }

    //? ------------------------------------------------

    public void UpdateKeybindText() //& Met à jour l'UI de la touche
    {
        keybind.text = S_UserInput.instance._interactAction.GetBindingDisplayString();
    }
    
    
    

}
