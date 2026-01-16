using UnityEngine;

public class S_PadlockInteractable : MonoBehaviour, SI_Interactable
{
     //~ Gestion de l'affichage de l'UI du cadenas
    [Header("Gestion du cadenas")]
    [SerializeField] private GameObject displayPanelPadlock;
    [SerializeField] private string interactText = "not_set"; // Texte à afficher

    [SerializeField] private S_PlayerController playerController;
    [SerializeField] private S_FirstPersonCamera playerCamera;

    private S_PlayerCrouch playerCrouch;
    private S_PlayerInteract playerInteract;
    private S_PlayerFootsteps playerFootsteps;

    private bool shown = false;

    void Awake()
    {
        playerCrouch = playerController.GetComponent<S_PlayerCrouch>();
        playerInteract = playerController.GetComponent<S_PlayerInteract>();
        playerFootsteps = playerController.GetComponent<S_PlayerFootsteps>();
    }

    void Start() //& Initialize la montre
    {
        UpdateInteractText(); // Setup
        
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    void Update()
    {
        if (S_UserInput.instance.CancelInteractionAction.WasPressedThisFrame() && shown)
        {
            Hide();
        }
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
       if (!shown)
        {
            Show();
        }
    }

    public string getInteractText() => interactText; //& Texte affiché sur l'UI
    public Transform getTransform() => transform; //& Position du cadenas
    
    //!---------------------------------------------

    public void CorrectPassword() 
    {
        Hide();
        Destroy(gameObject);
    }

    private void Show()
    {
        shown = true;

        displayPanelPadlock.SetActive(true);

        playerController.setMovementsEnabled(false);
        playerCamera.setCursorEnabled(true);
        playerCamera.setRotationEnabled(false);
        playerInteract.setInteractionEnabled(false);
        playerCrouch.setAbleToCrouch(false);
        playerFootsteps.SetSoundsEnabled(false);
    }

    private void Hide()
    {
        shown = false;

        displayPanelPadlock.SetActive(false);

        playerController.setMovementsEnabled(true);
        playerCamera.setCursorEnabled(false);
        playerCamera.setRotationEnabled(true);
        playerInteract.setInteractionEnabled(true);
        playerCrouch.setAbleToCrouch(true);
        playerFootsteps.SetSoundsEnabled(true);
    }
    

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            interactText = "Afficher cadenas";
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            interactText = "Show padlock";
        }
    }

}
