// Jauge de lucidité
// Plus c'est bas plus l'intervale des events est court + plus ils sont forts et durent longtemps

using System.Collections.Generic;
using UnityEngine;

public class S_LucidityGauge : MonoBehaviour
{
    #region Attributes
    
    [Header("Gauge Settings")]
    [SerializeField, Range(0f, 100f)] 
    private float gauge = 100f;
    
    [Header("Interval Settings")]
    [Tooltip("Intervalle max entre les events (quand lucidité = 100)")]
    [SerializeField, Min(1f)] private float maxInterval = 300f;
    
    [Tooltip("Intervalle min entre les events (quand lucidité = 0)")]
    [SerializeField, Min(1f)] private float minInterval = 30f;
    
    [Header("References")]
    [SerializeField] private S_AlzheimerEventsManager alzheimerEventsManager;
    
    [Header("Event Configurations")]
    [Tooltip("Configuration individuelle de chaque event par rapport à la jauge")]
    [SerializeField] private List<S_EventLucidityConfig> eventConfigs = new List<S_EventLucidityConfig>();
    
    // Propriété publique pour accéder à la jauge
    public float Gauge => gauge;
    
    #endregion Attributes

    #region Unity Methods
    
    private void Awake()
    {
        // Initialiser toutes les configs AVANT le Start des autres scripts
        InitializeAllConfigs();
    }
    
    private void Start()
    {
        UpdateAllFromGauge();
        Debug.Log($"[S_LucidityGauge] Started with gauge = {gauge}%, interval = {GetCurrentInterval()}s");
    }
    
    private void OnApplicationQuit()
    {
        // Restaurer les valeurs de base quand on quitte le jeu
        ResetAllEventsToBase();
    }
    
    private void OnDestroy()
    {
        ResetAllEventsToBase();
    }
    
    #endregion Unity Methods

    #region Initialization
    
    /**
     * Initialise toutes les configurations d'events
     */
    private void InitializeAllConfigs()
    {
        Debug.Log($"[S_LucidityGauge] Initializing {eventConfigs.Count} event configs...");
        foreach (var config in eventConfigs)
        {
            config.Initialize();
        }
    }
    
    #endregion Initialization

    #region Gauge Management
    
    /**
     * Récupère l'intervalle actuel basé sur la jauge
     */
    public float GetCurrentInterval()
    {
        float normalizedLucidity = Mathf.Clamp01(gauge / 100f);
        return Mathf.Lerp(minInterval, maxInterval, normalizedLucidity);
    }
    
    /**
     * Met à jour tous les paramètres en fonction de la jauge actuelle
     */
    private void UpdateAllFromGauge()
    {
        UpdateEventConfigs();
        UpdateEventInterval();
    }
    
    /**
     * Met à jour toutes les configurations d'events
     */
    private void UpdateEventConfigs()
    {
        foreach (var config in eventConfigs)
        {
            config.UpdateFromLucidity(gauge);
        }
    }
    
    /**
     * Met à jour l'intervalle d'activation des events
     */
    private void UpdateEventInterval()
    {
        if (alzheimerEventsManager == null) return;
        
        float newInterval = GetCurrentInterval();
        alzheimerEventsManager.SetActivationInterval(newInterval);
        Debug.Log($"[S_LucidityGauge] Event interval set to {newInterval}s (gauge: {gauge}%)");
    }
    
    /**
     * Baisse la jauge et met à jour les events
     */
    public void DecreaseGauge(float amount)
    {
        gauge = Mathf.Clamp(gauge - amount, 0f, 100f);
        UpdateAllFromGauge();
    }

    /**
     * Augmente la jauge et met à jour les events
     */
    public void IncreaseGauge(float amount)
    {
        gauge = Mathf.Clamp(gauge + amount, 0f, 100f);
        UpdateAllFromGauge();
    }
    
    /**
     * Définit directement la valeur de la jauge
     */
    public void SetGauge(float value)
    {
        gauge = Mathf.Clamp(value, 0f, 100f);
        UpdateAllFromGauge();
    }
    
    #endregion Gauge Management

    #region Event Config Access
    
    /**
     * Récupère la configuration d'un event par son nom
     */
    public S_EventLucidityConfig GetConfigByName(string eventName)
    {
        return eventConfigs.Find(c => c.alzheimerEvent != null && c.alzheimerEvent.eventName == eventName);
    }
    
