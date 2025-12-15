
using System.Collections.Generic;
using UnityEngine;

class S_EventLucidityConfig : MonoBehaviour
{
    #region Variables

    [Header("Event Reference")]
    [Tooltip("L'event Alzheimer concerné")]
    public SO_AlzheimerEvent alzheimerEvent;
    
    [Header("Weight Settings")]
    [Tooltip("Poids minimum de l'event (quand lucidité = 100)")]
    [Min(0)] public float minWeight = 0f;
    
    [Tooltip("Poids maximum de l'event (quand lucidité = 0)")]
    [Min(0)] public float maxWeight = 10f;
    
    [Tooltip("Poids actuel (modifié dynamiquement)")]
    [Min(0)] public float currentWeight;
    
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
    
    // Valeurs de base sauvegardées pour pouvoir les restaurer
    [HideInInspector] public float baseWeight;
    [HideInInspector] public float baseDuration;
    [HideInInspector] public float baseIntensity;
    [HideInInspector] public bool isInitialized = false;

    #endregion Variables


    
}