// ! Info sur le script : 
// ! appliquer le script sur l'objet où on veut s'asseoir


// TODO : faire une animation


using UnityEngine;

public class S_ChairInteractable : MonoBehaviour, SI_Interactable
{
    private string interactText = "S'asseoir";
    [SerializeField] private GameObject player;
    [SerializeField] private S_FirstPersonCamera playerCamera;
    private S_PlayerController playerController;
    private bool isPlayerSitting = false;

    // & ===================================================
    
    
    


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
        UnityEngine.Vector3 chairPosition_Center = transform.position + new UnityEngine.Vector3(0, 0.5f, 0);


        player.transform.position = chairPosition_Center;

        // Bloquer les mouvements du joueur
        playerController.DisableMovements();
        CameraLock();


        interactText = "Se lever";
    }

    private void UnsitPlayer()
    {

        // Mettre le joueur debout à coté de la chaise
        UnityEngine.Vector3 chairPosition_Side = transform.position + transform.right * 1.0f;
        player.transform.position = chairPosition_Side;


        // Débloquer les mouvements du joueur
        playerController.EnableMovements();
        CameraLock();


        interactText = "S'asseoir";
    }

    private void CameraLock()
    {
        if (isPlayerSitting)
        {
            playerCamera.DisableRotation();

            // Orientation de la caméra alignée avec la chaise
            Vector3 forward = transform.forward;
            forward.y = 0; // on garde l’horizontale
            forward.Normalize();

            playerCamera.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }
        else
        {
            playerCamera.EnableRotation();
        }
    }


}
