// ! Info sur le script : 
// ! appliquer le script sur l'objet où on veut s'asseoir


// TODO : faire une animation


using UnityEngine;

public class S_ChairInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de chaises
    [Header("Gestion de la chaise")]
    [SerializeField] private S_DisplayMenus displayMenus; // Pour désactivé le menu quand on est assis
    [SerializeField] private Collider chairCollider; // Collider a désactivé/activer en fonction de si on est assis
    private GameObject player;
    private S_PlayerController playerController;
    private S_FirstPersonCamera playerCamera;
    private string interactText = "not_set";

    private bool isPlayerSitting = false;

    void Start()
    {
        UpdateInteractText(); // Setup
 
        S_GameSettingsData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    void Update()
    {
        if (isPlayerSitting && S_UserInput.instance.CancelInteractionAction.WasPressedThisFrame())
        {
            GetUp();
        }
    }

    // * ===================================================================================
    // * Ne pas retirer ce qui est en dessous, nécessaire pour l'interface SI_Interactable
    // * ===================================================================================

    // ~ Méthode qui est activer quand on interagit avec l'objet
    public void Interact(Transform playerTransform)
    {
        if (!isPlayerSitting)
        {
            // Récupère les components au moment de l'interaction
            player = playerTransform.gameObject;
            playerController = player.GetComponent<S_PlayerController>();
            playerCamera = playerTransform.GetComponentInChildren<S_FirstPersonCamera>();

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

    // * ===================================================================================

    // Teleporte le joueur à la position assise
    private void Sit() //& S'assoir
    {
        // milieu de la chaise
        Vector3 chairPosition_Center = transform.position + new Vector3(0, 0.5f, 0);

        player.transform.position = chairPosition_Center;
        playerCamera.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); //! Pas oublier de changer en fonction de l'angle de la chaise

        // Bloquer les mouvements du joueur
        playerController.setMovementsEnabled(false);
        playerCamera.setRotationEnabled(false);
        chairCollider.enabled = false;
        displayMenus.setAbleToOpenCloseMenu(false);

        isPlayerSitting = true;

        UpdateInteractText();
    }

    private void GetUp() //& Se lever
    {
        // Mettre le joueur debout à coté de la chaise
        Vector3 chairPosition_Side = transform.position + transform.right * 1.0f;
        player.transform.position = chairPosition_Side;

        // Débloquer les mouvements du joueur
        playerController.setMovementsEnabled(true);
        playerCamera.setRotationEnabled(true);
        chairCollider.enabled = true;
        displayMenus.setAbleToOpenCloseMenu(true);
        

        isPlayerSitting = false;

        // Détruit les components 
        player = null;
        playerController = null;
        playerCamera = null;

        UpdateInteractText();
    }
    
    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (!isPlayerSitting) // Si Debout
        {
            if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.French)
            {
                interactText = "S'asseoir";
            }
            else if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.English)
            {
                interactText = "Sit down";
            }
        }
        else // Si Assis
        {
            if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.French)
            {
                interactText = "Se lever";
            }
            else if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.English)
            {
                interactText = "Get up";
            }
        }
    }

}
