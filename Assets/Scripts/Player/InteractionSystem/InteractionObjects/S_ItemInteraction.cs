using UnityEngine;
using UnityEngine.InputSystem;

public class S_ItemInteraction : MonoBehaviour, SI_Interactable
{
    //~ Gestion de l'item
    [Header("Gestion de l'item")]
    [SerializeField] private S_PlayerInteract playerInteract;
    [SerializeField] private string interactText; // Nom de l'objet
    private InputAction interactAction;

    void Start()
    {
        interactAction = InputSystem.actions.FindAction("Interact");
    }

    void Update()
    {
        HoldingItem();
    }


    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform) //& Ramasse l'item
    {
        PickUp();
    }

    public string getInteractText()  //& Retourne le nom de l'item
    {
        return interactText;
    }

    public Transform getTransform() //& Position de l'item
    {
        return gameObject.transform;
    }

    //! --------------- Fonctions privés ---------------

    private void PickUp() //& Ramasser un item
    {
        if (playerInteract.isHoldingItem())
        {
            return;
        }

        //! Ramasser
        Debug.Log("Attraper " + interactText);
        playerInteract.DisableInteractions();
        playerInteract.setHoldingItem(true);
    }

    private void HoldingItem() //& Gestion lorsqu'on tient un item
    {
        if (playerInteract.isHoldingItem())
        {
            if (interactAction.WasReleasedThisFrame())
            {
                Drop();
            }

            //! Gestion jeter
        }
    }

    private void Drop() //& Lacher un item
    {
        if (!playerInteract.isHoldingItem())
        {
            return;
        }

        //! Lacher
        Debug.Log("Lacher " + interactText);
        playerInteract.EnableInteractions();
        playerInteract.setHoldingItem(false);
    }
    
    private void Throw() //& Lancer un item
    {
        if (!playerInteract.isHoldingItem())
        {
            return;
        }

        //! Lancer
        Debug.Log("Jeter " + interactText);
        playerInteract.EnableInteractions();
        playerInteract.setHoldingItem(false);
    }
}
