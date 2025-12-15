// Jauge de lucidité
// Plus c'est bas plus l'intervale des events est court + plus ils sont forts et durent longtemps
// > 70% = Pas d'events
// 40-70% = Effets doux
// 20-40% = Effets moyens
// < 20% = Effets intenses

using System.Collections.Generic;
using UnityEngine;

public class S_LucidityGauge : MonoBehaviour
{
    #region Enums
    
    public enum LucidityLevel
    {
        Safe,       // > 70% - Pas d'events
        Mild,       // 40-70% - Effets doux
        Moderate,   // 20-40% - Effets moyens
        Severe      // < 20% - Effets intenses
    }
    
    #endregion Enums

    #region Attributes
    
    [Header("Gauge Settings")]
    [SerializeField, Range(0f, 100f)] 
    private float gauge = 100f;
    
    [Header("Lucidity Thresholds")]
    [Tooltip("Au dessus de ce seuil = pas d'events")]
    [SerializeField] private float safeThreshold = 70f;
    
    [Tooltip("Au dessus de ce seuil = effets doux")]
    [SerializeField] private float mildThreshold = 40f;
    
    [Tooltip("Au dessus de ce seuil = effets moyens, en dessous = effets intenses")]
    [SerializeField] private float severeThreshold = 20f;
    
    [Header("Interval Settings (by level)")]
    [Tooltip("Intervalle quand effets doux (40-70%)")]
    [SerializeField, Min(1f)] private float mildInterval = 180f;
    
    [Tooltip("Intervalle quand effets moyens (20-40%)")]
    [SerializeField, Min(1f)] private float moderateInterval = 90f;
    
    [Tooltip("Intervalle quand effets intenses (<20%)")]
    [SerializeField, Min(1f)] private float severeInterval = 30f;
    
    [Header("Intensity Multipliers (by level)")]
    [Tooltip("Multiplicateur d'intensité pour effets doux")]
    [SerializeField] private float mildIntensityMult = 0.5f;
    
    [Tooltip("Multiplicateur d'intensité pour effets moyens")]
    [SerializeField] private float moderateIntensityMult = 1f;
    
    [Tooltip("Multiplicateur d'intensité pour effets intenses")]
    [SerializeField] private float severeIntensityMult = 2f;
    
    [Header("Duration Multipliers (by level)")]
    [Tooltip("Multiplicateur de durée pour effets doux")]
    [SerializeField] private float mildDurationMult = 0.5f;
    
    [Tooltip("Multiplicateur de durée pour effets moyens")]
    [SerializeField] private float moderateDurationMult = 1f;
    
    [Tooltip("Multiplicateur de durée pour effets intenses")]
    [SerializeField] private float severeDurationMult = 2f;
    
    [Header("References")]
    [SerializeField] private S_AlzheimerEventsManager alzheimerEventsManager;
    
    [Header("Event Configurations")]
    [Tooltip("Configuration individuelle de chaque event par rapport à la jauge")]
    [SerializeField] private List<S_EventLucidityConfig> eventConfigs = new List<S_EventLucidityConfig>();
    
    // État actuel
    private LucidityLevel currentLevel = LucidityLevel.Safe;
    private LucidityLevel previousLevel = LucidityLevel.Safe;
    
    // Propriétés publiques
    public float Gauge => gauge;
    public LucidityLevel CurrentLevel => currentLevel;
    
    #endregion Attributes

    #region Unity Methods
    
    private void Awake()
    {
        InitializeAllConfigs();
    }
    
    private void Start()
    {
        UpdateAllFromGauge();
        Debug.Log($"[S_LucidityGauge] Started - Gauge: {gauge}%, Level: {currentLevel}");
    }
    
    private void OnApplicationQuit()
    {
        ResetAllEventsToBase();
    }
    
    private void OnDestroy()
    {
        ResetAllEventsToBase();
    }
    
    #endregion Unity Methods

    #region Initialization
    
    private void InitializeAllConfigs()
    {
        Debug.Log($"[S_LucidityGauge] Initializing {eventConfigs.Count} event configs...");
        foreach (var config in eventConfigs)
        {
            config.Initialize();
        }
    }
    
    #endregion Initialization

