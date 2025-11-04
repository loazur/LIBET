using UnityEngine;
using UnityEngine.InputSystem;

public class S_ItemInteraction : MonoBehaviour, SI_Interactable
{
    //~ Gestion de l'item
    [Header("Gestion de l'item")]
    [SerializeField] private S_PlayerInteract playerInteract;
    [SerializeField] private S_FirstPersonCamera playerCamera;
    [SerializeField] private string interactText; // Nom de l'objet
    private Rigidbody rigidbodyItem;
    private InputAction dropThrowAction;
    private Transform originalParent; // Utile pour le remettre à son état initial


    void Start()
    {
        dropThrowAction = InputSystem.actions.FindAction("CancelInteraction");
        originalParent = transform.parent;
        rigidbodyItem = GetComponent<Rigidbody>();
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

        rigidbodyItem.useGravity = false;
        rigidbodyItem.isKinematic = true;
        rigidbodyItem.constraints = RigidbodyConstraints.FreezeRotation;
        transform.SetParent(playerInteract.transform);

        playerInteract.setInteractionEnabled(false);
        playerInteract.setHoldingItem(true);
    }

    private void HoldingItem() //& Gestion lorsqu'on tient un item
    {
        if (!playerInteract.isHoldingItem())
        {
            return;
        }

        // Gestion des mouvements de l'item
        Vector3 targetPos = playerInteract.transform.position + playerCamera.transform.forward * 1.3f;
        transform.position = targetPos;
        transform.rotation = playerInteract.transform.rotation; // Suit la rotation du joueur

        if (dropThrowAction.WasReleasedThisFrame()) // Action de lacher
        {
            Drop();
        }

        //! Manque jeter
    }

    private void Drop()
    {
        if (!playerInteract.isHoldingItem()) return;

        rigidbodyItem.useGravity = true;
        rigidbodyItem.isKinematic = false;
        rigidbodyItem.constraints = RigidbodyConstraints.None;
        transform.SetParent(originalParent);

        playerInteract.setInteractionEnabled(true);
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
        playerInteract.setInteractionEnabled(true);
        playerInteract.setHoldingItem(false);
    }
}
