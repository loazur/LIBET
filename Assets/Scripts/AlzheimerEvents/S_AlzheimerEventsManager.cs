using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class S_AlzheimerEventsManager : MonoBehaviour
{
    public static S_AlzheimerEventsManager instance; // Singleton pour accès global

    [Header("Gestion des events du jeu")]
    [Tooltip("Liste des events activable")]
    [SerializeField] private List<SO_AlzheimerEvent> alzheimerEvents = new List<SO_AlzheimerEvent>();

    [Tooltip("Tout les combiens de temps un event s'active en secondes")]
    [SerializeField] private float activationInterval = 180f;

    [Tooltip("Si la boucle d'event aléatoire est active")]
    [SerializeField] private bool eventLoopActive = true;

    //~ Système de jauge d'intensité
    [Header("Jauge d'intensité")]
    [Tooltip("Valeur actuelle de la jauge (0-100%)")]
    [SerializeField, Range(0f, 100f)] private float currentIntensityGauge = 100f;

    [Tooltip("Nombre maximum d'events actifs en même temps")]
    [SerializeField, Min(1)] private int maxSimultaneousEvents = 3;

    [Tooltip("Vitesse de décroissance de la jauge par seconde")]
    [SerializeField, Min(0)] private float gaugeDecreaseRate = 0.5f;

    [Tooltip("Si la jauge diminue automatiquement")]
    [SerializeField] private bool autoDecreaseGauge = true;

    //~ Paliers d'intensité configurables
    [Header("Paliers d'intensité (configurables)")]
    [Tooltip("Liste des paliers d'intensité triés du plus haut au plus bas")]
    [SerializeField] private List<IntensityTier> intensityTiers = new List<IntensityTier>()
    {
        new IntensityTier("Aucun", 70f, 100f, 0f, 0),
        new IntensityTier("Faible", 50f, 70f, 0.5f, 1),
        new IntensityTier("Moyen", 30f, 50f, 1f, 2),
        new IntensityTier("Fort", 0f, 30f, 2f, 3)
    };

    // Variables internes
    private List<ActiveEventInstance> activeEvents = new List<ActiveEventInstance>();
    private IntensityTier currentTier;
    private IntensityTier previousTier;

    //~ Classes pour le système
    [System.Serializable]
    public class IntensityTier
    {
        [Tooltip("Nom du palier")]
        public string tierName;
        [Tooltip("Seuil minimum (inclus)")]
        public float minThreshold;
        [Tooltip("Seuil maximum (exclus)")]
        public float maxThreshold;
        [Tooltip("Multiplicateur d'intensité des events")]
        public float intensityMultiplier;
        [Tooltip("Nombre max d'events pour ce palier")]
        public int maxEventsForTier;

        public IntensityTier(string name, float min, float max, float multiplier, int maxEvents)
        {
            tierName = name;
            minThreshold = min;
            maxThreshold = max;
            intensityMultiplier = multiplier;
            maxEventsForTier = maxEvents;
        }
    }

    // Stocke les instances d'events actifs
    private class ActiveEventInstance
    {
        public SO_AlzheimerEvent eventData;
        public GameObject instance;
        public float originalIntensity;

        public ActiveEventInstance(SO_AlzheimerEvent data, GameObject obj)
        {
            eventData = data;
            instance = obj;
            originalIntensity = data.eventIntensity;
        }
    }

    //~ Propriétés publiques
    public float CurrentIntensityGauge => currentIntensityGauge;
    public IntensityTier CurrentTier => currentTier;
    public int ActiveEventsCount => activeEvents.Count;
    public List<SO_AlzheimerEvent> AlzheimerEvents => alzheimerEvents;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {   
        currentTier = GetCurrentTier();
        previousTier = currentTier;

        if (eventLoopActive)
            StartCoroutine(EventLoop());

        if (autoDecreaseGauge)
            StartCoroutine(AutoDecreaseGaugeLoop());
    }

    void Update()
    {
        // Nettoie les events détruits de la liste
        CleanupDestroyedEvents();
    }

    //~ Gestion de la jauge d'intensité
    private IEnumerator AutoDecreaseGaugeLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (currentIntensityGauge > 0)
            {
                SetIntensityGauge(currentIntensityGauge - gaugeDecreaseRate);
            }
        }
    }

    /**
     * 
     * Définit la valeur de la jauge d'intensité et met à jour les events
     * 
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	public
     * @param	float	value	
     * @return	void
     */
    public void SetIntensityGauge(float value)
    {
        float oldValue = currentIntensityGauge;
        currentIntensityGauge = Mathf.Clamp(value, 0f, 100f);

        previousTier = currentTier;
        currentTier = GetCurrentTier();

        // Si on change de palier
        if (currentTier != previousTier)
        {
            OnTierChanged(previousTier, currentTier);
        }

        // Met à jour l'intensité de tous les events actifs
        UpdateActiveEventsIntensity();
    }

    /**
     * Ajoute ou retire de la valeur à la jauge
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	public
     * @param	float	delta	
     * @return	void
     */
    public void ModifyIntensityGauge(float delta)
    {
        SetIntensityGauge(currentIntensityGauge + delta);
    }

    /**
     * Retourne le palier actuel en fonction de la jauge
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	private
     * @return	mixed
     */
    private IntensityTier GetCurrentTier()
    {
        foreach (var tier in intensityTiers)
        {
            if (currentIntensityGauge >= tier.minThreshold && currentIntensityGauge < tier.maxThreshold)
                return tier;
        }
        // Si on est à 100%, on prend le premier palier
        if (currentIntensityGauge >= 100f && intensityTiers.Count > 0)
            return intensityTiers[0];

        return null;
    }

    /**
     * Appelé quand on change de palier
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	private
     * @param	intensitytier	oldTier	
     * @param	intensitytier	newTier	
     * @return	void
     */
    private void OnTierChanged(IntensityTier oldTier, IntensityTier newTier)
    {
        Debug.Log($"[AlzheimerEvents] Changement de palier: {oldTier?.tierName ?? "null"} -> {newTier?.tierName ?? "null"}");

        // Si la jauge remonte (nouveau palier plus haut)
        if (newTier != null && oldTier != null && newTier.minThreshold > oldTier.minThreshold)
        {
            OnGaugeIncreased(newTier);
        }

        // Ajuste le nombre d'events actifs selon le nouveau palier
        AdjustActiveEventsCount();
    }

    /**
     * Appelé quand la jauge remonte vers un palier supérieur
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	private
     * @param	intensitytier	newTier	
     * @return	void
     */
    private void OnGaugeIncreased(IntensityTier newTier)
    {
        // Si on est dans le palier "aucun event" (100-70% par défaut)
        if (newTier.maxEventsForTier == 0)
        {
            DeactivateAllEvents();
        }
    }

    /**
     * Désactive tous les events actifs
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	public
     * @return	void
     */
    public void DeactivateAllEvents()
    {
        foreach (var activeEvent in activeEvents.ToList())
        {
            if (activeEvent.instance != null)
            {
                // Restaure l'intensité originale avant destruction
                activeEvent.eventData.eventIntensity = activeEvent.originalIntensity;
                Destroy(activeEvent.instance);
            }
        }
        activeEvents.Clear();
        Debug.Log("[AlzheimerEvents] Tous les events ont été désactivés");
    }

    /**
     * Ajuste le nombre d'events actifs selon le palier
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	private
     * @return	void
     */
    private void AdjustActiveEventsCount()
    {
        if (currentTier == null) return;

        int maxAllowed = Mathf.Min(currentTier.maxEventsForTier, maxSimultaneousEvents);

        // Si on a trop d'events actifs, on en désactive
        while (activeEvents.Count > maxAllowed)
        {
            var eventToRemove = activeEvents[activeEvents.Count - 1];
            if (eventToRemove.instance != null)
            {
                eventToRemove.eventData.eventIntensity = eventToRemove.originalIntensity;
                Destroy(eventToRemove.instance);
            }
            activeEvents.RemoveAt(activeEvents.Count - 1);
        }
    }

    /**
     * Met à jour l'intensité de tous les events actifs
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	private
     * @return	void
     */
    private void UpdateActiveEventsIntensity()
    {
        if (currentTier == null) return;

        foreach (var activeEvent in activeEvents)
        {
            if (activeEvent.instance != null)
            {
                // Applique le multiplicateur d'intensité du palier
                activeEvent.eventData.eventIntensity = activeEvent.originalIntensity * currentTier.intensityMultiplier;
            }
        }
    }

    /**
     * Nettoie les events détruits de la liste
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	private
     * @return	void
     */
    private void CleanupDestroyedEvents()
    {
        activeEvents.RemoveAll(e => e.instance == null);
    }

    /**
     * Vérifie si on peut activer un nouvel event
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	private
     * @return	boolean
     */
    private bool CanActivateNewEvent()
    {
        if (currentTier == null) return false;
        if (currentTier.maxEventsForTier == 0) return false;

        int maxAllowed = Mathf.Min(currentTier.maxEventsForTier, maxSimultaneousEvents);
        return activeEvents.Count < maxAllowed;
    }

    //~ Boucles et déclenchement d'events
    private IEnumerator EventLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(activationInterval);
            
            if (CanActivateNewEvent())
            {
                TriggerRandomEvent();
            }
        }
    }

    private void TriggerRandomEvent()
    {
        if (alzheimerEvents.Count == 0) return;
        if (!CanActivateNewEvent()) return;

        // Calcul du total des poids avec multiplicateur de palier
        float totalWeight = 0;
        foreach (var alzheimerEvent in alzheimerEvents)
        {
            if (!(alzheimerEvent.eventIsOneShot && alzheimerEvent.eventHasTriggered && alzheimerEvent.eventActivationType == SO_AlzheimerEvent.ActivationType.Randomly))
            {
                // Applique le multiplicateur du palier au poids
                float adjustedWeight = alzheimerEvent.eventBaseWeight * (currentTier?.intensityMultiplier ?? 1f);
                totalWeight += adjustedWeight;
            }
        }

        if (totalWeight == 0) return;

        float randomValue = Random.Range(0, totalWeight);
        float currentSum = 0;

        foreach (var alzheimerEvent in alzheimerEvents)
        {
            if (alzheimerEvent.eventIsOneShot && alzheimerEvent.eventHasTriggered)
                continue;

            if (alzheimerEvent.eventActivationType != SO_AlzheimerEvent.ActivationType.Randomly)
                continue;

            float adjustedWeight = alzheimerEvent.eventBaseWeight * (currentTier?.intensityMultiplier ?? 1f);
            currentSum += adjustedWeight;

            if (randomValue < currentSum)
            {
                TriggerEventWithIntensity(alzheimerEvent);
                return;
            }
        }
    }

    /**
     * Déclenche un event avec l'intensité ajustée selon le palier
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	private
     * @param	so_alzheimerevent	alzheimerEvent	
     * @return	void
     */
    private void TriggerEventWithIntensity(SO_AlzheimerEvent alzheimerEvent)
    {
        if (!CanActivateNewEvent()) return;

        Transform parent = GameObject.Find("AlzheimerEventsManager")?.transform ?? new GameObject("AlzheimerEventsManager").transform;
        GameObject instance = Instantiate(alzheimerEvent.eventPrefab, parent);

        // Sauvegarde l'intensité originale et applique le multiplicateur
        float originalIntensity = alzheimerEvent.eventIntensity;
        alzheimerEvent.eventIntensity = originalIntensity * (currentTier?.intensityMultiplier ?? 1f);

        alzheimerEvent.eventHasTriggered = true;

        // Ajoute à la liste des events actifs
        var activeEvent = new ActiveEventInstance(alzheimerEvent, instance);
        activeEvent.originalIntensity = originalIntensity;
        activeEvents.Add(activeEvent);

        Debug.Log($"[AlzheimerEvents] Event activé: {alzheimerEvent.eventName} | Intensité: {alzheimerEvent.eventIntensity:F2} | Palier: {currentTier?.tierName}");

        if (alzheimerEvent.eventDuration != 0)
        {
            StartCoroutine(DestroyEventAfterDuration(activeEvent, alzheimerEvent.eventDuration));
        }
    }

    private IEnumerator DestroyEventAfterDuration(ActiveEventInstance activeEvent, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (activeEvent.instance != null)
        {
            activeEvent.eventData.eventIntensity = activeEvent.originalIntensity;
            Destroy(activeEvent.instance);
        }
        activeEvents.Remove(activeEvent);
    }

    /**
     * Lance un event spécifique avec gestion de l'intensité
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	public
     * @param	so_alzheimerevent	alzheimerEvent	
     * @return	void
     */
    public void TriggerSpecificEvent(SO_AlzheimerEvent alzheimerEvent)
    {
        if (alzheimerEvent.eventIsOneShot && alzheimerEvent.eventHasTriggered) return;
        if (!CanActivateNewEvent()) return;

        TriggerEventWithIntensity(alzheimerEvent);
    }

    /**
     * Force l'activation d'un event même si le palier ne le permet pas
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	public
     * @param	so_alzheimerevent	alzheimerEvent	
     * @return	void
     */
    public void ForceActivateEvent(SO_AlzheimerEvent alzheimerEvent)
    {
        if (alzheimerEvent.eventIsOneShot && alzheimerEvent.eventHasTriggered) return;

        Transform parent = GameObject.Find("AlzheimerEventsManager")?.transform ?? new GameObject("AlzheimerEventsManager").transform;
        GameObject instance = Instantiate(alzheimerEvent.eventPrefab, parent);

        float originalIntensity = alzheimerEvent.eventIntensity;
        alzheimerEvent.eventIntensity = originalIntensity * (currentTier?.intensityMultiplier ?? 1f);
        alzheimerEvent.eventHasTriggered = true;

        var activeEvent = new ActiveEventInstance(alzheimerEvent, instance);
        activeEvent.originalIntensity = originalIntensity;
        activeEvents.Add(activeEvent);

        if (alzheimerEvent.eventDuration != 0)
        {
            StartCoroutine(DestroyEventAfterDuration(activeEvent, alzheimerEvent.eventDuration));
        }
    }

    /**
     * Redéfinit le poids d'un event spécifique
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	public
     * @param	so_alzheimerevent	alzheimerEvent	
     * @param	float            	newWeight     	
     * @return	void
     */
    public void SetEventWeight(SO_AlzheimerEvent alzheimerEvent, float newWeight)
    {
        alzheimerEvent.eventBaseWeight = Mathf.Max(0, newWeight);
    }

    /**
     * Redéfinit l'intensité de base d'un event spécifique
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	public
     * @param	so_alzheimerevent	alzheimerEvent	
     * @param	float            	newIntensity  	
     * @return	void
     */
    public void SetEventBaseIntensity(SO_AlzheimerEvent alzheimerEvent, float newIntensity)
    {
        // Met à jour l'intensité dans la liste des events actifs si présent
        var activeEvent = activeEvents.Find(e => e.eventData == alzheimerEvent);
        if (activeEvent != null)
        {
            activeEvent.originalIntensity = newIntensity;
            alzheimerEvent.eventIntensity = newIntensity * (currentTier?.intensityMultiplier ?? 1f);
        }
        else
        {
            alzheimerEvent.eventIntensity = newIntensity;
        }
    }

    /**
     * Récupère la liste des events actifs actuellement
     *
     * @var		mixed	GetActiveEvents()
     */
    public List<SO_AlzheimerEvent> GetActiveEvents()
    {
        return activeEvents.Where(e => e.instance != null).Select(e => e.eventData).ToList();
    }

    /// <summary>
    /// Désactive un event spécifique s'il est actif
    /// </summary>
    public void DeactivateEvent(SO_AlzheimerEvent alzheimerEvent)
    {
        var activeEvent = activeEvents.Find(e => e.eventData == alzheimerEvent);
        if (activeEvent != null && activeEvent.instance != null)
        {
            activeEvent.eventData.eventIntensity = activeEvent.originalIntensity;
            Destroy(activeEvent.instance);
            activeEvents.Remove(activeEvent);
        }
    }

    //~ Debug
    [ContextMenu("Afficher état de la jauge")]
    private void DebugShowGaugeState()
    {
        Debug.Log($"[AlzheimerEvents] Jauge: {currentIntensityGauge}% | Palier: {currentTier?.tierName ?? "null"} | Events actifs: {activeEvents.Count}");
    }

    [ContextMenu("Diminuer jauge de 10%")]
    private void DebugDecreaseGauge()
    {
        ModifyIntensityGauge(-10f);
    }

    [ContextMenu("Augmenter jauge de 10%")]
    private void DebugIncreaseGauge()
    {
        ModifyIntensityGauge(10f);
    }
}