    /**
     * Récupère la configuration d'un event par son ScriptableObject
     */
    public S_EventLucidityConfig GetConfig(SO_AlzheimerEvent alzheimerEvent)
    {
        return eventConfigs.Find(c => c.alzheimerEvent == alzheimerEvent);
    }
    
    /**
     * Remet toutes les valeurs d'events à leur état de base
     */
    public void ResetAllEventsToBase()
    {
        foreach (var config in eventConfigs)
        {
            config.ResetToBase();
        }
    }
    
    /**
     * Récupère la liste de toutes les configurations
     */
    public List<S_EventLucidityConfig> GetAllConfigs()
    {
        return eventConfigs;
    }
    
    #endregion Event Config Access

    #region Debug Methods

    /**
     * Auto-populate les configs depuis le manager
     */
    [ContextMenu("Auto-Populate Event Configs")]
    public void AutoPopulateConfigs()
    {
        if (alzheimerEventsManager == null)
        {
            Debug.LogWarning("[S_LucidityGauge] AlzheimerEventsManager non assigné!");
            return;
        }
        
        eventConfigs.Clear();
        
        foreach (var alzheimerEvent in alzheimerEventsManager.alzheimerEvents)
        {
            var config = new S_EventLucidityConfig
            {
                alzheimerEvent = alzheimerEvent,
                useBaseWeightAsMin = true,
                maxWeight = alzheimerEvent.eventBaseWeight * 2f,
                minDurationMultiplier = 1f,
                maxDurationMultiplier = 2f,
                minIntensityMultiplier = 1f,
                maxIntensityMultiplier = 2f
            };
            eventConfigs.Add(config);
        }
        
        Debug.Log($"[S_LucidityGauge] {eventConfigs.Count} configurations créées.");
    }

    [ContextMenu("Debug - Afficher Jauge")]
    void DebugShowGauge()
    {
        Debug.Log($"[S_LucidityGauge] Jauge: {gauge}%");
        Debug.Log($"[S_LucidityGauge] Intervalle actuel: {GetCurrentInterval()}s");
    }

    [ContextMenu("Debug - Afficher Configs")]
    void DebugShowConfigs()
    {
        Debug.Log("=== EVENT CONFIGS ===");
        foreach (var config in eventConfigs)
        {
            if (config.alzheimerEvent != null)
            {
                Debug.Log($"Event: {config.alzheimerEvent.eventName}");
                Debug.Log($"  - Poids actuel: {config.CurrentWeight} (base: {config.GetBaseWeight()}, max: {config.maxWeight})");
                Debug.Log($"  - Durée: {config.CurrentDuration}s (base: {config.GetBaseDuration()})");
                Debug.Log($"  - Intensité: {config.CurrentIntensity} (base: {config.GetBaseIntensity()})");
                Debug.Log($"  - Ignore gauge: {config.ignoreGaugeModifications}");
            }
        }
        Debug.Log("====================");
    }

    [ContextMenu("Debug - Set Gauge 0% (Max Effects)")]
    void DebugSetGaugeZero()
    {
        SetGauge(0f);
        Debug.Log("[S_LucidityGauge] Jauge mise à 0% (effets maximum)");
        DebugShowConfigs();
    }
    
    [ContextMenu("Debug - Set Gauge 50%")]
    void DebugSetGaugeHalf()
    {
        SetGauge(50f);
        Debug.Log("[S_LucidityGauge] Jauge mise à 50%");
        DebugShowConfigs();
    }
    
    [ContextMenu("Debug - Set Gauge 100% (Min Effects)")]
    void DebugSetGaugeFull()
    {
        SetGauge(100f);
        Debug.Log("[S_LucidityGauge] Jauge mise à 100% (effets minimum)");
        DebugShowConfigs();
    }

    [ContextMenu("Debug - Test Decrease (10)")]
    void DebugDecrease()
    {
        DecreaseGauge(10f);
        Debug.Log($"[S_LucidityGauge] Jauge après diminution: {gauge}%");
    }

    [ContextMenu("Debug - Test Increase (10)")]
    void DebugIncrease()
    {
        IncreaseGauge(10f);
        Debug.Log($"[S_LucidityGauge] Jauge après augmentation: {gauge}%");
    }
    
    [ContextMenu("Debug - Force Reset All Events")]
    void DebugForceReset()
    {
        ResetAllEventsToBase();
        Debug.Log("[S_LucidityGauge] All events reset to base values");
    }

    #endregion Debug Methods
}