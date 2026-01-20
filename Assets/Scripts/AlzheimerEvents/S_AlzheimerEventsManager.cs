using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Gestionnaire central des événements Alzheimer
 * Gère la jauge de lucidité et le déclenchement des events
 *
 * @author	---
 * @since	v0.0.1
 * @version	v1.0.0	Sunday, December 21st, 2025.
 * @global
 */
public class S_AlzheimerEventsManager : MonoBehaviour, SI_DataPersistance
{  
    public static S_AlzheimerEventsManager instance { get; private set; }

    //~ Configuration des Events
    [Header("=== ÉVÉNEMENTS ===")]
    [Tooltip("Liste de tous les events disponibles")]
    [SerializeField] private List<SO_AlzheimerEvent> availableEvents = new List<SO_AlzheimerEvent>();

    [Tooltip("Intervalle maximum entre les events (quand lucidité haute)")]
    [SerializeField] private float maxEventInterval = 180f; // 3 minutes

    [Tooltip("Intervalle minimum entre les events (quand lucidité basse)")]
    [SerializeField] private float minEventInterval = 30f; // 30 secondes

    [Tooltip("Nombre maximum d'events simultanés")]
    [SerializeField, Range(1, 10)] private int maxSimultaneousEvents = 3;

    [Tooltip("Activer la boucle d'events aléatoires")]
    [SerializeField] private bool enableEventLoop = true;

    //~ Configuration de la Jauge de Lucidité
    [Header("=== JAUGE DE LUCIDITÉ ===")]
    [Tooltip("Valeur actuelle de la jauge (0-100%)")]
    [SerializeField, Range(0, 100)] private float lucidity = 100f;

    [Tooltip("Seuil en dessous duquel les events peuvent se déclencher")]
    [SerializeField, Range(0, 100)] private float eventActivationThreshold = 60f;

    [Tooltip("Vitesse de diminution de la lucidité par seconde")]
    [SerializeField, Min(0)] private float lucidityDecreaseRate = 0.5f;

    [Tooltip("Activer la diminution automatique de la lucidité")]
    [SerializeField] private bool autoDecreaseLucidity = true;

    //~ Configuration du Cycle Alzheimer
    [Header("=== CYCLE ALZHEIMER ===")]
    [Tooltip("Cycle actuel (augmente la difficulté)")]
    [SerializeField, Min(0)] private int currentCycle = 0;

    [Tooltip("Lucidité à laquelle on passe au cycle suivant")]
    [SerializeField, Range(0, 50)] private float cycleThreshold = 20f;

    //~ Paliers d'intensité
    [Header("=== PALIERS D'INTENSITÉ ===")]
    [SerializeField] private List<LucidityTier> lucidityTiers = new List<LucidityTier>()
    {
        new LucidityTier("Lucide", 60, 100, 0f, 0),
        new LucidityTier("Légèrement confus", 40, 60, 1f, 1),
        new LucidityTier("Confus", 20, 40, 1.5f, 2),
        new LucidityTier("Très confus", 0, 20, 2f, 3)
    };

    //~ État interne
    private List<ActiveEvent> activeEvents = new List<ActiveEvent>();
    private Transform eventsContainer;
    private Coroutine eventLoopCoroutine;
    private Coroutine lucidityDecreaseCoroutine;
    private LucidityTier currentTier;
    private LucidityTier previousTier;

    //~ Classes internes
    [System.Serializable]
    public class LucidityTier
    {
        public string name;
        [Range(0, 100)] public float minLucidity;
        [Range(0, 100)] public float maxLucidity;
        public float intensityMultiplier;
        public int maxEvents;

        public LucidityTier(string name, float min, float max, float multiplier, int maxEvents)
        {
            this.name = name;
            this.minLucidity = min;
            this.maxLucidity = max;
            this.intensityMultiplier = multiplier;
            this.maxEvents = maxEvents;
        }
    }

