using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerSprint : MonoBehaviour
{
    //~ Références
    private S_PlayerController playerController;

    //~ Gestion du sprint
    [HideInInspector] public InputAction sprintAction;
    [HideInInspector] public bool isSprinting = false;
    public float sprintMultiplier; // 1.4f

    void Start() //& INITIALISATION VARIABLES
    {
        playerController = GetComponent<S_PlayerController>();
        sprintAction = InputSystem.actions.FindAction("Sprint");
    }

    void Update() //& PAS PHYSICS
    {
        isSprinting = sprintAction.IsPressed();
    }

    //! --------------- Fonctions privés ---------------

    public void Sprint(ref Vector3 movementVector3) //& Gére le sprint du joueur
    {
        movementVector3 *= sprintMultiplier;
        playerController.meshRenderer.material = playerController.sprintMat; // Change le material lorsque le joueur cours

    }
}
