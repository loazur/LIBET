using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class S_PlayerInteractUI : MonoBehaviour
{
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

    void Update() //& PAS PHYSICS
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

    private void Show(SI_Interactable interactable) //& Affiche l'UI et change le texte en fonction de interactText
    {
        // Si il s'agit d'un nouvel objet
        if (interactable != lastInteractable)
        {
            if (lastInteractableOutline)
                lastInteractableOutline.Disable();

            // Remplace l'objet
            lastInteractable = interactable;

            // Récupère l'outline du nouveau
            lastInteractableOutline = interactable.getTransform().GetComponent<S_InteractableOutline>();
            lastInteractableOutline.Enable();
        }

        uiContainer.SetActive(true); // Active le visuel
        interactText.text = interactable.getInteractText();
    }

    private void Hide() //& Cache l'UI
    {
        uiContainer.SetActive(false); // Désactive le visuel

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
