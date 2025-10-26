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

        DisableCursor();
    }

    void Update() //& PAS PHYSICS
    {
        if (canRotateCamera())
        {
            Rotate();
        }
    }

    //! --------------- Fonctions privés ---------------

    private void Rotate() //& Gère la rotation de la camera et du joueur
    {
        lookValue = lookAction.ReadValue<Vector2>() * mouseSensitivity;

        // Rotation vertical
        cameraVerticalRotation -= lookValue.y;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, limitYdown, limitYup);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        // Rotation horizontal
        player.Rotate(Vector3.up * lookValue.x);
    }

    //? ------------------------------------------------    

    public void EnableCursor() //& Unlock le curseur et l'affiche
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void DisableCursor() //& Lock le curseur et le cache
    {
        // Pour que le camera sois lock et ne bouge plus
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool canRotateCamera() //& A le droit de rotate la camera
    {
        return isRotationActive;
    }

    public void EnableRotation() //& Active la rotation
    {
        isRotationActive = true;
    }
    
    public void DisableRotation() //& Désactive la rotation
    {
        isRotationActive = false;
    }
    
}
