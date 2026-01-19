using UnityEngine;

public class S_RepetitionTest : MonoBehaviour, SI_Interactable
{   
    //~ Gestion de l'affichage de l'UI du cadenas
    [Header("Gestion du minijeu")]
    [SerializeField] private string interactText = "not_set"; // Texte à afficher
    [SerializeField] private S_MinijeuRepetition minigame;

    void Start() //& Initialize la montre
    {
        UpdateInteractText(); // Setup
        
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        // Lance le mini-jeu
        minigame.StartMinigame(OnMinigameComplete);
    }

    public string getInteractText() => interactText; //& Texte affiché sur l'UI
    public Transform getTransform() => transform; //& Position du cadenas
    
    //!---------------------------------------------
    
    private void OnMinigameComplete(bool success)
    {
        if (success)
        {
            Destroy(gameObject);
            Debug.Log("<color=green>Ré&ussi !</color>");
        }
        else
        {
            // Échec
            Debug.Log("<color=red>Raté.</color>");
        }
    }
    

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            interactText = "Jouer minijeu";
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            interactText = "Play minigame";
        }
    }
}
