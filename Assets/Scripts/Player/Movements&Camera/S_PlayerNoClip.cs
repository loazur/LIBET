using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerNoClip : MonoBehaviour
{
    //~ Gestion NoClip
    [Header("Gestion No Clip")]
    private S_PlayerController playerController;
    
    private InputAction noClipAction, sprintAction, flyUp, flyDown;
    public float noClipSpeed;
    public float sprintMultiplier;
    [HideInInspector] public bool isNoClipping = false;

    void Start() //& INITIALISATION VARIABLES
    {
        playerController = GetComponent<S_PlayerController>();
        
        noClipAction = InputSystem.actions.FindAction("NoClip");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        flyUp = InputSystem.actions.FindAction("flyUp");
        flyDown = InputSystem.actions.FindAction("FlyDown");
    }

    void Update()  //& PAS PHYSICS
    {
        if (noClipAction.WasPressedThisFrame())
        {
            if (!isNoClipping)
            {
                EnableNoClip();
            }
            else
            {
                DisableNoClip();
            }
        }
    }

    void FixedUpdate() //& PHYSICS
    {
        if (isNoClipping)
        {
            NoClipMovement();
        }
    }

    //! --------------- Fonctions privés ---------------

    private void NoClipMovement() //& Changement du système de mouvement
    {
        Vector2 moveInput = playerController.moveAction.ReadValue<Vector2>();

        // avant/arrière et gauche/droite selon la direction du regard
        Vector3 move = transform.forward * moveInput.y + transform.right * moveInput.x;

        // Montée / Descente
        float vertical = 0f;
        if (flyUp.IsPressed()) vertical += 1f;
        if (flyDown.IsPressed()) vertical -= 1f;

        move += transform.up * vertical;

        if (!sprintAction.IsPressed()) // Non Sprint
        {
            transform.position += move.normalized * noClipSpeed * Time.fixedDeltaTime;
        }
        else // Sprint
        {
            transform.position += move.normalized * noClipSpeed * sprintMultiplier * Time.fixedDeltaTime;
        }
    }

    //? ------------------------------------------------    

    private void EnableNoClip() //& Activation No Clip
    {
        isNoClipping = true;

        playerController.playerRigidbody.isKinematic = true; // Désactive collisions
        playerController.playerRigidbody.useGravity = false; // Désactive gravité
    }

    private void DisableNoClip() //& Désactivation No Clip
    {
        isNoClipping = false;

        playerController.playerRigidbody.isKinematic = false;
        playerController.playerRigidbody.useGravity = true;
    }
}
