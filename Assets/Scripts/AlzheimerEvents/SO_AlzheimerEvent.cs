using UnityEngine;

/// <summary>
/// ScriptableObject représentant un événement Alzheimer
/// </summary>
[CreateAssetMenu(menuName = "Alzheimer/Event")]
public class SO_AlzheimerEvent : ScriptableObject
{
    //~ Informations de base
    [Header("Informations de l'event")]
    [Tooltip("Nom unique de l'event")]
    public string eventName;

    [Tooltip("Description de l'event")]
    [TextArea(2, 4)]
    public string eventDescription;

    //~ Configuration de déclenchement
    [Header("Configuration de déclenchement")]
    [Tooltip("Comment l'event s'active")]
    public ActivationType activationType = ActivationType.Random;

    [Tooltip("Poids de base pour la sélection aléatoire (plus c'est haut, plus ça a de chances d'arriver)")]
    [Min(0.1f)]
    public float baseWeight = 1f;

    [Tooltip("Si l'event ne peut s'activer qu'une seule fois par partie")]
    public bool isOneShot = false;

    [Tooltip("Palier de lucidité minimum requis pour déclencher (0-100, 0 = peut se déclencher à n'importe quel niveau bas)")]
    [Range(0, 100)]
    public float minLucidityThreshold = 0f;

    [Tooltip("Palier de lucidité maximum pour déclencher (doit être <= 60 pour les events normaux)")]
    [Range(0, 100)]
    public float maxLucidityThreshold = 60f;

    //~ Intensité et durée
    [Header("Intensité et durée")]
    [Tooltip("Intensité de base de l'event (sera modifiée par le niveau de lucidité)")]
    [Range(0.1f, 10f)]
    public float baseIntensity = 1f;

    [Tooltip("Durée de l'event en secondes (0 = permanent jusqu'à annulation)")]
    [Min(0)]
    public float duration = 0f;

    [Tooltip("Peut se cumuler avec d'autres events")]
    public bool canStack = true;

    [Tooltip("Priorité de l'event (plus c'est haut, moins ça sera annulé en premier)")]
    [Range(1, 10)]
    public int priority = 5;

    //~ Prefab
    [Header("Prefab de l'event")]
    [Tooltip("Prefab contenant la logique de l'event")]
    public GameObject eventPrefab;

    //~ État runtime (non sauvegardé)
    [HideInInspector] public bool hasTriggered = false;
    [HideInInspector] public float currentIntensity = 1f;

    public enum ActivationType
    {
        Random,         // Se lance aléatoirement selon le poids
        OnWakeUp,       // Se lance quand Libet se réveille
        OnThreshold,    // Se lance quand un palier de lucidité est atteint
        Story,          // Event de progression d'histoire (oneshot automatique)
        Manual          // Déclenché manuellement par script
    }

    /// <summary>
    /// Réinitialise l'état de l'event pour une nouvelle partie
    /// </summary>
    public void ResetState()
    {
        hasTriggered = false;
        currentIntensity = baseIntensity;
    }

    /// <summary>
    /// Vérifie si l'event peut être déclenché selon le niveau de lucidité
    /// </summary>
    public bool CanTriggerAtLucidity(float lucidity)
    {
        // Vérifie le one-shot
        if ((isOneShot || activationType == ActivationType.Story) && hasTriggered)
            return false;

        // Vérifie les paliers de lucidité
        return lucidity >= minLucidityThreshold && lucidity <= maxLucidityThreshold;
    }

    /// <summary>
    /// Calcule l'intensité ajustée selon le niveau de lucidité
    /// </summary>
    public float GetAdjustedIntensity(float lucidity, float intensityMultiplier)
    {
        // Plus la lucidité est basse, plus l'intensité est forte
        // Formule: intensité de base * multiplicateur * (1 + (60 - lucidité) / 60)
        float lucidityFactor = 1f + Mathf.Max(0, (60f - lucidity) / 60f);
        return baseIntensity * intensityMultiplier * lucidityFactor;
    }

    /// <summary>
    /// Calcule le poids ajusté selon le niveau de lucidité et le cycle
    /// </summary>
    public float GetAdjustedWeight(float lucidity, int alzheimerCycle)
    {
        // Plus la lucidité est basse, plus les chances augmentent
        float lucidityMultiplier = 1f + Mathf.Max(0, (60f - lucidity) / 30f);
        
        // Le cycle augmente aussi les chances
        float cycleMultiplier = 1f + (alzheimerCycle * 0.2f);
        
        return baseWeight * lucidityMultiplier * cycleMultiplier;
    }
}
