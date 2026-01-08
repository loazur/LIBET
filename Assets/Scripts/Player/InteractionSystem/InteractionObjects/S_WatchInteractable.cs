using UnityEngine;

public class S_WatchInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de la montre
    [SerializeField] private string interactText = "not_set"; // Texte à afficher
    private bool isUsed = false;

    //TODO Faire l'UI, et désactivé la montre si trop éloigné

    void Start() //& Initialize la montre
    {
        UpdateInteractText(); // Setup
        
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        if (!isUsed) // Activer la montre
        {
            EnableWatch();
        }
        else // Désactiver la montre
        {
            DisableWatch();
        }

        UpdateInteractText();
    }

    public string getInteractText() => interactText; //& Texte affiché sur l'UI
    public Transform getTransform() => transform; //& Position de la montre
    

    //!---------------------------------------------

    private void EnableWatch() //& Active la montre
    {
        if (isUsed) return;

        isUsed = true;

        //TODO - Afficher UI
        Debug.Log($"{S_DaysManager.instance.GetDayProgress() * 100}% effectué du jour actuel");

        Debug.Log("Montre activé");
    }

    private void DisableWatch() //& Désactive la montre
    {
        if (!isUsed) return;

        isUsed = false;

        //TODO - Desactivé l'UI
        
        Debug.Log("Montre désactivé");
    }

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (!isUsed)
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                interactText = "Activer";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                interactText = "Enable";
            }
        }
        else
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                interactText = "Désactiver";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                interactText = "Disable";
            }
        }
    }
}
