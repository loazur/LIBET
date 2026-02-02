using UnityEngine;

public class S_WatchInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de la montre
    [Header("Gestion de la montre")]
    [SerializeField] private float maxDistance = 3f; // Distance maximale avant désactivation auto
    [SerializeField] private string interactText = "not_set"; // Texte à afficher

    //~ Gestion de l'UI
    [Header("UI")]
    [SerializeField] private GameObject watchUI;
    [SerializeField] private RectTransform secondsHand;
    const float degreesPerFullDay = 360f; // Un tour complet = 360°

    private bool isUsed = false;
    private Transform playerTransform; // Référence au joueur

    void Start() //& Initialize la montre
    {
        UpdateInteractText(); // Setup
        
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    void Update() //& Gère la mise à jour de l'affichage
    {
        if (isUsed)
        {
            UpdateClock();
            CheckPlayerDistance();
        }
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        if (!isUsed) // Activer la montre
        {
            this.playerTransform = playerTransform; // Sauvegarder la référence au joueur
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
        watchUI.SetActive(true); // Affichage de l'UI

    }

    private void DisableWatch() //& Désactive la montre
    {
        if (!isUsed) return;

        isUsed = false;
        watchUI.SetActive(false);
        playerTransform = null; // Réinitialiser la référence
        
        UpdateInteractText(); // Mettre à jour le texte d'interaction
    }

    private void CheckPlayerDistance() //& Vérifie la distance avec le joueur
    {
        if (playerTransform == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance > maxDistance) // Trop éloigné
        {
            DisableWatch();
        }
    }

    private void UpdateClock()
    {
        //TODO Changer en fonction du temps du S_DayNight
        // Calculer la rotation : 0% = 0°, 100% = 360°
        float rotation = S_DaysManager.instance.GetDayProgress() * degreesPerFullDay;
        
        secondsHand.rotation = Quaternion.Euler(0, 0, -rotation); // Négatif pour tourner dans le sens horaire
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
