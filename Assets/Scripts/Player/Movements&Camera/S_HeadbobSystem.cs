using UnityEngine;

public class S_HeadbobSystem : MonoBehaviour
{
    [Header("Headbob Settings")]
    [Range(0.001f, 0.1f)]
    public float Amount = 0.002f;

    [Range(1f, 30f)]
    public float Frequency = 10f;

    [Range(10f, 100f)]
    public float Smooth = 10f;

    [Header("Movement Detection")]
    [SerializeField] private S_PlayerController playerController;
    [SerializeField] private float movementThreshold = 0.1f; // Seuil minimal pour déclencher le headbob

    private Vector3 startPos;
    private bool isHeadbobActive = false;

    //! Walk
    // 0.075f Amount
    // 8f     Frequency
    // 10     Smooth

    //! Run
    // 0.0805f   Amount
    // 12f       Frequency
    // 10        Smooth

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForHeadbobTrigger();

        if (isHeadbobActive)
        {
            ApplyHeadbob();
        }
        else
        {
            StopHeadbob();
        }
    }

    private void CheckForHeadbobTrigger()
    {
        // Vérifier si la rotation de la caméra est active
        S_FirstPersonCamera cameraScript = GetComponent<S_FirstPersonCamera>();
        bool cameraActive = cameraScript != null ? cameraScript.canRotateCamera() : true;
        
        // Utiliser MoveInput au lieu de LookInput
        Vector2 moveInput = S_UserInput.instance.MoveInput;
        float inputMagnitude = moveInput.magnitude;

        // Activer le headbob si le joueur se déplace ET que la caméra est active
        isHeadbobActive = inputMagnitude > movementThreshold && cameraActive;
    }

    private void ApplyHeadbob()
    {
        if (!playerController.canMove()) return; // Ne peut pas se déplacer

        if (S_UserInput.instance.SprintInput) // Courir
        {
            Amount = 0.0805f;
            Frequency = 12f;
            Smooth = 10;
        }
        else // Marcher
        {
            Amount = 0.075f;
            Frequency = 8f;
            Smooth = 10;
        }

        Vector3 pos = Vector3.zero;

        // Mouvement vertical (haut/bas)
        pos.y = Mathf.Sin(Time.time * Frequency) * Amount * 1.4f;

        // Mouvement horizontal (gauche/droite)
        pos.x = Mathf.Cos(Time.time * Frequency / 2f) * Amount * 1.6f;

        // Appliquer la position avec interpolation pour un mouvement fluide
        transform.localPosition = Vector3.Lerp(transform.localPosition, startPos + pos, Smooth * Time.deltaTime);
    }

    private void StopHeadbob()
    {
        // Retourner progressivement à la position de départ
        if (Vector3.Distance(transform.localPosition, startPos) > 0.001f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, Smooth * Time.deltaTime);
        }
        else
        {
            transform.localPosition = startPos;
        }
    }

    /// <summary>
    /// Réinitialise la position de la caméra (utile lors de téléportations)
    /// </summary>
    public void ResetPosition()
    {
        transform.localPosition = startPos;
    }
}
