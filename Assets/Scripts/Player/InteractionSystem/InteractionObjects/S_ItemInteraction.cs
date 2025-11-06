using UnityEngine;
using UnityEngine.InputSystem;

public class S_ItemInteraction : MonoBehaviour, SI_Interactable
{
    //~ Gestion de l'item
    [Header("Gestion de l'item")]
    [SerializeField] private S_PlayerInteract playerInteract;
    [SerializeField] private S_FirstPersonCamera playerCamera;
    [SerializeField] private string interactText; // Nom de l'objet
    [SerializeField] private float distanceMultiplier; // Distance de l'item quand on le tient
    [SerializeField] private float offsetY; // Position vertical de l'item quant on le tient (0.5 = au milieu de l'ecran)
    private Rigidbody rigidbodyItem;
    private Collider itemCollider;
    private InputAction dropThrowAction;
    private Transform originalParent; // Utile pour le remettre à son état initial


    void Start() //& INITIALISATION DE VARIABLES
    {
        dropThrowAction = InputSystem.actions.FindAction("CancelInteraction");
        itemCollider = GetComponent<Collider>();
        originalParent = transform.parent;
        rigidbodyItem = GetComponent<Rigidbody>();
    }

    void LateUpdate() //& Late update car l'objet se déplace après la camera
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
        if (playerInteract.isHoldingItem()) return;

        itemCollider.enabled = false; // Pour ne pas voler

        rigidbodyItem.useGravity = false;
        rigidbodyItem.isKinematic = true;
        rigidbodyItem.constraints = RigidbodyConstraints.FreezeRotation;
        transform.SetParent(playerInteract.transform);

        playerInteract.setInteractionEnabled(false);
        playerInteract.setHoldingItem(this);
    }

    private void HoldingItem() //& Gestion lorsqu'on tient un item
    {
        if (!playerInteract.isHoldingItem()) return;

        if (dropThrowAction.WasReleasedThisFrame()) // Action de lacher
        {
            Drop();
            return;
        }

        //! Manque jeter
        //...

        // Gestion des mouvements de l'item
        Vector3 targetPos =
            playerInteract.transform.position + playerCamera.transform.forward * // Part de la position du joueur, vers l'avant de la camera
            distanceMultiplier + // En fonction de la distance choisi
            Vector3.up * offsetY; // Ajout l'offsetY vers le haut

        transform.SetPositionAndRotation(targetPos, playerInteract.transform.rotation);
    }

    
    private void Drop()
    {
        if (!playerInteract.isHoldingItem()) return;

        itemCollider.enabled = true; // On le réactive pour pouvoir detecté l'interaction

        Vector3 hitPos = castRaycast();

        if (hitPos != Vector3.zero) // Si essaye de poser l'item dans le mur
        {
            transform.position = hitPos;
        }
        

        rigidbodyItem.useGravity = true;
        rigidbodyItem.isKinematic = false;
        rigidbodyItem.constraints = RigidbodyConstraints.None;
        transform.SetParent(originalParent);

        playerInteract.setInteractionEnabled(true);
        playerInteract.setHoldingItem(null);
    }
    
    private void Throw() //& Lancer un item
    {
        if (!playerInteract.isHoldingItem()) return;

        //! Lancer
        Debug.Log("Jeter " + interactText);
        playerInteract.setInteractionEnabled(true);
        playerInteract.setHoldingItem(null);
    }
    
    private Vector3 castRaycast() //& Retourne la position de la fin du raycast si il y a un objet entre l'item et la camera
    {
        Vector3 camPos = playerCamera.transform.position;
        Vector3 itemPos = transform.position;

        if (Physics.Linecast(camPos, itemPos, out RaycastHit hit)) // Lance le raycast entre la camera et l'item
        {
            if (hit.collider.transform == transform) // Pour pas se détecter lui même
            {
                return Vector3.zero; 
            }
            else // Un objet est entre les deux
            {
                return hit.point; 
            } 
        }

        return Vector3.zero; // Aucun objet detecté
    }

}
