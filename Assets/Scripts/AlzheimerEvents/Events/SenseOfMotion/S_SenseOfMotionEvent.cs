using UnityEngine;

/// <summary>
/// Change le FOV et la vitesse du joueur pour donner l'illusion qu'il n'avance pas
/// L'intensité de l'effet dépend du niveau de lucidité
/// </summary>
public class S_SenseOfMotionEvent : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Référence à l'event ScriptableObject pour accéder à l'intensité")]
    [SerializeField] private SO_AlzheimerEvent eventData;

    [Header("Paramètres FOV")]
    [Tooltip("FOV minimum à appliquer")]
    [SerializeField] private float minFOV = 90f;
    
    [Tooltip("FOV maximum à appliquer (quand intensité max)")]
    [SerializeField] private float maxFOV = 130f;

    [Tooltip("Vitesse de transition du FOV")]
    [SerializeField] private float fovTransitionSpeed = 2f;

    private float originalFOV;
    private float targetFOV;
    private bool isTransitioning = false;

    void OnEnable()
    {
        ApplyEffect();
    }

    void OnDisable()
    {
        ResetEffect();
    }

    void OnDestroy()
    {
        ResetEffect();
    }

    void Update()
    {
        // Met à jour l'effet en fonction de l'intensité actuelle
        if (eventData != null && isTransitioning)
        {
            UpdateFOVBasedOnIntensity();
        }
    }

    private void ApplyEffect()
    {
        if (S_CameraSettingsData.instance == null)
        {
            Debug.LogWarning("[SenseOfMotion] S_CameraSettingsData.instance est null!");
            return;
        }

        // Calcule le FOV basé sur l'intensité
        float intensity = eventData != null ? eventData.currentIntensity : 1f;
        targetFOV = Mathf.Lerp(minFOV, maxFOV, Mathf.Clamp01(intensity / 5f)); // Intensité max = 5 pour FOV max

        S_CameraSettingsData.instance.setCurrentFieldOfView(targetFOV);
        isTransitioning = true;

        Debug.Log($"<color=cyan>[SenseOfMotion]</color> Effet appliqué - FOV: {targetFOV:F0} | Intensité: {intensity:F2}");
    }

    private void ResetEffect()
    {
        if (S_CameraSettingsData.instance != null)
        {
            S_CameraSettingsData.instance.resetCurrentFieldOfView();
            isTransitioning = false;
            Debug.Log("<color=gray>[SenseOfMotion]</color> Effet désactivé - FOV restauré");
        }
    }

    private void UpdateFOVBasedOnIntensity()
    {
        if (S_CameraSettingsData.instance == null || eventData == null) return;

        // Recalcule le FOV si l'intensité a changé
        float intensity = eventData.currentIntensity;
        float newTargetFOV = Mathf.Lerp(minFOV, maxFOV, Mathf.Clamp01(intensity / 5f));

        if (Mathf.Abs(newTargetFOV - targetFOV) > 0.5f)
        {
            targetFOV = newTargetFOV;
            S_CameraSettingsData.instance.setCurrentFieldOfView(targetFOV);
        }
    }

}