    #region Lucidity Level
    
    /**
     * Détermine le niveau de lucidité actuel
     */
    private LucidityLevel CalculateLucidityLevel(float gaugeValue)
    {
        if (gaugeValue > safeThreshold)
            return LucidityLevel.Safe;
        else if (gaugeValue > mildThreshold)
            return LucidityLevel.Mild;
        else if (gaugeValue > severeThreshold)
            return LucidityLevel.Moderate;
        else
            return LucidityLevel.Severe;
    }
    
    /**
     * Récupère les multiplicateurs pour le niveau actuel
     */
    private void GetMultipliersForLevel(LucidityLevel level, out float intensityMult, out float durationMult)
    {
        switch (level)
        {
            case LucidityLevel.Mild:
                intensityMult = mildIntensityMult;
                durationMult = mildDurationMult;
                break;
            case LucidityLevel.Moderate:
                intensityMult = moderateIntensityMult;
                durationMult = moderateDurationMult;
                break;
            case LucidityLevel.Severe:
                intensityMult = severeIntensityMult;
                durationMult = severeDurationMult;
                break;
            default: // Safe
                intensityMult = 0f;
                durationMult = 0f;
                break;
        }
    }
    
    /**
     * Récupère l'intervalle pour le niveau actuel
     */
    private float GetIntervalForLevel(LucidityLevel level)
    {
        switch (level)
        {
            case LucidityLevel.Mild:
                return mildInterval;
            case LucidityLevel.Moderate:
                return moderateInterval;
            case LucidityLevel.Severe:
                return severeInterval;
            default: // Safe - pas d'events donc intervalle max
                return 9999f;
        }
    }
    
    #endregion Lucidity Level

    #region Gauge Management
    
    /**
     * Met à jour tous les paramètres en fonction de la jauge actuelle
     */
    private void UpdateAllFromGauge()
    {
        previousLevel = currentLevel;
        currentLevel = CalculateLucidityLevel(gauge);
        
        // Si on passe en mode Safe (>70%), arrêter tous les events actifs
        if (currentLevel == LucidityLevel.Safe)
        {
            HandleSafeMode();
        }
        else
        {
            HandleActiveMode();
        }
        
        // Log si le niveau a changé
        if (previousLevel != currentLevel)
        {
            Debug.Log($"[S_LucidityGauge] Level changed: {previousLevel} -> {currentLevel} (gauge: {gauge}%)");
        }
    }
    
    /**
     * Gère le mode Safe (>70%) - arrête tout
     */
    private void HandleSafeMode()
    {
        if (alzheimerEventsManager == null) return;
        
        // Arrêter la boucle d'events
        if (alzheimerEventsManager.IsEventLoopActive())
        {
            alzheimerEventsManager.StopEventLoop();
            Debug.Log("[S_LucidityGauge] Safe mode - Event loop stopped");
        }
        
        // Arrêter tous les events actifs
        if (alzheimerEventsManager.ActiveEventsCount > 0)
        {
            alzheimerEventsManager.StopAllActiveEvents();
            Debug.Log("[S_LucidityGauge] Safe mode - All active events stopped");
        }
    }
    
    /**
     * Gère les modes actifs (<70%) - ajuste les paramètres
     */
    private void HandleActiveMode()
    {
        if (alzheimerEventsManager == null) return;
        
        // S'assurer que la boucle est active
        if (!alzheimerEventsManager.IsEventLoopActive())
        {
            alzheimerEventsManager.StartEventLoop();
            Debug.Log($"[S_LucidityGauge] Event loop restarted (level: {currentLevel})");
        }
        
        // Mettre à jour l'intervalle
        float interval = GetIntervalForLevel(currentLevel);
        alzheimerEventsManager.SetActivationInterval(interval);
        
        // Mettre à jour les configs d'events
        UpdateEventConfigs();
    }
    
