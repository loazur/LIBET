// ! Info sur le script : 
// ! appliquer le script sur l'objet où on veut s'asseoir


// TODO : faire une animation


using UnityEngine;

public class S_ChairInteractable : MonoBehaviour, SI_Interactable
{
    private string interactText = "S'asseoir";
    [SerializeField] private GameObject player;
    [SerializeField] private S_FirstPersonCamera playerCamera;
    [SerializeField] private Collider chairCollider; // Objet qui contient le collider a désactivé/activer en fonction de si on est assis

    private S_PlayerController playerController;
    private bool isPlayerSitting = false;

    void Start()
    {
        playerController = player.GetComponent<S_PlayerController>();
    }

    // * ===================================================================================
    // * Ne pas retirer ce qui est en desssous, nécessaire pour l'interface SI_Interactable
    // * ===================================================================================

    // ~ Méthode qui est activer quand on interagit avec l'objet
    public void Interact(Transform playerTransform)
    {
        if (!isPlayerSitting)
        {
            isPlayerSitting = true;
            SitPlayer();
        }
        else
        {
            isPlayerSitting = false;
            UnsitPlayer();
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
    private void SitPlayer()
    {
        // milieu de la chaise
        Vector3 chairPosition_Center = transform.position + new Vector3(0, 0.5f, 0);

        player.transform.position = chairPosition_Center;
        playerCamera.transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0); //! Pas oublier de changer en fonction de l'angle de la chaise

        // Bloquer les mouvements du joueur
        playerController.DisableMovements();
        playerCamera.DisableRotation();
        chairCollider.enabled = false;


        interactText = "Se lever";
    }

    private void UnsitPlayer()
    {
        // Mettre le joueur debout à coté de la chaise
        Vector3 chairPosition_Side = transform.position + transform.right * 1.0f;
        player.transform.position = chairPosition_Side;

        // Débloquer les mouvements du joueur
        playerController.EnableMovements();
        playerCamera.EnableRotation();
        chairCollider.enabled = true;

        interactText = "S'asseoir";
    }

}
