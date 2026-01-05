using UnityEngine;

/// <summary>
/// Supprime les ombres dans le jeu pour que les joueurs ne puissent vraiment pas
/// évaluer les distances (DepthPerceptionShadowLoss)
/// </summary>
public class S_DepthPerceptionShadowLoss : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Référence à l'event ScriptableObject (optionnel, pour accéder à l'intensité)")]
    [SerializeField] private SO_AlzheimerEvent eventData;

    // Stockage des ombres originales pour restauration
    private System.Collections.Generic.Dictionary<Light, LightShadows> originalShadows = 
        new System.Collections.Generic.Dictionary<Light, LightShadows>();

    void OnEnable()
    {
        DisableAllShadows();
    }

    void OnDisable()
    {
        RestoreAllShadows();
    }

    void OnDestroy()
    {
        RestoreAllShadows();
    }

    /// <summary>
    /// Désactive toutes les ombres dans la scène
    /// </summary>
    private void DisableAllShadows()
    {
        originalShadows.Clear();
        
        foreach (var light in Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            // Sauvegarde l'état original
            originalShadows[light] = light.shadows;
            light.shadows = LightShadows.None;
        }

        Debug.Log($"<color=cyan>[DepthPerception]</color> Ombres désactivées ({originalShadows.Count} lumières)");
    }

    /// <summary>
    /// Restaure toutes les ombres à leur état original
    /// </summary>
    private void RestoreAllShadows()
    {
        foreach (var kvp in originalShadows)
        {
            if (kvp.Key != null)
            {
                kvp.Key.shadows = kvp.Value;
            }
        }
        
        Debug.Log("<color=gray>[DepthPerception]</color> Ombres restaurées");
        originalShadows.Clear();
    }


}