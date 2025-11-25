using UnityEngine;

public class S_PlayerCrouch : MonoBehaviour
{
    //~ Références
    private S_PlayerController playerController;

    //~ Gestion du crouch
    [HideInInspector] public float speedDecreaser = 1.2f;
    [HideInInspector] public bool isCrouching = false; // Sprint / Accroupissement / EssayeSauter (utile surtout sur un slope)
    private float originalHeight;
    private float crouchHeight = 0.5f;

    private bool canCrouch = true;

    void Start() //& INITIALISATION VARIABLES
    {
        playerController = GetComponent<S_PlayerController>();

        originalHeight = playerController.capsuleCollider.height; // Taille originale du personnage
    }

    void Update()
    {
        if (S_UserInput.instance.CrouchInput && isAbleToCrouch())
        {
            OnCrouch();
        }
    }

    //! --------------- Fonctions privés ---------------

    private void OnCrouch() //& Gestion de l'accroupissement
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

    //? ------------------------------------------------    

    public void setAbleToCrouch(bool enabled)
    {
        canCrouch = enabled;
    } 

    private bool isAbleToCrouch()
    {
        return canCrouch;
    }



}
