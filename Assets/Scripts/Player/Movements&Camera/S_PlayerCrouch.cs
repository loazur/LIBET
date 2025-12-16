using UnityEngine;

public class S_PlayerCrouch : MonoBehaviour, SI_DataPersistance
{
    //~ Références
    private S_PlayerController playerController;

    //~ Gestion du crouch
    [HideInInspector] public float speedDecreaser = 1.2f;
    [HideInInspector] public bool isCrouching = false;
    private float originalHeight;
    private float crouchHeight = 0.5f;

    private bool canCrouch = true;

    void Awake() //& INITIALISATION VARIABLES
    {
        playerController = GetComponent<S_PlayerController>();
        
        if (playerController != null && playerController.capsuleCollider != null) // Vérification
        {
            originalHeight = playerController.capsuleCollider.height;
        }
    }

    void Update()
    {
        if (S_UserInput.instance.CrouchInput && isAbleToCrouch())
        {
            OnCrouch();
        }
    }


    //!---------------- SI_DataPersistance ----------------

    //~ Sauvegarde de l'état du joueur (accroupi ou non)

    public void LoadData(S_GameData gameData)
    {
        isCrouching = gameData.isCrouching;

        // Appliquer l'état de crouch seulement si nécessaire
        if (isCrouching && playerController != null && playerController.capsuleCollider != null)
        {
            // Appliquer directement sans appeler OnCrouch() pour éviter les vérifications isGrounded
            playerController.capsuleCollider.height = crouchHeight;
            transform.localScale = new Vector3(1, crouchHeight, 1);
            
            if (playerController.overheadCheck != null)
            {
                playerController.overheadCheck.SetActive(true);
            }
        }
    }

    public void SaveData(S_GameData gameData)
    {
        gameData.isCrouching = isCrouching;
    }

    //! --------------- Fonctions privés ---------------

    private void OnCrouch() //& Gestion de l'accroupissement
    {
        if (playerController == null) return;

        //  Se lever
        if (playerController.isGrounded() && isCrouching && canRaise())
        {
            playerController.capsuleCollider.height = originalHeight;
            transform.localScale = new Vector3(1, 1, 1);
            isCrouching = false;
            playerController.overheadCheck.SetActive(false);
        } 
        // S'accroupir 
        else if (playerController.isGrounded())
        {
            playerController.capsuleCollider.height = crouchHeight;
            transform.localScale = new Vector3(1, crouchHeight, 1);
            playerController.playerRigidbody.AddForce(Vector3.down * 10f, ForceMode.Impulse);
            isCrouching = true;
            playerController.overheadCheck.SetActive(true);
        }
    }

    public bool canRaise() //& Vérifie si le joueur peut se relever
    {
        if (playerController == null || playerController.overheadCheck == null || playerController.colliderOverhead == null)
            return false;

        Vector3 checkPos = playerController.overheadCheck.transform.position;
        float radius = playerController.colliderOverhead.radius * playerController.overheadCheck.transform.lossyScale.x;

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
