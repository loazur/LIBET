using System.Collections;
using UnityEngine;

/**
 * Quête pour attendre que le jour 2 soit atteint.
 * S'abonne à l'événement OnDayEnd pour détecter la fin du jour 1.
 * Note: OnDayEnd est appelé AVANT que currentDay soit incrémenté,
 * donc on complète la quête quand le jour 1 se termine.
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v3.0.0	Wednesday, January 22nd, 2026.
 * @global
 */
public class S_WaitDay2Quest : S_QuestStep
{
    private bool isCompleted = false;
    private bool isSubscribed = false;

    private void Start()
    {
        StartCoroutine(InitializeWhenReady());
    }

    private IEnumerator InitializeWhenReady()
    {
        // Attendre que les managers ET que la quest step soient initialisés
        while (S_GameManager.instance == null || 
               S_DaysManager.instance == null || 
               !IsQuestStepInitialized())
        {
            yield return null;
        }

        Debug.Log("[S_WaitDay2Quest] Managers ready, checking current day");

        // Vérifier immédiatement si le jour 2 est déjà atteint
        if (S_DaysManager.instance.IsDay2Reached())
        {
            Debug.Log("[S_WaitDay2Quest] Day 2 already reached at initialization");
            CompleteQuest();
            yield break;
        }

        // Sinon, s'abonner à l'événement OnDayEnd pour détecter la fin du jour 1
        SubscribeToEvents();
        UpdateState();
    }

    private void SubscribeToEvents()
    {
        if (S_DaysManager.instance == null || isSubscribed) return;

        S_DaysManager.instance.OnDayEnd += OnDayEnd;
        isSubscribed = true;
        Debug.Log("[S_WaitDay2Quest] Subscribed to OnDayEnd event");
    }

    private void UnsubscribeFromEvents()
    {
        if (S_DaysManager.instance == null || !isSubscribed) return;

        S_DaysManager.instance.OnDayEnd -= OnDayEnd;
        isSubscribed = false;
        Debug.Log("[S_WaitDay2Quest] Unsubscribed from OnDayEnd event");
    }

    private void OnDisable()
    {
        UnsubscribeFromEvents();
    }

    /**
     * Callback appelé quand le jour se termine.
     * Note: OnDayEnd est invoqué AVANT que currentDay soit incrémenté dans PrepareNextDay().
     * Donc si on est au jour 1 et que le jour se termine, on va passer au jour 2.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v2.0.0	Wednesday, January 22nd, 2026.
     * @access	private
     * @return	void
     */
    private void OnDayEnd()
    {
        if (isCompleted) return;

        int currentDay = S_DaysManager.instance.GetCurrentDay();
        Debug.Log($"[S_WaitDay2Quest] Day {currentDay} ended - completing quest (will be day 2 next)");

        // La fin du jour 1 signifie qu'on va passer au jour 2
        // On complète donc la quête à ce moment
        CompleteQuest();
    }

    private void UpdateState()
    {
        if (!IsQuestStepInitialized()) return;
        
        string state = isCompleted ? "1/1" : "0/1";
        ChangeState(state, "Attendre le jour 2");
    }

    private void CompleteQuest()
    {
        if (isCompleted) return;

        isCompleted = true;
        UpdateState();
        
        UnsubscribeFromEvents();
        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        // Appelé lors du chargement d'une sauvegarde
        if (state == "1/1")
        {
            isCompleted = true;
        }
    }
}