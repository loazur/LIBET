using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLevelManager : MonoBehaviour
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
        if (S_GameManager.instance == null || isSubscribed) return;

        S_GameManager.instance.playerEvents.onExperienceGained += ExperienceGained;
        isSubscribed = true;
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
        currentExperience += experience;
        // check if we're ready to level up
        while (currentExperience >= GlobalConstants.experienceToLevelUp) 
        {
            currentExperience -= GlobalConstants.experienceToLevelUp;
            currentLevel++;
            S_GameManager.instance.playerEvents.PlayerLevelChange(currentLevel);
        }
        S_GameManager.instance.playerEvents.PlayerExperienceChange(currentExperience);
    }
}