    /**
     * Met à jour toutes les configurations d'events
     */
    private void UpdateEventConfigs()
    {
        GetMultipliersForLevel(currentLevel, out float intensityMult, out float durationMult);
        
        foreach (var config in eventConfigs)
        {
            config.UpdateFromLucidityLevel(currentLevel, gauge, intensityMult, durationMult);
        }
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
    
    public S_EventLucidityConfig GetConfigByName(string eventName)
    {
        return eventConfigs.Find(c => c.alzheimerEvent != null && c.alzheimerEvent.eventName == eventName);
    }
    
    public S_EventLucidityConfig GetConfig(SO_AlzheimerEvent alzheimerEvent)
    {
        return eventConfigs.Find(c => c.alzheimerEvent == alzheimerEvent);
    }
    
    public void ResetAllEventsToBase()
    {
        foreach (var config in eventConfigs)
        {
            config.ResetToBase();
        }
    }
    
    public List<S_EventLucidityConfig> GetAllConfigs()
    {
        return eventConfigs;
    }
    
    #endregion Event Config Access

    #region Debug Methods

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

    [ContextMenu("Debug - Show Status")]
    void DebugShowStatus()
    {
        Debug.Log("=== LUCIDITY GAUGE STATUS ===");
        Debug.Log($"Gauge: {gauge}%");
        Debug.Log($"Level: {currentLevel}");
        Debug.Log($"Interval: {GetIntervalForLevel(currentLevel)}s");
        GetMultipliersForLevel(currentLevel, out float intMult, out float durMult);
        Debug.Log($"Intensity Mult: {intMult}x, Duration Mult: {durMult}x");
        if (alzheimerEventsManager != null)
        {
            Debug.Log($"Event Loop Active: {alzheimerEventsManager.IsEventLoopActive()}");
            Debug.Log($"Active Events: {alzheimerEventsManager.ActiveEventsCount}");
        }
        Debug.Log("==============================");
    }

    [ContextMenu("Debug - Show Configs")]
    void DebugShowConfigs()
    {
        Debug.Log("=== EVENT CONFIGS ===");
        foreach (var config in eventConfigs)
        {
            if (config.alzheimerEvent != null)
            {
                Debug.Log($"Event: {config.alzheimerEvent.eventName}");
                Debug.Log($"  - Weight: {config.CurrentWeight} (base: {config.GetBaseWeight()})");
                Debug.Log($"  - Duration: {config.CurrentDuration}s (base: {config.GetBaseDuration()})");
                Debug.Log($"  - Intensity: {config.CurrentIntensity} (base: {config.GetBaseIntensity()})");
            }
        }
    }

    [ContextMenu("Debug - Set Gauge 80% (Safe)")]
    void DebugSetGaugeSafe()
    {
        SetGauge(80f);
        Debug.Log($"[S_LucidityGauge] Gauge: {gauge}% -> Level: {currentLevel}");
    }
    
    [ContextMenu("Debug - Set Gauge 55% (Mild)")]
    void DebugSetGaugeMild()
    {
        SetGauge(55f);
        Debug.Log($"[S_LucidityGauge] Gauge: {gauge}% -> Level: {currentLevel}");
    }
    
    [ContextMenu("Debug - Set Gauge 30% (Moderate)")]
    void DebugSetGaugeModerate()
    {
        SetGauge(30f);
        Debug.Log($"[S_LucidityGauge] Gauge: {gauge}% -> Level: {currentLevel}");
    }
    
    [ContextMenu("Debug - Set Gauge 10% (Severe)")]
    void DebugSetGaugeSevere()
    {
        SetGauge(10f);
        Debug.Log($"[S_LucidityGauge] Gauge: {gauge}% -> Level: {currentLevel}");
    }

    [ContextMenu("Debug - Decrease (10)")]
    void DebugDecrease()
    {
        DecreaseGauge(10f);
        Debug.Log($"[S_LucidityGauge] After decrease: {gauge}% -> Level: {currentLevel}");
    }

    [ContextMenu("Debug - Increase (10)")]
    void DebugIncrease()
    {
        IncreaseGauge(10f);
        Debug.Log($"[S_LucidityGauge] After increase: {gauge}% -> Level: {currentLevel}");
    }
    
    [ContextMenu("Debug - Force Stop All Events")]
    void DebugForceStopAll()
    {
        if (alzheimerEventsManager != null)
        {
            alzheimerEventsManager.StopAllActiveEvents();
        }
    }

    #endregion Debug Methods
}