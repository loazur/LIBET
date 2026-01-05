using UnityEngine;

public class S_FirstPersonCamera : MonoBehaviour, SI_DataPersistance
{
    //~ Gestion de la camera
    [Header("Gestion de la caméra")]
    [SerializeField] private Transform player;
    private Camera playerCamera;

    private float limitYup = 90f; //Limite quand on regarde en haut
    private float limitYdown = -90f; //Limite quand on regarde en bas
    
    [Header("Limites horizontales")]  // UTILISER POUR LES CHAISES
    [SerializeField] private bool limitHorizontalRotation = false; // Activer/désactiver la limite
    [SerializeField] private float limitXLeft = -40f; // Limite à gauche
    [SerializeField] private float limitXRight = 160f; // Limite à droite
    
    private Vector2 lookValue = Vector2.zero;

    private float cameraVerticalRotation = 0f;
    private float playerHorizontalRotation = 0f; // Nouvelle variable pour tracker la rotation horizontale
    private bool isRotationActive = true;

    void Start() //& INITIALISATION VARIABLES
    {
        playerCamera = GetComponent<Camera>();

        UpdateFieldOfView();
        setCursorEnabled(false);

        S_CameraUserData.instance.OnFieldOfViewChanged += UpdateFieldOfView; // Lance cet fonction à chaque fois que le FOV change
        
        // Initialiser la rotation horizontale
        playerHorizontalRotation = player.localEulerAngles.y;
        if (playerHorizontalRotation > 180f)
        {
            playerHorizontalRotation -= 360f;
        }
    }

    void Update() //& PAS PHYSICS
    {
        Rotate();
    }

     //!---------------- SI_DataPersistance ----------------

    //~ Sauvegarde rotation de la camera

    public void LoadData(S_GameData gameData)
    {
        // Charger la rotation locale de la caméra (ce script est sur la caméra)
        transform.localEulerAngles = gameData.cameraRotation;
        
        // Mettre à jour cameraVerticalRotation pour que Rotate() fonctionne correctement
        cameraVerticalRotation = gameData.cameraRotation.x;
        
        // Corriger les valeurs > 180 (Unity représente -90 comme 270)
        if (cameraVerticalRotation > 180f)
        {
            cameraVerticalRotation -= 360f;
        }
        
        // Charger la rotation horizontale du joueur
        playerHorizontalRotation = player.localEulerAngles.y;
        if (playerHorizontalRotation > 180f)
        {
            playerHorizontalRotation -= 360f;
        }
    }

    public void SaveData(S_GameData gameData)
    {
        // Sauvegarder la rotation locale de la caméra (ce script est sur la caméra)
        gameData.cameraRotation = transform.localEulerAngles;
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
            lookValue = S_UserInput.instance.LookInput * (S_CameraUserData.instance.currentSensibilityMouse / 10); // divise par 100 (car plus précis pour régler)
        }
        else // Manettes
        {
            lookValue = S_UserInput.instance.LookInput * S_CameraUserData.instance.currentSensibilityController; // divise par 100 (car plus précis pour régler)
        }

        // Inversion de X,Y
        if (S_CameraUserData.instance.currentInverseXAxis) lookValue.x *= -1f;
        if (S_CameraUserData.instance.currentInverseYAxis) lookValue.y *= -1f;
        
        lookValue *= Time.deltaTime; // Pour que la sensibilité s'ajuste au framerate

        // Rotation vertical
        cameraVerticalRotation -= lookValue.y;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, limitYdown, limitYup);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        // Rotation horizontal
        if (limitHorizontalRotation)
        {
            playerHorizontalRotation += lookValue.x;
            playerHorizontalRotation = Mathf.Clamp(playerHorizontalRotation, limitXLeft, limitXRight);
            player.localEulerAngles = Vector3.up * playerHorizontalRotation;
        }
        else
        {
            player.Rotate(Vector3.up * lookValue.x);
        }
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

    public void setHorizontalLimitEnabled(bool isEnabled) //& Active/Désactive la limite horizontale
    {
        limitHorizontalRotation = isEnabled;
    }

    //? ------------------------------------------------

    private void UpdateFieldOfView()
    {
        playerCamera.fieldOfView = S_CameraUserData.instance.currentFieldOfView;
    }
    
}
