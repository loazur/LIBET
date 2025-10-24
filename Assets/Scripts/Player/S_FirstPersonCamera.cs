using UnityEngine;
using UnityEngine.InputSystem;

public class S_FirstPersonCamera : MonoBehaviour
{
    //~ Gestion de la camera
    public Transform player;
    private InputAction lookAction;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float limitYup, limitYdown; // Limit quand on regarde en haut et en bas

    //~ Privées
    private Vector2 lookValue = Vector2.zero;
    private float cameraVerticalRotation = 0f;


    void Start()
    {
        lookAction = InputSystem.actions.FindAction("Look");   
    }

    void Update()
    {
        Rotate();
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
}
