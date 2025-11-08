using UnityEngine;

public class S_PlayerSprint : MonoBehaviour
{
    //~ Références
    private S_PlayerController playerController;

    //~ Gestion du sprint
    [SerializeField] private float sprintMultiplier = 1.5f;

    void Start() //& INITIALISATION VARIABLES
    {
        playerController = GetComponent<S_PlayerController>();
    }

    //! --------------- Fonctions privés ---------------

    public void Sprint(ref Vector3 movementVector3) //& Gére le sprint du joueur
    {
        movementVector3 *= sprintMultiplier;
        playerController.meshRenderer.material = playerController.sprintMat; // Change le material lorsque le joueur cours

    }
}
