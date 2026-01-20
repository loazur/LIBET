using UnityEngine;

public class S_ItemInteraction : MonoBehaviour, SI_Interactable
{
    [SerializeField] private string id;
    
    [ContextMenu("Generate guid for id")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }

    //~ Gestion de l'item
    [Header("Gestion de l'item")]
    [SerializeField] private string interactText = "not_set"; // Nom de l'objet
    [SerializeField] private float distanceMultiplier = 1.45f; // Distance de l'item quand on le tient
    private float offsetY = 0.6f; // Position vertical de l'item quant on le tient (0.6 = au milieu de l'ecran)
    private S_PlayerInteract playerInteract;
    private S_FirstPersonCamera playerCamera;
    private Rigidbody rigidbodyItem;
    private Collider itemCollider;
    private Transform originalParent; // Utile pour le remettre à son état initial

    [Header("Gestion Lancer")]
    [SerializeField] private float throwForce = 850f; // Force du lancer
    [SerializeField] private float holdThrow = 0.4f; // Combien de temps faut tenir le bouton pour lancer
    private float holdTimer;

    void Start() //& INITIALISATION DE VARIABLES
    {
        holdTimer = holdThrow;

        itemCollider = GetComponent<Collider>();
        rigidbodyItem = GetComponent<Rigidbody>();
        rigidbodyItem.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Detecte les collision plus efficacement lors d'un lancer
        originalParent = transform.parent;
    }

    void LateUpdate() //& Late update car l'objet se déplace après la camera
    {
        HoldingItem();
    }

    //!---------------- SI_DataPersistance ----------------
    /*
    //~ Sauvegarde position/rotation de chaque item

    public void LoadData(S_GameData gameData)
    {
        if (gameData.itemsPosition.TryGetValue(id, out Vector3 itemPosition))
        {
            transform.position = itemPosition;
        }

        if (gameData.itemsRotation.TryGetValue(id, out Quaternion itemRotation))
        {
            transform.rotation = itemRotation;
        }
    }

    public void SaveData(S_GameData gameData)
    {
        if (gameData.itemsPosition.ContainsKey(id))
        {
            gameData.itemsPosition.Remove(id);
        }

        if (gameData.itemsRotation.ContainsKey(id))
        {
            gameData.itemsRotation.Remove(id);
        }

        gameData.itemsPosition.Add(id, transform.position);
        gameData.itemsRotation.Add(id, transform.rotation);
    }
    */

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform) //& Ramasse l'item
    {
        // Récupération des bons components au moment de l'interaction
        playerInteract = playerTransform.GetComponent<S_PlayerInteract>();
        playerCamera = playerTransform.GetComponentInChildren<S_FirstPersonCamera>();

        PickUpItem();
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

    private void PickUpItem() //& Ramasser un item
    {
        if (playerInteract == null || playerInteract.isHoldingItem()) return;

        itemCollider.enabled = false; // Pour ne pas voler

        // Mise à jour des variables pour le bon fonctionnement de HoldingItem()
        rigidbodyItem.useGravity = false;
        rigidbodyItem.isKinematic = true;
        rigidbodyItem.constraints = RigidbodyConstraints.FreezeRotation;
        transform.SetParent(playerInteract.transform.parent);

        playerInteract.setInteractionEnabled(false);
        playerInteract.setHoldingItem(this);

        // Notifier l'événement global que le joueur a ramassé un item
        if (S_GameManager.instance != null)
        {
            S_GameManager.instance.playerEvents.ItemPickedUp(gameObject);
        }
    }

    private void HoldingItem() //& Gestion lorsqu'on tient un item
    {
        if (playerInteract == null || !playerInteract.isHoldingItem()) return;

        if (S_UserInput.instance.CancelInteractionAction.WasReleasedThisFrame()) // Action de lacher
        {
            DropItem();
            return;
        }

        if (S_UserInput.instance.CancelInteractionAction.IsPressed()) // Action de lancer
        {
            holdTimer -= Time.deltaTime;

            if (holdTimer < 0)
            {
                ThrowItem();
                return;
            }
        }
        else
        {
            holdTimer = holdThrow; // Remet le timer à 0
        }

        // Gestion des mouvements de l'item
        Vector3 targetPos =
            playerInteract.transform.position + playerCamera.transform.forward * // Part de la position du joueur, vers l'avant de la camera
            distanceMultiplier + // En fonction de la distance choisi
            Vector3.up * offsetY;


        transform.SetPositionAndRotation(targetPos, playerInteract.transform.rotation);
    }

    private void DropItem() //& Poser un item
    {
        if (playerInteract == null || !playerInteract.isHoldingItem()) return;

        itemCollider.enabled = true; // On le réactive pour pouvoir detecté l'interaction

        // Ne pas mettres les items dans d'autres objets
        Vector3 hitPos = castRaycastBetweenCamAndItem();
        if (hitPos != Vector3.zero)
        {
            transform.position = hitPos;
        }

        ReEnableInteractionsAndRB();

        // Déstruction components au moment de drop
        playerInteract = null;
        playerCamera = null;
    }

    private void ThrowItem() //& Lancer un item
    {
        if (playerInteract == null || !playerInteract.isHoldingItem()) return;

        itemCollider.enabled = true; // On le réactive pour pouvoir detecté l'interaction

        // Ne pas mettres les items dans d'autres objets
        Vector3 hitPos = castRaycastBetweenCamAndItem();
        if (hitPos != Vector3.zero) 
        {
            transform.position = hitPos;
        }

        ReEnableInteractionsAndRB(); // Avant le AddForce pour réactivé la physique

        rigidbodyItem.AddForce(playerCamera.transform.forward * throwForce); // LANCEMENT DANS LA DIRECTION OU LE JOUEUR REGARDE

        // Déstruction components au moment de throw
        playerInteract = null;
        playerCamera = null;
    }

    private void ReEnableInteractionsAndRB() //& Réactive tout ce qui avait été desactivé lors de PickupItem()
    {
        rigidbodyItem.useGravity = true;
        rigidbodyItem.isKinematic = false;
        rigidbodyItem.constraints = RigidbodyConstraints.None;
        transform.SetParent(originalParent);

        playerInteract.setInteractionEnabled(true);
        playerInteract.setHoldingItem(null);
    }

    private Vector3 castRaycastBetweenCamAndItem() //& Retourne la position de la fin du raycast si il y a un objet entre l'item et la camera
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
