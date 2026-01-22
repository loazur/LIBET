using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(
    fileName = "PP_AlzheimerEffect",
    menuName = "Alzheimer/PostProcess Effect")]
public class SO_AlzheimerPostProcessEffect : ScriptableObject
{
    public enum ParameterType
    {
        VignetteIntensity,
        Exposure,
        Saturation,
        Contrast,
        BloomIntensity
    }

    public ParameterType parameter;

    [Tooltip("Valeur cible du paramètre")]
    public float targetValue = 1f;

    [Tooltip("Durée totale (aller + retour)")]
    public float duration = 2f;

    [Tooltip("Courbe d'évolution (0→1)")]
    public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Tooltip("Multiplier appliqué selon l'intensité de l'event")]
    public float intensityMultiplier = 1f;
}
