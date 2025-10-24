using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerCrouch : MonoBehaviour
{
    //~ Références
    private S_PlayerController playerController;
    private S_PlayerSprint playerSprint;

    //~ Gestion du crouch
    [HideInInspector] public InputAction crouchAction;
    [HideInInspector] public float speedDecreaser = 1.2f;
    [HideInInspector] public bool isCrouching = false; // Sprint / Accroupissement / EssayeSauter (utile surtout sur un slope)
    private float originalHeight;
    private float crouchHeight = 0.5f;

    void Start() //& INITIALISATION VARIABLES
    {
        playerController = GetComponent<S_PlayerController>();
        playerSprint = GetComponent<S_PlayerSprint>();

        crouchAction = InputSystem.actions.FindAction("Crouch");

        // Lances ces fonctions qu'on on appuis sur la bonne touche
        crouchAction.performed += OnCrouchPerformed;

        originalHeight = playerController.capsuleCollider.height; // Taille originale du personnage
    }

    //! --------------- Fonctions privés ---------------

    private void OnCrouchPerformed(InputAction.CallbackContext context) //& Gestion de l'accroupissement
    {
        //  Se lever
        if (playerController.isGrounded() && isCrouching && canRaise()) // AU SOL / ACCROUPI / PEUT SE LEVER
        {
            playerController.capsuleCollider.height = originalHeight;
                
            transform.localScale = new Vector3(1, 1, 1); // Reset scale 

            isCrouching = false;
            playerController.overheadCheck.SetActive(false); // On désactive overheadCheck
        } // S'accroupir 
        else if (playerController.isGrounded()) // AU SOL
        {
            playerController.capsuleCollider.height = crouchHeight;
            transform.localScale = new Vector3(1, crouchHeight, 1); // Change le scale pour etre pareil que le collider

            playerController.playerRigidbody.AddForce(Vector3.down * 10f, ForceMode.Impulse); // Pour le coller au sol direct et qu'il vole pas

            isCrouching = true;
            playerController.overheadCheck.SetActive(true); // On active overheadCheck
        }
    }

    public bool canRaise() //& Vérifie si le joueur peut se relever
    {
        // Position du centre du SphereCollider Overhead
        Vector3 checkPos = playerController.overheadCheck.transform.position;
        float radius = playerController.colliderOverhead.radius * playerController.overheadCheck.transform.lossyScale.x;

        // On vérifie si une sphère touche un Layer ou "Default"
        return !Physics.CheckSphere(checkPos, radius, LayerMask.GetMask("Default"));
    }
}
