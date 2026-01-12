using UnityEngine;

public class S_BedInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de la montre
    [Header("Gestion de la montre")]
    [SerializeField] private string interactText = "not_set"; // Texte à afficher

    void Start() //& Initialize la montre
    {
        UpdateInteractText(); // Setup
        
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        if (S_DaysManager.instance.AreQuestsDone())
        {
            Debug.Log("Les quetes sont terminées, on dort.");
            S_DaysManager.instance.EndDay();
        }
        else
        {
            Debug.Log("Les quetes n'ont pas été terminées, je ne peux pas dormir");
        }
    }

    public string getInteractText() => interactText; //& Texte affiché sur l'UI
    public Transform getTransform() => transform; //& Position de la montre
    
    //!---------------------------------------------

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (S_DaysManager.instance.AreQuestsDone()) // Les quetes sont terminées
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                interactText = "Dormir";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                interactText = "Sleep";
            }
        }
        else // Les quetes n'ont pas été terminées
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                interactText = "Incapable de dormir";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                interactText = "Unable to sleep";
            }
        }
        
    }
}
