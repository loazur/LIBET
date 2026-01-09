using System.Collections;
using UnityEngine;

public class S_PlayerLevelManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private int startingLevel = 1;
    [SerializeField] private int startingExperience = 0;

    private int currentLevel;
    private int currentExperience;
    private bool isSubscribed = false;

    private void Awake()
    {
        currentLevel = startingLevel;
        currentExperience = startingExperience;
    }

    private void Start()
    {
        StartCoroutine(InitializeWhenReady());
    }

    private IEnumerator InitializeWhenReady()
    {
        // Attendre que S_GameManager soit initialisé
        while (S_GameManager.instance == null)
        {
            yield return null;
        }

        // S'abonner aux événements
        SubscribeToEvents();

        // Notifier le niveau et l'expérience initiaux
        S_GameManager.instance.playerEvents.PlayerLevelChange(currentLevel);
        S_GameManager.instance.playerEvents.PlayerExperienceChange(currentExperience);
    }

    private void SubscribeToEvents()
    {
        if (S_GameManager.instance == null)
        {
            Debug.LogError("[PlayerLevelManager] Impossible de s'abonner : S_GameManager est null !");
            return;
        }
        
        if (isSubscribed)
        {
            Debug.LogWarning("[PlayerLevelManager] Déjà abonné aux événements");
            return;
        }

        S_GameManager.instance.playerEvents.onExperienceGained += ExperienceGained;
        isSubscribed = true;
        // Debug.Log("<color=green>[PlayerLevelManager]</color> Abonnement à l'événement ExperienceGained réussi !");
    }

    private void UnsubscribeFromEvents()
    {
        if (S_GameManager.instance == null || !isSubscribed) return;

        S_GameManager.instance.playerEvents.onExperienceGained -= ExperienceGained;
        isSubscribed = false;
    }

    private void OnDisable() 
    {
        UnsubscribeFromEvents();
    }

    private void ExperienceGained(int experience) 
    {
        // Debug.Log($"<color=cyan>[PlayerLevelManager]</color> Réception de {experience} XP | Niveau actuel: {currentLevel} | XP actuel: {currentExperience}/{S_GlobalConstants.experienceToLevelUp}");
        
        currentExperience += experience;
        
        // Debug.Log($"<color=cyan>[PlayerLevelManager]</color> Après ajout: {currentExperience}/{S_GlobalConstants.experienceToLevelUp} XP");
        
        // check if we're ready to level up
        int levelsGained = 0;
        while (currentExperience >= S_GlobalConstants.experienceToLevelUp) 
        {
            currentExperience -= S_GlobalConstants.experienceToLevelUp;
            currentLevel++;
            levelsGained++;
            S_GameManager.instance.playerEvents.PlayerLevelChange(currentLevel);
            // Debug.Log($"<color=green>[PlayerLevelManager]</color> NIVEAU SUPÉRIEUR ! Nouveau niveau: {currentLevel} | XP restant: {currentExperience}");
        }
        
        if (levelsGained == 0)
        {
            // Debug.Log($"<color=yellow>[PlayerLevelManager]</color> Pas assez d'XP pour monter de niveau (besoin de {S_GlobalConstants.experienceToLevelUp - currentExperience} XP supplémentaires)");
        }
        
        S_GameManager.instance.playerEvents.PlayerExperienceChange(currentExperience);
    }
}
