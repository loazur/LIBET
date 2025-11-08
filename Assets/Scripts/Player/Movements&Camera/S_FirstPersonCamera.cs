using UnityEngine;
using UnityEngine.InputSystem;

public class S_FirstPersonCamera : MonoBehaviour
{
    //~ Gestion de la camera
    [Header("Gestion de la caméra")]
    [SerializeField] private S_ControllerChecker controllerChecker;
    [SerializeField] private Transform player;
    [SerializeField] private float mouseSensitivityMouse = 100f; // Sensibilité Souris
    [SerializeField] private float mouseSensitivityController = 900f; // Sensibilité Manette
    private float limitYup = 90f; //Limite quand on regarde en haut
    private float limitYdown = -90f; //Limite quand on regarde en bas
    
    private InputAction lookAction;
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

        // Ajuste la vitesse de la camera en fonction du controller utilisé
        if (!controllerChecker.isUsingController()) // Souris
        {
            lookValue = lookAction.ReadValue<Vector2>() * (mouseSensitivityMouse / 1000); // divise par 1000 (car plus précis pour régler)
        }
        else // Manettes
        {
            lookValue = lookAction.ReadValue<Vector2>() * (mouseSensitivityController / 1000); // divise par 1000 (car plus précis pour régler)
        }

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
