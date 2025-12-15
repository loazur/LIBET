using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_AlzheimerEventsManager : MonoBehaviour
{
    [Header("Gestion des events du jeu")]
    [Tooltip("Liste des events activable")]
    [SerializeField] public List<SO_AlzheimerEvent> alzheimerEvents = new List<SO_AlzheimerEvent>();

    [Tooltip("Tout les combiens de temps un event s'active en secondes")]
    [SerializeField] private float activationInterval = 180f;

    [Tooltip("Si la boucle d'event aléatoire est active")]
    [SerializeField] private bool eventLoopActive = true;
    
    [Tooltip("Nombre maximum d'events actifs en même temps")]
    [SerializeField, Min(1)] private int maxActiveEvents = 3;

    private Coroutine eventLoopCoroutine;
    
    // Liste des events actuellement actifs (prefab instancié + SO associé)
    private List<ActiveEventData> activeEvents = new List<ActiveEventData>();
    
    // Structure pour tracker un event actif
    [System.Serializable]
    public class ActiveEventData
    {
        public SO_AlzheimerEvent eventData;
        public GameObject instance;
        public float startTime;
        
        public ActiveEventData(SO_AlzheimerEvent data, GameObject obj)
        {
            eventData = data;
            instance = obj;
            startTime = Time.time;
        }
    }
    
    // Propriété publique pour savoir combien d'events sont actifs
    public int ActiveEventsCount => activeEvents.Count;
    public List<ActiveEventData> ActiveEvents => activeEvents;

    void Start()
    {   
        Debug.Log($"[S_AlzheimerEventsManager] Start - Events: {alzheimerEvents.Count}, Interval: {activationInterval}s");
        
        if (eventLoopActive)
            StartEventLoop();
    }
    
    void Update()
    {
        // Nettoyer les events détruits automatiquement
        CleanupDestroyedEvents();
    }

    #region Event Loop Control
    
    /**
     * Démarre la boucle d'events
     */
    public void StartEventLoop()
    {
        if (eventLoopCoroutine != null)
            StopCoroutine(eventLoopCoroutine);
            
        eventLoopActive = true;
        eventLoopCoroutine = StartCoroutine(EventLoop());
        Debug.Log("[S_AlzheimerEventsManager] Event loop started");
    }
    
    /**
     * Arrête la boucle d'events
     */
    public void StopEventLoop()
    {
        eventLoopActive = false;
        if (eventLoopCoroutine != null)
        {
            StopCoroutine(eventLoopCoroutine);
            eventLoopCoroutine = null;
        }
        Debug.Log("[S_AlzheimerEventsManager] Event loop stopped");
    }
    
    /**
     * Modifie l'intervalle d'activation des events
     */
    public void SetActivationInterval(float newInterval)
    {
        activationInterval = Mathf.Max(1f, newInterval);
    }
    
    /**
     * Récupère l'intervalle actuel
     */
    public float GetActivationInterval()
    {
        return activationInterval;
    }
    
    /**
     * Vérifie si la boucle est active
     */
    public bool IsEventLoopActive()
    {
        return eventLoopActive;
    }

    #endregion Event Loop Control

    #region Event Loop
    
    private IEnumerator EventLoop()
    {
        while (eventLoopActive)
        {
            yield return new WaitForSeconds(activationInterval);
            
            // Vérifier si on peut lancer un event
            if (activeEvents.Count < maxActiveEvents)
            {
                TriggerRandomEvent();
            }
            else
            {
                Debug.Log($"[S_AlzheimerEventsManager] Max events reached ({maxActiveEvents}), skipping");
            }
        }
    }

    private void TriggerRandomEvent()
    {
        if (alzheimerEvents.Count == 0)
        {
            Debug.LogWarning("[S_AlzheimerEventsManager] No events in list!");
            return;
        }

        // Calcul du total des poids (seulement pour les events Randomly et non déjà actifs)
        float totalWeight = 0;
        foreach (var alzheimerEvent in alzheimerEvents)
        {
            // Skip si déjà actif
            if (IsEventActive(alzheimerEvent))
                continue;
                
            // Skip si oneshot déjà déclenché
            if (alzheimerEvent.eventIsOneShot && alzheimerEvent.eventHasTriggered)
                continue;
                
            // Skip si pas de type Randomly
            if (alzheimerEvent.eventActivationType != SO_AlzheimerEvent.ActivationType.Randomly)
                continue;
            
            // Skip si poids = 0
            if (alzheimerEvent.eventBaseWeight <= 0)
                continue;
                
            totalWeight += alzheimerEvent.eventBaseWeight;
        }

        if (totalWeight <= 0)
        {
            Debug.Log("[S_AlzheimerEventsManager] No available events to trigger");
            return;
        }

        // Sélection aléatoire pondérée
        float randomValue = Random.Range(0f, totalWeight);
        float currentSum = 0;

        foreach (var alzheimerEvent in alzheimerEvents)
        {
            if (IsEventActive(alzheimerEvent))
                continue;
                
            if (alzheimerEvent.eventIsOneShot && alzheimerEvent.eventHasTriggered)
                continue;

            if (alzheimerEvent.eventActivationType != SO_AlzheimerEvent.ActivationType.Randomly)
                continue;
            
            if (alzheimerEvent.eventBaseWeight <= 0)
                continue;

            currentSum += alzheimerEvent.eventBaseWeight;

            if (randomValue < currentSum)
            {
                TriggerEvent(alzheimerEvent);
                return;
            }
        }
    }
    
    #endregion Event Loop

    #region Event Triggering
    
    /**
     * Lance un event et le track
     */
    public void TriggerEvent(SO_AlzheimerEvent alzheimerEvent)
    {
        if (alzheimerEvent == null) return;
        
        if (alzheimerEvent.eventPrefab == null)
        {
            Debug.LogError($"[S_AlzheimerEventsManager] Event '{alzheimerEvent.eventName}' has no prefab!");
            return;
        }
        
        // Vérifier si l'event est déjà actif
        if (IsEventActive(alzheimerEvent))
        {
            Debug.Log($"[S_AlzheimerEventsManager] Event '{alzheimerEvent.eventName}' is already active");
            return;
        }
        
        // Instancier l'event
        GameObject instance = Instantiate(alzheimerEvent.eventPrefab, transform);
        instance.name = $"Event_{alzheimerEvent.eventName}";
        
        // Ajouter à la liste des events actifs
        var activeEvent = new ActiveEventData(alzheimerEvent, instance);
        activeEvents.Add(activeEvent);
        
        alzheimerEvent.eventHasTriggered = true;
        
        Debug.Log($"[S_AlzheimerEventsManager] Triggered '{alzheimerEvent.eventName}' (duration: {alzheimerEvent.eventDuration}s, intensity: {alzheimerEvent.eventIntensity})");
        Debug.Log($"[S_AlzheimerEventsManager] Active events: {activeEvents.Count}/{maxActiveEvents}");
        
        // Si l'event a une durée, planifier sa destruction
        if (alzheimerEvent.eventDuration > 0)
        {
            StartCoroutine(DestroyEventAfterDuration(activeEvent, alzheimerEvent.eventDuration));
        }
    }
    
    /**
     * Lance un event spécifique par son nom
     */
    public void TriggerEventByName(string eventName)
    {
        var alzheimerEvent = alzheimerEvents.Find(e => e.eventName == eventName);
        if (alzheimerEvent != null)
        {
            TriggerEvent(alzheimerEvent);
        }
        else
        {
            Debug.LogWarning($"[S_AlzheimerEventsManager] Event '{eventName}' not found");
        }
    }
    
    private IEnumerator DestroyEventAfterDuration(ActiveEventData activeEvent, float duration)
    {
        yield return new WaitForSeconds(duration);
        StopEvent(activeEvent);
    }
    
    #endregion Event Triggering

    #region Event Stopping
    
    /**
     * Arrête un event spécifique
     */
    public void StopEvent(ActiveEventData activeEvent)
    {
        if (activeEvent == null) return;
        
        if (activeEvent.instance != null)
        {
            Debug.Log($"[S_AlzheimerEventsManager] Stopping event '{activeEvent.eventData?.eventName}'");
            Destroy(activeEvent.instance);
        }
        
        activeEvents.Remove(activeEvent);
    }
    
    /**
     * Arrête un event par son SO
     */
    public void StopEvent(SO_AlzheimerEvent alzheimerEvent)
    {
        var activeEvent = activeEvents.Find(e => e.eventData == alzheimerEvent);
        if (activeEvent != null)
        {
            StopEvent(activeEvent);
        }
    }
    
    /**
     * Arrête un event par son nom
     */
    public void StopEventByName(string eventName)
    {
        var activeEvent = activeEvents.Find(e => e.eventData != null && e.eventData.eventName == eventName);
        if (activeEvent != null)
        {
            StopEvent(activeEvent);
        }
    }
    
    /**
     * Arrête TOUS les events actifs
     */
    public void StopAllActiveEvents()
    {
        Debug.Log($"[S_AlzheimerEventsManager] Stopping all {activeEvents.Count} active events");
        
        // Copier la liste car on la modifie pendant l'itération
        var eventsToStop = new List<ActiveEventData>(activeEvents);
        foreach (var activeEvent in eventsToStop)
        {
            StopEvent(activeEvent);
        }
        
        activeEvents.Clear();
    }
    
    /**
     * Vérifie si un event est actuellement actif
     */
    public bool IsEventActive(SO_AlzheimerEvent alzheimerEvent)
    {
        return activeEvents.Exists(e => e.eventData == alzheimerEvent && e.instance != null);
    }
    
    /**
     * Vérifie si un event est actif par son nom
     */
    public bool IsEventActiveByName(string eventName)
    {
        return activeEvents.Exists(e => e.eventData != null && e.eventData.eventName == eventName && e.instance != null);
    }
    
    /**
     * Nettoie les events qui ont été détruits (par d'autres scripts ou Unity)
     */
    private void CleanupDestroyedEvents()
    {
        activeEvents.RemoveAll(e => e.instance == null);
    }
    
    #endregion Event Stopping

    #region Debug Methods
    
    [ContextMenu("Debug - Force Trigger Random Event")]
    public void DebugForceRandomEvent()
    {
        Debug.Log("[S_AlzheimerEventsManager] Force triggering random event...");
        TriggerRandomEvent();
    }
    
    [ContextMenu("Debug - Show All Events Status")]
    public void DebugShowEventsStatus()
    {
        Debug.Log("=== ALZHEIMER EVENTS STATUS ===");
        foreach (var alzheimerEvent in alzheimerEvents)
        {
            bool isActive = IsEventActive(alzheimerEvent);
            Debug.Log($"Event: {alzheimerEvent.eventName} {(isActive ? "[ACTIVE]" : "")}");
            Debug.Log($"  - Type: {alzheimerEvent.eventActivationType}");
            Debug.Log($"  - Weight: {alzheimerEvent.eventBaseWeight}");
            Debug.Log($"  - Duration: {alzheimerEvent.eventDuration}s, Intensity: {alzheimerEvent.eventIntensity}");
            Debug.Log($"  - Prefab: {(alzheimerEvent.eventPrefab != null ? alzheimerEvent.eventPrefab.name : "NULL!")}");
        }
        Debug.Log($"\nActive events: {activeEvents.Count}/{maxActiveEvents}");
        Debug.Log($"Loop Active: {eventLoopActive}, Interval: {activationInterval}s");
        Debug.Log("================================");
    }
    
    [ContextMenu("Debug - Show Active Events")]
    public void DebugShowActiveEvents()
    {
        Debug.Log($"=== ACTIVE EVENTS ({activeEvents.Count}) ===");
        foreach (var activeEvent in activeEvents)
        {
            float elapsed = Time.time - activeEvent.startTime;
            Debug.Log($"- {activeEvent.eventData?.eventName}: running for {elapsed:F1}s");
        }
    }
    
    [ContextMenu("Debug - Stop All Events")]
    public void DebugStopAllEvents()
    {
        StopAllActiveEvents();
    }
    
    [ContextMenu("Debug - Trigger First Event")]
    public void DebugTriggerFirstEvent()
    {
        if (alzheimerEvents.Count > 0)
        {
            TriggerEvent(alzheimerEvents[0]);
        }
    }
    
    [ContextMenu("Debug - Reset All HasTriggered")]
    public void DebugResetAllTriggered()
    {
        foreach (var alzheimerEvent in alzheimerEvents)
        {
            alzheimerEvent.eventHasTriggered = false;
        }
        Debug.Log("[S_AlzheimerEventsManager] All events reset (hasTriggered = false)");
    }
    
    #endregion Debug Methods
}
