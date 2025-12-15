
using UnityEngine;

/**
 * Configuration d'un event Alzheimer par rapport à la jauge de lucidité
 * Permet de personnaliser comment chaque event réagit à la baisse de lucidité
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.1.0	Monday, December 15th, 2025.
 */
[System.Serializable]
public class S_EventLucidityConfig
{
    #region Variables

    [Header("Event Reference")]
    [Tooltip("L'event Alzheimer concerné")]
    public SO_AlzheimerEvent alzheimerEvent;
    
    [Header("Weight Settings")]
    [Tooltip("Si true, utilise le poids de base du SO comme poids minimum")]
    public bool useBaseWeightAsMin = true;
    
    [Tooltip("Poids minimum de l'event (quand lucidité = 100) - Ignoré si useBaseWeightAsMin = true")]
    [Min(0.1f)] public float minWeight = 1f;
    
    [Tooltip("Poids maximum de l'event (quand lucidité = 0)")]
    [Min(0.1f)] public float maxWeight = 10f;
    
    [Header("Duration Settings")]
    [Tooltip("Multiplicateur de durée minimum (quand lucidité = 100)")]
    [Min(0.1f)] public float minDurationMultiplier = 1f;
    
    [Tooltip("Multiplicateur de durée maximum (quand lucidité = 0)")]
    [Min(0.1f)] public float maxDurationMultiplier = 3f;
    
    [Header("Intensity Settings")]
    [Tooltip("Multiplicateur d'intensité minimum (quand lucidité = 100)")]
    [Min(0.1f)] public float minIntensityMultiplier = 1f;
    
    [Tooltip("Multiplicateur d'intensité maximum (quand lucidité = 0)")]
    [Min(0.1f)] public float maxIntensityMultiplier = 2f;
    
    [Header("Options")]
    [Tooltip("Si true, cet event ignore les modifications de la jauge")]
    public bool ignoreGaugeModifications = false;
    
    [Header("Debug (Runtime Values)")]
    [SerializeField] private float currentWeight;
    [SerializeField] private float currentDuration;
    [SerializeField] private float currentIntensity;
    
    // Valeurs de base sauvegardées au démarrage (ne modifie pas le SO)
    private float baseWeight;
    private float baseDuration;
    private float baseIntensity;
    private bool isInitialized = false;
    
    // Propriétés publiques
    public float CurrentWeight => currentWeight;
    public float CurrentDuration => currentDuration;
    public float CurrentIntensity => currentIntensity;

    #endregion Variables

    #region Methods
    
    /**
     * Sauvegarde les valeurs de base depuis le ScriptableObject
     * IMPORTANT: Appelé une seule fois au démarrage pour capturer les vraies valeurs
     */
    public void Initialize()
    {
        if (alzheimerEvent == null)
        {
            Debug.LogWarning("[S_EventLucidityConfig] No AlzheimerEvent assigned!");
            return;
        }
        
        // Capturer les valeurs de base SEULEMENT si pas encore initialisé
        // Cela évite de capturer des valeurs déjà modifiées
        if (!isInitialized)
        {
            baseWeight = alzheimerEvent.eventBaseWeight;
            baseDuration = alzheimerEvent.eventDuration;
            baseIntensity = alzheimerEvent.eventIntensity;
            
            // Vérifier si les valeurs de base sont valides
            if (baseWeight <= 0)
            {
                Debug.LogWarning($"[S_EventLucidityConfig] Event '{alzheimerEvent.eventName}' has baseWeight = {baseWeight}. Setting to 1.");
                baseWeight = 1f;
                alzheimerEvent.eventBaseWeight = 1f;
            }
            
            isInitialized = true;
            Debug.Log($"[S_EventLucidityConfig] Initialized '{alzheimerEvent.eventName}': weight={baseWeight}, duration={baseDuration}s, intensity={baseIntensity}");
        }
        
        // Calculer le min/max effectif
        float effectiveMinWeight = useBaseWeightAsMin ? baseWeight : minWeight;
        if (maxWeight < effectiveMinWeight)
        {
            maxWeight = effectiveMinWeight * 2f;
        }
        
        // Initialiser les valeurs courantes
        currentWeight = baseWeight;
        currentDuration = baseDuration;
        currentIntensity = baseIntensity;
    }
    
    /**
     * Met à jour les valeurs de l'event en fonction du niveau de lucidité (0-100)
     *
     * @param	float	lucidity	Niveau de lucidité entre 0 et 100
     */
    public void UpdateFromLucidity(float lucidity)
    {
        if (alzheimerEvent == null) return;
        
        // S'assurer de l'initialisation
        if (!isInitialized)
        {
            Initialize();
        }
        
        if (ignoreGaugeModifications)
        {
            // Garder les valeurs de base
            currentWeight = baseWeight;
            currentDuration = baseDuration;
            currentIntensity = baseIntensity;
            ApplyToEvent();
            return;
        }
        
        // Normaliser la lucidité (0 = max effet, 1 = min effet)
        float normalizedLucidity = Mathf.Clamp01(lucidity / 100f);
        float invertedLucidity = 1f - normalizedLucidity; // Plus c'est bas, plus c'est fort
        
        // Calculer le poids effectif minimum
        float effectiveMinWeight = useBaseWeightAsMin ? baseWeight : minWeight;
        
        // Calculer le poids actuel
        currentWeight = Mathf.Lerp(effectiveMinWeight, maxWeight, invertedLucidity);
        
        // Calculer la durée
        float durationMultiplier = Mathf.Lerp(minDurationMultiplier, maxDurationMultiplier, invertedLucidity);
        currentDuration = baseDuration * durationMultiplier;
        
        // Calculer l'intensité
        float intensityMultiplier = Mathf.Lerp(minIntensityMultiplier, maxIntensityMultiplier, invertedLucidity);
        currentIntensity = baseIntensity * intensityMultiplier;
        
        // Appliquer au SO
        ApplyToEvent();
    }
    
    /**
     * Applique les valeurs calculées à l'event
     */
    private void ApplyToEvent()
    {
        if (alzheimerEvent == null) return;
        
        alzheimerEvent.eventBaseWeight = currentWeight;
        alzheimerEvent.eventDuration = currentDuration;
        alzheimerEvent.eventIntensity = currentIntensity;
    }
    
    /**
     * Remet les valeurs de base de l'event
     */
    public void ResetToBase()
    {
        if (alzheimerEvent == null || !isInitialized) return;
        
        currentWeight = baseWeight;
        currentDuration = baseDuration;
        currentIntensity = baseIntensity;
        ApplyToEvent();
        
        Debug.Log($"[S_EventLucidityConfig] Reset '{alzheimerEvent.eventName}' to base values");
    }
    
    /**
     * Modifie manuellement le poids de l'event
     *
     * @param	float	newWeight	Nouveau poids
     */
    public void SetWeight(float newWeight)
    {
        currentWeight = Mathf.Max(0.1f, newWeight);
        if (alzheimerEvent != null)
        {
            alzheimerEvent.eventBaseWeight = currentWeight;
        }
    }
    
    /**
     * Modifie les limites de poids min/max
     *
     * @param	float	min	Poids minimum
     * @param	float	max	Poids maximum
     */
    public void SetWeightBounds(float min, float max)
    {
        minWeight = Mathf.Max(0.1f, min);
        maxWeight = Mathf.Max(minWeight, max);
    }
    
    /**
     * Récupère les valeurs de base originales
     */
    public float GetBaseWeight() => baseWeight;
    public float GetBaseDuration() => baseDuration;
    public float GetBaseIntensity() => baseIntensity;

    #endregion Methods
}