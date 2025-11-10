using UnityEngine;

public class S_FirstPersonCamera : MonoBehaviour
{
    //~ Gestion de la camera
    [Header("Gestion de la caméra")]
    [SerializeField] private Transform player;
    [SerializeField] private float sensitivityMouse = 100f; // Sensibilité Souris
    [SerializeField] private float sensitivityController = 1600f; // Sensibilité Manette
    private float limitYup = 90f; //Limite quand on regarde en haut
    private float limitYdown = -90f; //Limite quand on regarde en bas
    
    private Vector2 lookValue = Vector2.zero;

    private float cameraVerticalRotation = 0f;
    private bool isRotationActive = true;

    void Start() //& INITIALISATION VARIABLES
    {
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
        if (!S_UserInput.instance.isUsingController()) // Clavier & Souris
        {
            lookValue = S_UserInput.instance.LookInput * (sensitivityMouse / 10); // divise par 100 (car plus précis pour régler)
        }
        else // Manettes
        {
            lookValue = S_UserInput.instance.LookInput * (sensitivityController / 10); // divise par 100 (car plus précis pour régler)
        }

        lookValue *= Time.deltaTime; // Pour que la sensibilité s'ajuste au framerate

        // Rotation vertical
        cameraVerticalRotation -= lookValue.y;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, limitYdown, limitYup);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        // Rotation horizontal
        player.Rotate(Vector3.up * lookValue.x);
    }

    public void changeMouseSensitivity(float value)
    {
        sensitivityMouse = value;
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