    private class ActiveEvent
    {
        public SO_AlzheimerEvent eventData;
        public GameObject instance;
        public float startTime;
        public Coroutine durationCoroutine;

        public ActiveEvent(SO_AlzheimerEvent data, GameObject obj)
        {
            eventData = data;
            instance = obj;
            startTime = Time.time;
        }
    }

    //~ Propriétés publiques
    public float Lucidity => lucidity;
    public int CurrentCycle => currentCycle;
    public LucidityTier CurrentTier => currentTier;
    public int ActiveEventsCount => activeEvents.Count;
    public bool EventsAreActive => lucidity < eventActivationThreshold;
    public List<SO_AlzheimerEvent> AvailableEvents => availableEvents;

    //*--------------------------------*
    //* Event Lucidity

    public System.Action OnLucidityZero;

    //*--------------------------------*

    #region Unity Lifecycle

    void Awake()
    {
        // Singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Crée le container pour les events
        eventsContainer = new GameObject("ActiveEvents").transform;
        eventsContainer.SetParent(transform);
    }

    void Start()
    {
        // Initialise le palier actuel
        currentTier = GetTierForLucidity(lucidity);
        previousTier = currentTier;

        // Réinitialise les états des events
        ResetAllEventsState();

        // Démarre les boucles
        if (enableEventLoop)
            StartEventLoop();

        if (autoDecreaseLucidity)
            StartLucidityDecrease();
    }

    void Update()
    {
        // Nettoie les events détruits
        CleanupDestroyedEvents();

        if (lucidity <= 0f)
        {
            OnLucidityZero?.Invoke();
        }
    }

    void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    //!---------------- SI_DataPersistance ----------------

    //~ Sauvegarde Jauge de lucidité

    public void LoadData(S_GameData gameData)
    {
        SetLucidity(gameData.lucidityJauge);
    }

    public void SaveData(S_GameData gameData)
    {
        gameData.lucidityJauge = lucidity;
    }

    public int GetLoadPriority() => 0; // ✅ Priorité normale

    #endregion

    #region Gestion de la Lucidité

    /**
     * Définit la valeur de la jauge de lucidité
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @param	float	value	
     * @return	void
     */
    public void SetLucidity(float value)
    {
        float oldValue = lucidity;
        lucidity = Mathf.Clamp(value, 0f, 100f);

        previousTier = currentTier;
        currentTier = GetTierForLucidity(lucidity);

        // Détecte le changement de palier
        if (currentTier != previousTier)
        {
            OnTierChanged(previousTier, currentTier);
        }

        // Détecte le passage de cycle
        if (lucidity <= cycleThreshold && oldValue > cycleThreshold)
        {
            OnCycleProgress();
        }

        // Met à jour l'intensité des events actifs
        UpdateActiveEventsIntensity();

        // Si la lucidité remonte au-dessus du seuil, désactive les events
        if (lucidity >= eventActivationThreshold && oldValue < eventActivationThreshold)
        {
            OnLucidityRecovered();
        }
    }

    /**
     * Modifie la lucidité de manière relative
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @param	float	delta	
     * @return	void
     */
    public void ModifyLucidity(float delta)
    {
        SetLucidity(lucidity + delta);
    }

    /**
     * Augmente la lucidité (récupération)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @param	float	amount	
     * @return	void
     */
    public void RecoverLucidity(float amount)
    {
        ModifyLucidity(Mathf.Abs(amount));
    }

    /**
     * Diminue la lucidité
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @param	float	amount	
     * @return	void
     */
    public void DecreaseLucidity(float amount)
    {
        ModifyLucidity(-Mathf.Abs(amount));
    }

    private LucidityTier GetTierForLucidity(float value)
    {
        foreach (var tier in lucidityTiers)
        {
            if (value >= tier.minLucidity && value < tier.maxLucidity)
                return tier;
        }
        // Cas limite à 100%
        if (value >= 100f && lucidityTiers.Count > 0)
            return lucidityTiers.FirstOrDefault(t => t.maxLucidity >= 100);
        
        return lucidityTiers.LastOrDefault();
    }

