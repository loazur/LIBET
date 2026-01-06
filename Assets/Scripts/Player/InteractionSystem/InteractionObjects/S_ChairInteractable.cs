using UnityEngine;

public class S_ChairInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de chaises
    [Header("Gestion de la chaise")]
    [SerializeField] private Collider chairCollider; // Collider a désactivé/activer en fonction de si on est assis
    private GameObject player;
    private S_PlayerController playerController;
    private S_FirstPersonCamera playerCamera;
    private S_PlayerCrouch playerCrouch;
    private Collider playerCollider;
    private Rigidbody playerRigidBody;
    private string interactText = "not_set";

    private bool isPlayerSitting = false;

    void Start()
    {
        UpdateInteractText(); // Setup
 
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    void Update()
    {
        if (isPlayerSitting && S_UserInput.instance.CancelInteractionAction.WasPressedThisFrame())
        {
            GetUp();
        }
    }

    //! Méthodes provenant de l'interface SI_Interactable

    // ~ Méthode qui est activer quand on interagit avec l'objet
    public void Interact(Transform playerTransform)
    {
        if (!isPlayerSitting)
        {
            // Récupère les components au moment de l'interaction
            player = playerTransform.gameObject;
            playerController = player.GetComponent<S_PlayerController>();
            playerCamera = playerTransform.GetComponentInChildren<S_FirstPersonCamera>();
            playerCrouch = playerController.GetComponentInChildren<S_PlayerCrouch>();
            playerRigidBody = player.GetComponent<Rigidbody>();
            playerCollider = player.GetComponent<Collider>();

            Sit();
        }
    }

    public string getInteractText()
    {
        return interactText;
    }

    public Transform getTransform()
    {
        return gameObject.transform;
    }

    //! -------------------------------------------------------

    private void Sit() //& S'assoir
    {
        if (playerCrouch.isCrouching) return;

        // milieu de la chaise
        Vector3 chairPosition_Center = transform.position + new Vector3(0, 0.5f, 0);

        player.transform.position = chairPosition_Center;
        player.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); // Aligner le joueur avec la chaise
        playerCamera.transform.localRotation = Quaternion.identity; // Réinitialiser la rotation locale de la caméra

        // Désactivé les collisions quand il est assis
        chairCollider.enabled = false;
        playerCollider.enabled = false;
        playerRigidBody.useGravity = false;
        playerRigidBody.isKinematic = true;

        // Bloquer les mouvements du joueur
        playerController.setMovementsEnabled(false);
        playerCrouch.setAbleToCrouch(false);
        S_HandlerPauseMenu.instance.setAbleToOpenClosePauseMenu(false);

        //Activation limitation des mouvements de camera
        playerCamera.setHorizontalLimitEnabled(true);

        isPlayerSitting = true;

        // Notifier le système de quêtes
        if (S_GameManager.instance != null)
        {
            S_GameManager.instance.playerEvents.PlayerSat(gameObject);
        }

        UpdateInteractText();
    }

    private void GetUp() //& Se lever
    {
        // Mettre le joueur debout à coté de la chaise
        Vector3 chairPosition_Side = transform.position + transform.right * 1.0f;
        player.transform.position = chairPosition_Side;

        // Désactivé les collisions quand il est assis
        chairCollider.enabled = true;
        playerCollider.enabled = true;
        playerRigidBody.useGravity = true;
        playerRigidBody.isKinematic = false;


        // Débloquer les mouvements du joueur
        playerController.setMovementsEnabled(true);
        playerCrouch.setAbleToCrouch(true);
        S_HandlerPauseMenu.instance.setAbleToOpenClosePauseMenu(true);

        // Désactivation limitation des mouvements de camera
        playerCamera.setHorizontalLimitEnabled(false);
        
        isPlayerSitting = false;

        // Détruit les components 
        player = null;
        playerController = null;
        playerCamera = null;
        playerCrouch = null;
        playerRigidBody = null;
        playerCollider = null;

        UpdateInteractText();
    }
    
    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (!isPlayerSitting) // Si Debout
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                interactText = "S'asseoir";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                interactText = "Sit down";
            }
        }
        else // Si Assis
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                interactText = "Se lever";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                interactText = "Get up";
            }
        }
    }

}
