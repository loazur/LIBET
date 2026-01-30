using UnityEngine;
using UnityEngine.InputSystem;

public class S_UserInput : MonoBehaviour
{
    //~ Gestion des inputs du joueur
    public static S_UserInput instance;

    public Vector2 MoveInput { get; private set; }
    public Vector2 LookInput { get; private set; }
    public InputAction InteractAction { get; private set; } // -> Publique (à besoin de récuperer la touche pour l'UI)
    public InputAction CancelInteractionAction { get; private set; } // -> Publique (utilisé en appuyant et maintenant le bouton)
    public bool SprintInput { get; private set; }
    public bool MenuOpenCloseInput { get; private set; }
    // Quest menu pressed this frame (true only on the frame the button was pressed)
    public bool QuestMenuInput { get; private set; } //& touche pour ouvrir/fermer le menu des quêtes
    public bool NotesMenuInput {get; private set;}

    // Actions
    private PlayerInput _playerInput;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _sprintAction;
    private InputAction _menuOpenCloseAction;
    private InputAction _questMenuAction;
    private InputAction _notesMenuAction;



    // ! Dévs
    public bool NoClipInput { get; private set; }
    public bool FlyUpInput { get; private set; }
    public bool FlyDownInput { get; private set; }

    private InputAction _noClipAction;
    private InputAction _flyUpAction;
    private InputAction _flyDownAction;


    void Awake() //& Initialisation du singleton
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _playerInput = GetComponent<PlayerInput>();

        SetupInputActions();
    }

    void Update() //& Met à jour les inputs
    {
        UpdateInputs();
    }

    //! --------------- Fonctions privés ---------------

    private void SetupInputActions()
    {
        _moveAction = _playerInput.actions["Move"];
        _lookAction = _playerInput.actions["Look"];
        InteractAction = _playerInput.actions["Interact"];
        CancelInteractionAction = _playerInput.actions["CancelInteraction"];
        _sprintAction = _playerInput.actions["Sprint"];
        _menuOpenCloseAction = _playerInput.actions["MenuOpenClose"];
        _questMenuAction = _playerInput.actions["QuestMenu"];
        _notesMenuAction = _playerInput.actions["NotesMenu"];

        //! Dévs
        _noClipAction = _playerInput.actions["NoClip"];
        _flyUpAction = _playerInput.actions["FlyUp"];
        _flyDownAction = _playerInput.actions["FlyDown"];
    }

    private void UpdateInputs()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();
        LookInput = _lookAction.ReadValue<Vector2>();
        SprintInput = _sprintAction.IsPressed();
        MenuOpenCloseInput = _menuOpenCloseAction.WasPressedThisFrame();
        QuestMenuInput = _questMenuAction.WasPressedThisFrame();
        NotesMenuInput = _notesMenuAction.WasPressedThisFrame();

        //! Dévs
        NoClipInput = _noClipAction.WasPressedThisFrame();
        FlyUpInput = _flyUpAction.IsPressed();
        FlyDownInput = _flyDownAction.IsPressed();
    }
    
    //? ------------------------------------------------
    public bool isUsingController() //& Si le joueur utilise une manette
    {
        return _playerInput.currentControlScheme == "Gamepad";
    }
}