    private void OnTierChanged(LucidityTier oldTier, LucidityTier newTier)
    {
        Debug.Log($"<color=yellow>[AlzheimerEvents]</color> Changement de palier: {oldTier?.name ?? "null"} → {newTier?.name ?? "null"}");

        // Ajuste le nombre d'events actifs
        AdjustActiveEventsForTier();

        // Si on remonte vers un palier plus lucide
        if (newTier != null && oldTier != null && newTier.minLucidity > oldTier.minLucidity)
        {
            // Réduit l'intensité ou désactive certains events
            ReduceEventsOnRecovery();
        }
    }

    private void OnLucidityRecovered()
    {
        Debug.Log($"<color=green>[AlzheimerEvents]</color> Lucidité récupérée! Désactivation de tous les events.");
        DeactivateAllEvents();
    }

    private void OnCycleProgress()
    {
        currentCycle++;
        Debug.Log($"<color=red>[AlzheimerEvents]</color> Passage au cycle {currentCycle}! Difficulté augmentée.");
    }

    private IEnumerator LucidityDecreaseLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (lucidity > 0)
            {
                SetLucidity(lucidity - lucidityDecreaseRate);
            }
        }
    }

    public void StartLucidityDecrease()
    {
        if (lucidityDecreaseCoroutine != null)
            StopCoroutine(lucidityDecreaseCoroutine);
        lucidityDecreaseCoroutine = StartCoroutine(LucidityDecreaseLoop());
    }

    public void StopLucidityDecrease()
    {
        if (lucidityDecreaseCoroutine != null)
        {
            StopCoroutine(lucidityDecreaseCoroutine);
            lucidityDecreaseCoroutine = null;
        }
    }

    /**
     * Setter pour la vitesse de diminution
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, January 16th, 2026.
     * @access	public
     * @param	float	rate	
     * @return	void
     */
    public void SetlucidityDecreaseRate(float rate)
    {
        lucidityDecreaseRate = rate;
    }

    /**
     * Getter pour la vitesse de diminution
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Friday, January 16th, 2026.	
     * @access	public
     * @param	out	floa	
     * @return	void
     */
    public float GetLucidityDecreaseRate()
    {
        return lucidityDecreaseRate;
    }


    #endregion

    #region Gestion des Events

    /**
     * Démarre la boucle d'events aléatoires
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @return	void
     */
    public void StartEventLoop()
    {
        if (eventLoopCoroutine != null)
            StopCoroutine(eventLoopCoroutine);
        eventLoopCoroutine = StartCoroutine(EventLoop());
    }

    /**
     * Arrête la boucle d'events aléatoires
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @return	void
     */
    public void StopEventLoop()
    {
        if (eventLoopCoroutine != null)
        {
            StopCoroutine(eventLoopCoroutine);
            eventLoopCoroutine = null;
        }
    }

    private IEnumerator EventLoop()
    {
        while (true)
        {
            // Calcule l'intervalle dynamique basé sur la lucidité
            float currentInterval = GetDynamicEventInterval();
            yield return new WaitForSeconds(currentInterval);

            // Ne déclenche que si en dessous du seuil
            if (lucidity < eventActivationThreshold)
            {
                TryTriggerRandomEvent();
            }
        }
    }

    /**
     * Calcule l'intervalle entre les events basé sur la lucidité
     * Plus la lucidité est basse, plus l'intervalle est court
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @return	mixed
     */
    public float GetDynamicEventInterval()
    {
        // Normalise la lucidité entre 0 et le seuil d'activation
        float normalizedLucidity = Mathf.Clamp01(lucidity / eventActivationThreshold);
        
        // Interpole entre min et max (inverse: lucidité basse = intervalle court)
        float interval = Mathf.Lerp(minEventInterval, maxEventInterval, normalizedLucidity);
        
        // Applique le modificateur de cycle (chaque cycle réduit l'intervalle de 10%)
        float cycleModifier = 1f - (currentCycle * 0.1f);
        cycleModifier = Mathf.Max(0.3f, cycleModifier); // Minimum 30% de l'intervalle
        
        return interval * cycleModifier;
    }

    /**
     * Tente de déclencher un event aléatoire
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @return	void
     */
    public void TryTriggerRandomEvent()
    {
        if (!CanActivateNewEvent())
        {
            Debug.Log($"<color=orange>[AlzheimerEvents]</color> Impossible d'activer un nouvel event (max atteint ou lucidité trop haute)");
            return;
        }

        // Filtre les events disponibles
        var eligibleEvents = GetEligibleEvents();
        if (eligibleEvents.Count == 0)
        {
            Debug.Log($"<color=orange>[AlzheimerEvents]</color> Aucun event éligible");
            return;
        }

        // Sélection pondérée
        SO_AlzheimerEvent selectedEvent = SelectWeightedEvent(eligibleEvents);
        if (selectedEvent != null)
        {
            ActivateEvent(selectedEvent);
        }
    }

    private List<SO_AlzheimerEvent> GetEligibleEvents()
    {
        return availableEvents.Where(e => 
            e != null &&
            e.eventPrefab != null &&
            e.activationType == SO_AlzheimerEvent.ActivationType.Random &&
            e.CanTriggerAtLucidity(lucidity) &&
            !IsEventActive(e)
        ).ToList();
    }

    private SO_AlzheimerEvent SelectWeightedEvent(List<SO_AlzheimerEvent> events)
    {
        if (events.Count == 0) return null;

        // Calcule les poids ajustés
        float totalWeight = 0;
        var weights = new List<float>();

        foreach (var evt in events)
        {
            float weight = evt.GetAdjustedWeight(lucidity, currentCycle);
            weights.Add(weight);
            totalWeight += weight;
        }

        if (totalWeight <= 0) return events[0];

        // Sélection aléatoire pondérée
        float random = Random.Range(0, totalWeight);
        float cumulative = 0;

        for (int i = 0; i < events.Count; i++)
        {
            cumulative += weights[i];
            if (random < cumulative)
            {
                return events[i];
            }
        }

        return events[events.Count - 1];
    }

    /**
     * Active un event spécifique
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @param	so_alzheimerevent	eventData	
     * @return	void
     */
    public void ActivateEvent(SO_AlzheimerEvent eventData)
    {
        if (eventData == null || eventData.eventPrefab == null)
        {
            Debug.LogWarning("[AlzheimerEvents] Event ou prefab null!");
            return;
        }

        // Vérifie si déjà actif
        if (IsEventActive(eventData) && !eventData.canStack)
        {
            Debug.Log($"<color=orange>[AlzheimerEvents]</color> Event '{eventData.eventName}' déjà actif et non-stackable");
            return;
        }

        // Jouer un son différent lors de l'activation de l'event en fonction de l'intensité
        if (eventData.baseIntensity > 0.5f) // Puissant
        {
            S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.strong_AE, S_FMODEvents.instance.target.position);
        }
        else // Non puissant
        {
            S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.weak_AE, S_FMODEvents.instance.target.position);
        }

        //TODO Post Processing durant 2-3s en fonction de l'intensité aussi peut etre


        // Instancie l'event
        GameObject instance = Instantiate(eventData.eventPrefab, eventsContainer);
        instance.name = $"Event_{eventData.eventName}";

        // Calcule et applique l'intensité
        float multiplier = currentTier?.intensityMultiplier ?? 1f;
        eventData.currentIntensity = eventData.GetAdjustedIntensity(lucidity, multiplier);
        eventData.hasTriggered = true;

        // Crée l'entrée dans la liste active
        var activeEvent = new ActiveEvent(eventData, instance);
        activeEvents.Add(activeEvent);

        Debug.Log($"<color=cyan>[AlzheimerEvents]</color> Event activé: {eventData.eventName} | Intensité: {eventData.currentIntensity:F2} | Lucidité: {lucidity:F1}%");

        // Gère la durée
        if (eventData.duration > 0)
        {
            activeEvent.durationCoroutine = StartCoroutine(DeactivateEventAfterDuration(activeEvent, eventData.duration));
        }
    }

    /**
     * Force l'activation d'un event (ignore les restrictions)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @param	so_alzheimerevent	eventData	
     * @return	void
     */
    public void ForceActivateEvent(SO_AlzheimerEvent eventData)
    {
        if (eventData == null || eventData.eventPrefab == null) return;

        GameObject instance = Instantiate(eventData.eventPrefab, eventsContainer);
        instance.name = $"Event_{eventData.eventName}_Forced";

        float multiplier = currentTier?.intensityMultiplier ?? 1f;
        eventData.currentIntensity = eventData.GetAdjustedIntensity(lucidity, multiplier);
        eventData.hasTriggered = true;

        var activeEvent = new ActiveEvent(eventData, instance);
        activeEvents.Add(activeEvent);

        Debug.Log($"<color=magenta>[AlzheimerEvents]</color> Event FORCÉ: {eventData.eventName}");

        if (eventData.duration > 0)
        {
            activeEvent.durationCoroutine = StartCoroutine(DeactivateEventAfterDuration(activeEvent, eventData.duration));
        }
    }

    private IEnumerator DeactivateEventAfterDuration(ActiveEvent activeEvent, float duration)
    {
        yield return new WaitForSeconds(duration);
        DeactivateEvent(activeEvent);
    }

    /**
     * Désactive un event spécifique
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @param	so_alzheimerevent	eventData	
     * @return	void
     */
    public void DeactivateEvent(SO_AlzheimerEvent eventData)
    {
        var activeEvent = activeEvents.Find(e => e.eventData == eventData);
        if (activeEvent != null)
        {
            DeactivateEvent(activeEvent);
        }
    }

    private void DeactivateEvent(ActiveEvent activeEvent)
    {
        if (activeEvent == null) return;

        if (activeEvent.durationCoroutine != null)
            StopCoroutine(activeEvent.durationCoroutine);

        if (activeEvent.instance != null)
        {
            Debug.Log($"<color=gray>[AlzheimerEvents]</color> Event désactivé: {activeEvent.eventData.eventName}");
            Destroy(activeEvent.instance);
        }

        activeEvents.Remove(activeEvent);
    }

    /**
     * Désactive tous les events actifs
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @return	void
     */
    public void DeactivateAllEvents()
    {
        foreach (var activeEvent in activeEvents.ToList())
        {
            DeactivateEvent(activeEvent);
        }
        activeEvents.Clear();
        Debug.Log($"<color=gray>[AlzheimerEvents]</color> Tous les events désactivés");
    }

    private void CleanupDestroyedEvents()
    {
        activeEvents.RemoveAll(e => e.instance == null);
    }

    private bool CanActivateNewEvent()
    {
        if (lucidity >= eventActivationThreshold) return false;
        if (currentTier == null) return false;

        int maxAllowed = Mathf.Min(currentTier.maxEvents, maxSimultaneousEvents);
        return activeEvents.Count < maxAllowed;
    }

    private bool IsEventActive(SO_AlzheimerEvent eventData)
    {
        return activeEvents.Any(e => e.eventData == eventData && e.instance != null);
    }

    private void AdjustActiveEventsForTier()
    {
        if (currentTier == null) return;

        int maxAllowed = Mathf.Min(currentTier.maxEvents, maxSimultaneousEvents);

        // Désactive les events en trop (par ordre de priorité croissante)
        while (activeEvents.Count > maxAllowed)
        {
            var lowestPriority = activeEvents
                .OrderBy(e => e.eventData.priority)
                .FirstOrDefault();

            if (lowestPriority != null)
                DeactivateEvent(lowestPriority);
            else
                break;
        }
    }

    private void UpdateActiveEventsIntensity()
    {
        float multiplier = currentTier?.intensityMultiplier ?? 1f;

        foreach (var activeEvent in activeEvents)
        {
            if (activeEvent.instance != null)
            {
                activeEvent.eventData.currentIntensity = 
                    activeEvent.eventData.GetAdjustedIntensity(lucidity, multiplier);
            }
        }
    }

    private void ReduceEventsOnRecovery()
    {
        // Quand la lucidité remonte, réduit l'intensité des events
        // ou en désactive certains selon le nouveau palier
        AdjustActiveEventsForTier();
    }

    /**
     * Réinitialise l'état de tous les events
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @return	void
     */
    public void ResetAllEventsState()
    {
        foreach (var evt in availableEvents)
        {
            if (evt != null)
                evt.ResetState();
        }
    }

    /**
     * Récupère la liste des events actuellement actifs
     *
     * @var		mixed	GetActiveEventsList()
     */
    public List<SO_AlzheimerEvent> GetActiveEventsList()
    {
        return activeEvents.Where(e => e.instance != null).Select(e => e.eventData).ToList();
    }

    #endregion

    #region Events spéciaux

    /**
     * Déclenche les events de type OnWakeUp
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @return	void
     */
    public void TriggerWakeUpEvents()
    {
        var wakeUpEvents = availableEvents.Where(e => 
            e != null && 
            e.activationType == SO_AlzheimerEvent.ActivationType.OnWakeUp &&
            e.CanTriggerAtLucidity(lucidity)
        );

        foreach (var evt in wakeUpEvents)
        {
            if (CanActivateNewEvent())
                ActivateEvent(evt);
        }
    }

    /**
     * Déclenche un event de type Story par son nom
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, December 21st, 2025.
     * @access	public
     * @param	string	eventName	
     * @return	void
     */
    public void TriggerStoryEvent(string eventName)
    {
        var storyEvent = availableEvents.FirstOrDefault(e => 
            e != null && 
            e.eventName == eventName && 
            e.activationType == SO_AlzheimerEvent.ActivationType.Story
        );

        if (storyEvent != null && !storyEvent.hasTriggered)
        {
            ForceActivateEvent(storyEvent);
        }
    }

    #endregion

    #region Debug ContextMenu

    [ContextMenu("Debug/Afficher État Complet")]
    private void DebugShowFullState()
    {
        string activeEventNames = activeEvents.Count > 0 
            ? string.Join(", ", activeEvents.Select(e => e.eventData.eventName))
            : "Aucun";

        Debug.Log($@"
<color=yellow>========== ÉTAT ALZHEIMER EVENTS ==========</color>
<color=white>Lucidité:</color> {lucidity:F1}%
<color=white>Palier actuel:</color> {currentTier?.name ?? "null"} (mult: x{currentTier?.intensityMultiplier ?? 0})
<color=white>Cycle:</color> {currentCycle}
<color=white>Events actifs:</color> {activeEvents.Count}/{currentTier?.maxEvents ?? 0} - [{activeEventNames}]
<color=white>Seuil d'activation:</color> {eventActivationThreshold}%
<color=white>Intervalle actuel:</color> {GetDynamicEventInterval():F1}s (min: {minEventInterval}s, max: {maxEventInterval}s)
<color=white>Diminution auto:</color> {(autoDecreaseLucidity ? "Oui" : "Non")} ({lucidityDecreaseRate}/s)
<color=yellow>=============================================</color>
");
    }

    [ContextMenu("Debug/Lucidité -10%")]
    private void DebugDecreaseLucidity10()
    {
        DecreaseLucidity(10f);
        Debug.Log($"<color=red>[Debug]</color> Lucidité: {lucidity:F1}%");
    }

    [ContextMenu("Debug/Lucidité -25%")]
    private void DebugDecreaseLucidity25()
    {
        DecreaseLucidity(25f);
        Debug.Log($"<color=red>[Debug]</color> Lucidité: {lucidity:F1}%");
    }

    [ContextMenu("Debug/Lucidité +10%")]
    private void DebugIncreaseLucidity10()
    {
        RecoverLucidity(10f);
        Debug.Log($"<color=green>[Debug]</color> Lucidité: {lucidity:F1}%");
    }

    [ContextMenu("Debug/Lucidité +25%")]
    private void DebugIncreaseLucidity25()
    {
        RecoverLucidity(25f);
        Debug.Log($"<color=green>[Debug]</color> Lucidité: {lucidity:F1}%");
    }

    [ContextMenu("Debug/Lucidité = 100%")]
    private void DebugMaxLucidity()
    {
        SetLucidity(100f);
        Debug.Log($"<color=green>[Debug]</color> Lucidité restaurée à 100%");
    }

    [ContextMenu("Debug/Lucidité = 50%")]
    private void DebugHalfLucidity()
    {
        SetLucidity(50f);
        Debug.Log($"<color=yellow>[Debug]</color> Lucidité à 50%");
    }

    [ContextMenu("Debug/Lucidité = 0%")]
    private void DebugMinLucidity()
    {
        SetLucidity(0f);
        Debug.Log($"<color=red>[Debug]</color> Lucidité à 0%!");
    }

    [ContextMenu("Debug/Forcer Event Aléatoire")]
    private void DebugForceRandomEvent()
    {
        var oldMaxEvents = maxSimultaneousEvents;
        maxSimultaneousEvents = 99;
        TryTriggerRandomEvent();
        maxSimultaneousEvents = oldMaxEvents;
    }

    [ContextMenu("Debug/Désactiver Tous les Events")]
    private void DebugDeactivateAll()
    {
        DeactivateAllEvents();
    }

    [ContextMenu("Debug/Lister Events Éligibles")]
    private void DebugListEligibleEvents()
    {
        var eligible = GetEligibleEvents();
        if (eligible.Count == 0)
        {
            Debug.Log("<color=orange>[Debug]</color> Aucun event éligible actuellement");
            return;
        }

        string list = string.Join("\n", eligible.Select(e => 
            $"  - {e.eventName} (poids ajusté: {e.GetAdjustedWeight(lucidity, currentCycle):F2})"));
        Debug.Log($"<color=cyan>[Debug]</color> Events éligibles ({eligible.Count}):\n{list}");
    }

    [ContextMenu("Debug/Cycle +1")]
    private void DebugIncrementCycle()
    {
        currentCycle++;
        Debug.Log($"<color=magenta>[Debug]</color> Cycle: {currentCycle}");
    }

    [ContextMenu("Debug/Reset Cycle")]
    private void DebugResetCycle()
    {
        currentCycle = 0;
        Debug.Log($"<color=magenta>[Debug]</color> Cycle réinitialisé");
    }

    [ContextMenu("Debug/Toggle Diminution Auto")]
    private void DebugToggleAutoDecrease()
    {
        autoDecreaseLucidity = !autoDecreaseLucidity;
        if (autoDecreaseLucidity)
            StartLucidityDecrease();
        else
            StopLucidityDecrease();
        Debug.Log($"<color=yellow>[Debug]</color> Diminution auto: {(autoDecreaseLucidity ? "ON" : "OFF")}");
    }

    [ContextMenu("Debug/Reset États Events")]
    private void DebugResetEventsState()
    {
        ResetAllEventsState();
        Debug.Log("<color=yellow>[Debug]</color> États des events réinitialisés");
    }

    

    #endregion
}
