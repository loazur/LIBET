using UnityEngine;
using TMPro;

public class S_WatchInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de la montre
    [Header("Gestion de la montre")]
    [SerializeField] private float maxDistance = 3f; // Distance maximale avant désactivation auto
    [SerializeField] private string interactText = "not_set"; // Texte à afficher

    //~ Gestion de l'UI
    [Header("UI")]
    [SerializeField] private GameObject watchUI;
    [SerializeField] private RectTransform secondsHand; // Aiguille des secondes
    [SerializeField] private TextMeshProUGUI timeText; // Texte optionnel pour afficher l'heure digitale

    private bool isUsed = false;
    private Transform playerTransform; // Référence au joueur

    void Start() //& Initialize la montre
    {
        UpdateInteractText(); // Setup
        
        if (S_GameUserData.instance != null)
        {
            S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
        }

        // S'assurer que l'UI est désactivée au démarrage
        if (watchUI != null)
        {
            watchUI.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (S_GameUserData.instance != null)
        {
            S_GameUserData.instance.OnLanguageChanged -= UpdateInteractText;
        }
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
        
        if (watchUI != null)
        {
            watchUI.SetActive(true); // Affichage de l'UI
        }
    }

    private void DisableWatch() //& Désactive la montre
    {
        if (!isUsed) return;

        isUsed = false;
        
        if (watchUI != null)
        {
            watchUI.SetActive(false);
        }
        
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
        if (S_DayNightManager.instance == null) return;

        // Récupérer les valeurs du DayNightManager
        float currentTime = S_DayNightManager.instance.timeLasted;
        float timeStart = S_DayNightManager.instance.GetTimeStart();
        float timeEnd = S_DayNightManager.instance.GetTimeEnd();

        // Calculer la progression normalisée entre timeStart et timeEnd (0 à 1)
        float normalizedProgress = Mathf.InverseLerp(timeStart, timeEnd, currentTime);
        
        // Clamper pour éviter les valeurs hors limites
        normalizedProgress = Mathf.Clamp01(normalizedProgress);

        // Calculer la rotation de l'aiguille (360° = un tour complet)
        float rotation = normalizedProgress * 360f;
        
        // Appliquer la rotation (négatif pour tourner dans le sens horaire)
        if (secondsHand != null)
        {
            secondsHand.rotation = Quaternion.Euler(0, 0, -rotation);
        }

        // Mettre à jour le texte digital (optionnel)
        if (timeText != null)
        {
            timeText.text = S_DayNightManager.instance.GetCurrentTimeString();
        }
    }

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (S_GameUserData.instance == null) return;

        if (!isUsed)
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                interactText = "Regarder l'heure";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                interactText = "Check time";
            }
        }
        else
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                interactText = "Fermer";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                interactText = "Close";
            }
        }
    }
}
