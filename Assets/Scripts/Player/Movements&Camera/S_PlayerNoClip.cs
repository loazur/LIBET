using UnityEngine;
using UnityEngine.InputSystem;

public class S_PlayerNoClip : MonoBehaviour
{
    //~ Gestion NoClip
    [Header("Gestion No Clip")]
    private S_PlayerController playerController;
    [SerializeField] private float noClipSpeed = 10f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [HideInInspector] public bool isNoClipping = false;

    void Start() //& INITIALISATION VARIABLES
    {
        playerController = GetComponent<S_PlayerController>();
    }

    void Update()  //& PAS PHYSICS
    {
        if (S_UserInput.instance.NoClipInput)
        {
            if (!isNoClipping)
            {
                setNoClipEnabled(true);
            }
            else
            {
                setNoClipEnabled(false);
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
        // avant/arrière et gauche/droite selon la direction du regard
        Vector3 move = transform.forward * S_UserInput.instance.MoveInput.y + transform.right * S_UserInput.instance.MoveInput.x;

        // Montée / Descente
        float vertical = 0f;
        if (S_UserInput.instance.FlyUpInput) vertical += 1f;
        if (S_UserInput.instance.FlyDownInput) vertical -= 1f;

        move += transform.up * vertical;

        if (!S_UserInput.instance.SprintInput) // Non Sprint
        {
            transform.position += move.normalized * noClipSpeed * Time.fixedDeltaTime;
        }
        else // Sprint
        {
            transform.position += move.normalized * noClipSpeed * sprintMultiplier * Time.fixedDeltaTime;
        }
    }

    //? ------------------------------------------------    

    private void setNoClipEnabled(bool isEnabled) //& Activation/Désactivation No Clip
    {
        if (isEnabled)
        {
            isNoClipping = true;

            playerController.playerRigidbody.isKinematic = true; // Désactive collisions
            playerController.playerRigidbody.useGravity = false; // Désactive gravité
        }
        else
        {
            isNoClipping = false;

            playerController.playerRigidbody.isKinematic = false;
            playerController.playerRigidbody.useGravity = true;
        }
    }
}
