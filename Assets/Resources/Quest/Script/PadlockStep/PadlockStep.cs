using UnityEngine;
using System.Collections;

/**
 * Quête pour vérifier si un cadenas est déverrouillé.
 * Détecte quand le joueur déverrouille un cadenas via le système d'événements
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Thursday, January 23rd, 2026.
 * @global
 */
public class PadlockStep : S_QuestStep
{
    private bool hasUnlocked = false;
    private bool isSubscribed = false;

    // *==========================================================================

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

        Debug.Log("[PadlockStep] GameManager ready, subscribing to events");
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (S_GameManager.instance == null || isSubscribed) return;
        
        S_GameManager.instance.playerEvents.onPadlockUnlocked += OnPadlockUnlocked;
        isSubscribed = true;
        Debug.Log("[PadlockStep] Subscribed to onPadlockUnlocked event");
    }

    private void UnsubscribeFromEvents()
    {
        if (S_GameManager.instance == null || !isSubscribed) return;

        S_GameManager.instance.playerEvents.onPadlockUnlocked -= OnPadlockUnlocked;
        isSubscribed = false;
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    // *==========================================================================

    /**
     * Callback appelé quand un cadenas est déverrouillé
     */
    private void OnPadlockUnlocked()
    {
        if (hasUnlocked) return;

        hasUnlocked = true;
        Debug.Log("[PadlockStep] Cadenas déverrouillé! Quête complétée.");
        
        UnsubscribeFromEvents();
        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        
    }
}



