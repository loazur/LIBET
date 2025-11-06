// ! Info sur le script : 
// ! appliquer le script sur l'objet où on veut s'asseoir


// TODO : faire une animation


using UnityEngine;
using UnityEngine.InputSystem;

public class S_ChairInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de chaises
    [Header("Gestion de la chaise")]
    [SerializeField] private GameObject player;
    [SerializeField] private S_FirstPersonCamera playerCamera;
    [SerializeField] private Collider chairCollider; // Collider a désactivé/activer en fonction de si on est assis
    private InputAction getUpAction;
    private string interactText = "S'asseoir";

    private S_PlayerController playerController;
    private bool isPlayerSitting = false;

    void Start()
    {
        playerController = player.GetComponent<S_PlayerController>();
        getUpAction = InputSystem.actions.FindAction("CancelInteraction"); 
    }

    void Update()
    {
        if (isPlayerSitting && getUpAction.WasPressedThisFrame())
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
        
        isPlayerSitting = true;

        interactText = "Se lever";
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

        isPlayerSitting = false;

        interactText = "S'asseoir";
    }

}
