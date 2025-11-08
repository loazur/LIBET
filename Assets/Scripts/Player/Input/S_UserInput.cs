using UnityEngine;
using UnityEngine.InputSystem;

public class S_UserInput : MonoBehaviour
{
    //~ Gestion des inputs du joueur
    public static S_UserInput instance;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public bool InteractInput { get; private set; }
    public InputAction CancelInteractionAction { get; private set; } // -> Spécial (utilisé en appuyant et maintenant le bouton)
    public bool CrouchInput { get; private set; }
    public bool SprintInput { get; private set; }

    // Actions
    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _interactAction;
    private InputAction _crouchAction;
    private InputAction _sprintAction;

    // ! Dévs
    public bool NoClipInput { get; private set; }
    public bool FlyUpInput { get; private set; }
    public bool FlyDownInput { get; private set; }

    private InputAction _noClipAction;
    private InputAction _flyUpAction;
    private InputAction _flyDownAction;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        _playerInput = GetComponent<PlayerInput>();

        SetupInputActions();
    }

    void Update()
    {
        UpdateInputs();
    }

    private void SetupInputActions()
    {
        _moveAction = _playerInput.actions["Move"];
        _lookAction = _playerInput.actions["Look"];
        _interactAction = _playerInput.actions["Interact"];
        CancelInteractionAction = _playerInput.actions["CancelInteraction"];
        _crouchAction = _playerInput.actions["Crouch"];
        _sprintAction = _playerInput.actions["Sprint"];

        //! Dévs
        _noClipAction = _playerInput.actions["NoClip"];
        _flyUpAction = _playerInput.actions["FlyUp"];
        _flyDownAction = _playerInput.actions["FlyDown"];
    }
    
    private void UpdateInputs()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        LookInput = _lookAction.ReadValue<Vector2>();
        InteractInput = _interactAction.WasPressedThisFrame();
        CrouchInput = _crouchAction.WasPressedThisFrame();
        SprintInput = _sprintAction.IsPressed();

        //! Dévs
        NoClipInput = _noClipAction.WasPressedThisFrame();
        FlyUpInput = _flyUpAction.IsPressed();
        FlyDownInput = _flyDownAction.IsPressed();
    }
}
