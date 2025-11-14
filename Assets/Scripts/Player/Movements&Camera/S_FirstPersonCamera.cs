using UnityEngine;

public class S_FirstPersonCamera : MonoBehaviour
{
    //~ Gestion de la camera
    [Header("Gestion de la caméra")]
    [SerializeField] private Transform player;
    private Camera playerCamera;
    private float limitYup = 90f; //Limite quand on regarde en haut
    private float limitYdown = -90f; //Limite quand on regarde en bas
    
    private Vector2 lookValue = Vector2.zero;

    private float cameraVerticalRotation = 0f;
    private bool isRotationActive = true;

    void Start() //& INITIALISATION VARIABLES
    {
        playerCamera = GetComponent<Camera>();

        UpdateFieldOfView();
        setCursorEnabled(false);

        S_CameraSettingsData.instance.OnFieldOfViewChanged += UpdateFieldOfView; // Lance cet fonction à chaque fois que le FOV change
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
        if (!S_UserInput.instance.isUsingController()) // Clavier & Souris
        {
            lookValue = S_UserInput.instance.LookInput * (S_CameraSettingsData.instance.currentSensibilityMouse / 10); // divise par 100 (car plus précis pour régler)
        }
        else // Manettes
        {
            lookValue = S_UserInput.instance.LookInput * S_CameraSettingsData.instance.currentSensibilityController; // divise par 100 (car plus précis pour régler)
        }

        // Inversion de X,Y
        if (S_CameraSettingsData.instance.currentInverseXAxis) lookValue.x *= -1f;
        if (S_CameraSettingsData.instance.currentInverseYAxis) lookValue.y *= -1f;
        
        lookValue *= Time.deltaTime; // Pour que la sensibilité s'ajuste au framerate

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


    //? ------------------------------------------------

    private void UpdateFieldOfView()
    {
        playerCamera.fieldOfView = S_CameraSettingsData.instance.currentFieldOfView;
    }
    
}
