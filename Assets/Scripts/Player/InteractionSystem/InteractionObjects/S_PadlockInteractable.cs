using UnityEngine;

public class S_PadlockInteractable : MonoBehaviour, SI_Interactable
{
     //~ Gestion de l'affichage de l'UI du cadenas
    [Header("Gestion du cadenas")]
    [SerializeField] private GameObject displayPanelPadlock;
    [SerializeField] private string interactText = "not_set"; // Texte à afficher

    private bool shown = false;


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
        if (S_MenuManager.instance != null)
        {
            if (!S_MenuManager.instance.RegisterMenuOpen(S_MenuManager.MenuType.PADLOCK))
            {
                Debug.LogWarning("[DialogueManager] Impossible de démarrer le menu cadenas, un menu est ouvert");
                return;
            }
        }

        shown = true;

        displayPanelPadlock.SetActive(true);
    }

    private void Hide()
    {
        if (S_MenuManager.instance != null)
        {
            S_MenuManager.instance.RegisterMenuClose(S_MenuManager.MenuType.PADLOCK);
        }
        shown = false;

        displayPanelPadlock.SetActive(false);
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
