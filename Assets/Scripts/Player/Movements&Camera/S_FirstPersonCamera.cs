using UnityEngine;

public class S_FirstPersonCamera : MonoBehaviour, SI_DataPersistance
{
    //~ Gestion de la camera
    [Header("Gestion de la caméra")]
    [SerializeField] private Transform player;
    private Camera playerCamera;
    private Rigidbody playerRigidbody; 

    private float limitYup = 90f;
    private float limitYdown = -90f;
    
    [Header("Limites horizontales")]
    private bool limitHorizontalRotation = false;
    private float limitXLeft = -90f;
    private float limitXRight = 90f;
    private float basePlayerRotation = 0f;
    
    private Vector2 lookValue = Vector2.zero;

    private float cameraVerticalRotation = 0f;
    private float playerHorizontalRotation = 0f;
    private bool isRotationActive = true;

    void Start()
    {
        playerCamera = GetComponent<Camera>();
        playerRigidbody = player.GetComponent<Rigidbody>(); 

        UpdateFieldOfView();
        setCursorEnabled(false);

        S_CameraUserData.instance.OnFieldOfViewChanged += UpdateFieldOfView;
        
        if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
        {
            S_GameManager.instance.playerEvents.onLockPlayerCamera += OnLockPlayerCamera;
        }
        
        playerHorizontalRotation = player.localEulerAngles.y;
        if (playerHorizontalRotation > 180f)
        {
            playerHorizontalRotation -= 360f;
        }
    }

    private void OnDestroy()
    {
        if (S_CameraUserData.instance != null)
        {
            S_CameraUserData.instance.OnFieldOfViewChanged -= UpdateFieldOfView;
        }

        if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
        {
            S_GameManager.instance.playerEvents.onLockPlayerCamera -= OnLockPlayerCamera;
        }
    }

    void LateUpdate()
    {
        Rotate();
    }

    public void LoadData(S_GameData gameData)
    {
        transform.localEulerAngles = gameData.cameraRotation;
        
        cameraVerticalRotation = gameData.cameraRotation.x;
        
        if (cameraVerticalRotation > 180f)
        {
            cameraVerticalRotation -= 360f;
        }
        
        playerHorizontalRotation = player.localEulerAngles.y;
        if (playerHorizontalRotation > 180f)
        {
            playerHorizontalRotation -= 360f;
        }
    }

    public void SaveData(S_GameData gameData)
    {
        gameData.cameraRotation = transform.localEulerAngles;
    }

    private void Rotate()
    {
        if (!canRotateCamera())
        {
            return;
        }

        if (!S_UserInput.instance.isUsingController())
        {
            lookValue = S_UserInput.instance.LookInput * (S_CameraUserData.instance.currentSensibilityMouse / 10);
        }
        else
        {
            lookValue = S_UserInput.instance.LookInput * S_CameraUserData.instance.currentSensibilityController;
        }

        if (S_CameraUserData.instance.currentInverseXAxis) lookValue.x *= -1f;
        if (S_CameraUserData.instance.currentInverseYAxis) lookValue.y *= -1f;
        
        lookValue *= Time.deltaTime;

        // Rotation vertical (caméra seulement)
        cameraVerticalRotation -= lookValue.y;
        cameraVerticalRotation = Mathf.Clamp(cameraVerticalRotation, limitYdown, limitYup);
        transform.localEulerAngles = Vector3.right * cameraVerticalRotation;

        if (limitHorizontalRotation)
        {
            playerHorizontalRotation += lookValue.x;
            playerHorizontalRotation = Mathf.Clamp(playerHorizontalRotation, limitXLeft, limitXRight);
            
            // Utiliser MoveRotation au lieu de localEulerAngles
            Quaternion targetRotation = Quaternion.Euler(0f, basePlayerRotation + playerHorizontalRotation, 0f);
            playerRigidbody.MoveRotation(targetRotation);
        }
        else
        {
            Quaternion deltaRotation = Quaternion.Euler(Vector3.up * lookValue.x);
            playerRigidbody.MoveRotation(playerRigidbody.rotation * deltaRotation);
        }
    }

    public void setCursorEnabled(bool isEnabled)
    {
        if (isEnabled)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void OnLockPlayerCamera(bool locked)
    {
        setRotationEnabled(!locked ? true : false);
        setCursorEnabled(locked);
    }

    public bool canRotateCamera()
    {
        return isRotationActive;
    }

    public void setRotationEnabled(bool isEnabled)
    {
        isRotationActive = isEnabled;
    }

    public void setHorizontalLimitEnabled(bool isEnabled)
    {
        limitHorizontalRotation = isEnabled;
        
        if (isEnabled)
        {
            basePlayerRotation = player.localEulerAngles.y;
            playerHorizontalRotation = 0f;
        }
    }

    private void UpdateFieldOfView()
    {
        playerCamera.fieldOfView = S_CameraUserData.instance.currentFieldOfView;
    }
}
