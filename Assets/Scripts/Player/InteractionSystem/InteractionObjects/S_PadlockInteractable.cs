using UnityEditor.Search;
using UnityEngine;

public class S_PadlockInteractable : MonoBehaviour, SI_Interactable
{
     //~ Gestion de l'affichage de l'UI du cadenas
    [Header("Gestion du cadenas")]
    [SerializeField] private GameObject displayPanelPadlock;
    [SerializeField] private string interactText = "not_set"; // Texte à afficher

    private bool shown = false;

    [Header("Supprimer un objet après déverrouillage (optionnel)")]
    [SerializeField] private GameObject objectToDestroyOnUnlock;

    [Header("Option supplémentaire pour les quêtes")]
    [SerializeField] private bool isQuestPadlock = true; //& Indique si le cadenas est lié à une quête


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

        //TODO METTRE les truc quand le cadna est OK
        // Désactiver l'objet optionnel
        if (objectToDestroyOnUnlock != null)
        {
            objectToDestroyOnUnlock.SetActive(false);
        }

        // Envoyer event au piano pour dire qu'il est déverrouillé
        S_GameManager.instance.playerEvents.PadlockUnlocked();
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

        // Traduction texte d'aide
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            S_MenuManager.instance.EnableHelpingUI("Quitter le menu");
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            S_MenuManager.instance.EnableHelpingUI("Quit the menu");
        }
    }

    private void Hide()
    {
        if (S_MenuManager.instance != null)
        {
            S_MenuManager.instance.RegisterMenuClose(S_MenuManager.MenuType.PADLOCK);
        }
        shown = false;

        displayPanelPadlock.SetActive(false);

        // Fermeture UI d'aide
        S_MenuManager.instance.DisableHelpingUI();
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
