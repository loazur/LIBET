using UnityEngine;
using UnityEngine.InputSystem;

public class S_FirstPersonCamera : MonoBehaviour
{
    //~ Gestion de la camera
    [Header("Gestion de la caméra")]
    public Transform player;
    private InputAction lookAction;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float limitYup, limitYdown; // Limit quand on regarde en haut et en bas

    //~ Privées
    private Vector2 lookValue = Vector2.zero;
    private float cameraVerticalRotation = 0f;

    private bool isRotationActive = true;

    void Start() //& INITIALISATION VARIABLES
    {
        lookAction = InputSystem.actions.FindAction("Look");

        setCursorEnabled(false);
    }

    void Update() //& PAS PHYSICS
    {
        Rotate();
    }

    //! --------------- Fonctions privés ---------------

    private void Rotate() //& Gère la rotation de la camera et du joueur
    {
        if (!canRotateCamera()) // Si désactivé
        {
            return;
        }

        lookValue = lookAction.ReadValue<Vector2>() * mouseSensitivity;

        // Rotation vertical
        cameraVerticalRotation -= lookValue.y;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, limitYdown, limitYup);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        // Rotation horizontal
        player.Rotate(Vector3.up * lookValue.x);
    }

    //? ------------------------------------------------    

    public void setCursorEnabled(bool isEnabled) //& Affiche/Enleve le curseur (ou le lock)
    {
        if (isEnabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // Pour que le camera sois lock et ne bouge plus
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public bool canRotateCamera() //& A le droit de rotate la camera
    {
        return isRotationActive;
    }

    public void setRotationEnabled(bool isEnabled) //& Active/Désactive la rotation
    {
        isRotationActive = isEnabled;
    }
    
}
