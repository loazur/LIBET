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
        uiContainer.SetActive(true); // Active le visuel
        interactText.text = interactable.getInteractText();
    }

    private void Hide() //& Cache l'UI
    {
        uiContainer.SetActive(false); // Désactive le visuel
    }

    //? ------------------------------------------------

    public void UpdateKeybindText() //& Met à jour l'UI de la touche
    {
        keybind.text = S_UserInput.instance._interactAction.GetBindingDisplayString();
    }
    
    
    

}
