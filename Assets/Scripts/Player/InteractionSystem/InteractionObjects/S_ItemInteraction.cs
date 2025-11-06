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


    void Start()
    {
        dropThrowAction = InputSystem.actions.FindAction("CancelInteraction");
        itemCollider = GetComponent<Collider>();
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
        if (playerInteract.isHoldingItem()) return;

        itemCollider.enabled = false; // Pour ne pas voler

        rigidbodyItem.useGravity = false;
        rigidbodyItem.isKinematic = true;
        rigidbodyItem.constraints = RigidbodyConstraints.FreezeRotation;
        transform.SetParent(playerInteract.transform);

        playerInteract.setInteractionEnabled(false);
        playerInteract.setHoldingItem(true);
    }

    private void HoldingItem() //& Gestion lorsqu'on tient un item
    {
        if (!playerInteract.isHoldingItem()) return;

        if (dropThrowAction.WasReleasedThisFrame()) // Action de lacher
        {
            Drop();
        }

        // Gestion des mouvements de l'item
        Vector3 targetPos =
            playerInteract.transform.position + playerCamera.transform.forward * // Part de la position du joueur, vers l'avant de la camera
            distanceMultiplier + // En fonction de la distance choisi
            Vector3.up * offsetY; // Ajout l'offsetY vers le haut

        transform.SetPositionAndRotation(targetPos, playerInteract.transform.rotation);

        //! Manque jeter
    }

    private void Drop()
    {
        if (!playerInteract.isHoldingItem()) return;

        itemCollider.enabled = true; // On le réactive pour pouvoir detecté l'interaction

        //!Marche pas
        Vector3 hitPos = castRaycast();
        Vector3 finalPos = transform.position;

        if (hitPos != Vector3.zero)
        {
            finalPos = hitPos + (transform.position - playerCamera.transform.position).normalized * 0.05f;
        }

        rigidbodyItem.useGravity = true;
        rigidbodyItem.isKinematic = false;
        rigidbodyItem.constraints = RigidbodyConstraints.None;
        transform.SetParent(originalParent);

        transform.position = finalPos;

        playerInteract.setInteractionEnabled(true);
        playerInteract.setHoldingItem(false);
    }


    private void Throw() //& Lancer un item
    {
        if (!playerInteract.isHoldingItem()) return;

        //! Lancer
        Debug.Log("Jeter " + interactText);
        playerInteract.setInteractionEnabled(true);
        playerInteract.setHoldingItem(false);
    }
    
    private Vector3 castRaycast() //& Retourne la position du raycast si il y a un objet entre l'item et la camera
    {
        Vector3 camPos = playerCamera.transform.position;
        Vector3 itemPos = transform.position;

        Vector3 dir = (itemPos - camPos).normalized;
        float dist = Vector3.Distance(camPos, itemPos);

        if (Physics.Raycast(camPos, dir, out RaycastHit hit, dist))
        {
            if (hit.collider.transform == transform) return Vector3.zero; // Pour pas se détecter lui même

            Debug.DrawRay(camPos, dir * hit.distance, Color.red);
            Debug.Log("Objet détecté : " + hit.collider.name + " Pos : " + hit.point);

            return hit.point;
        }

        return Vector3.zero;

    }

}
