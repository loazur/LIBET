using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class S_AlzheimerPostProcessManager : MonoBehaviour
{

    #if UNITY_EDITOR
        [ContextMenu("DEBUG / Play Random Effect")]
        private void DebugPlayRandom()
        {
            PlayRandomEffect(0.5f);
        }
        #endif

    public static S_AlzheimerPostProcessManager instance;

    [Header("Post Process")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private List<SO_AlzheimerPostProcessEffect> effects;

    private VolumeProfile profile;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        if (globalVolume == null)
        {
            Debug.LogError("[PP] GlobalVolume NON ASSIGNÉ");
            return;
        }

        profile = globalVolume.profile;

        if (profile == null)
        {
            Debug.LogError("[PP] VolumeProfile NULL");
        }
        else
        {
            Debug.Log("[PP] VolumeProfile OK : " + profile.name);
        }
    }


    public void PlayRandomEffect(float intensity)
    {
        Debug.Log($"[PP] PlayRandomEffect appelé | intensity = {intensity}");

        if (effects == null || effects.Count == 0)
        {
            Debug.LogWarning("[PP] Aucun effet configuré");
            return;
        }

        var effect = effects[Random.Range(0, effects.Count)];
        Debug.Log($"[PP] Effet choisi : {effect.name} | param = {effect.parameter}");

        StartCoroutine(PlayEffectCoroutine(effect, intensity));
    }


    private IEnumerator PlayEffectCoroutine(
        SO_AlzheimerPostProcessEffect effect,
        float intensity)
    {
        if (effect == null)
        {
            Debug.LogError("[PP] Effect NULL");
            yield break;
        }

        float halfDuration = effect.duration * 0.5f;

        Debug.Log($"[PP] Coroutine START | duration={effect.duration}");

        switch (effect.parameter)
        {
            case SO_AlzheimerPostProcessEffect.ParameterType.VignetteIntensity:
                if (!profile.TryGet(out Vignette vignette))
                {
                    Debug.LogError("[PP] Vignette NON TROUVÉE dans le VolumeProfile");
                    yield break;
                }

                Debug.Log($"[PP] Vignette trouvée | valeur actuelle = {vignette.intensity.value}");

                yield return AnimateFloat(
                    vignette.intensity,
                    vignette.intensity.value,
                    effect.targetValue * effect.intensityMultiplier * intensity,
                    halfDuration,
                    effect.curve
                );
                break;

            case SO_AlzheimerPostProcessEffect.ParameterType.Exposure:
                if (!profile.TryGet(out ColorAdjustments color))
                {
                    Debug.LogError("[PP] ColorAdjustments NON TROUVÉ");
                    yield break;
                }

                Debug.Log($"[PP] Exposure trouvée | valeur actuelle = {color.postExposure.value}");

                yield return AnimateFloat(
                    color.postExposure,
                    color.postExposure.value,
                    effect.targetValue * effect.intensityMultiplier * intensity,
                    halfDuration,
                    effect.curve
                );
                break;

            case SO_AlzheimerPostProcessEffect.ParameterType.Saturation:
                if (profile.TryGet(out ColorAdjustments colorSat))
                {
                    yield return AnimateFloat(
                        colorSat.saturation,
                        colorSat.saturation.value,
                        effect.targetValue * effect.intensityMultiplier * intensity,
                        halfDuration,
                        effect.curve
                    );
                }
                break;

            case SO_AlzheimerPostProcessEffect.ParameterType.Contrast:
                if (profile.TryGet(out ColorAdjustments colorCon))
                {
                    yield return AnimateFloat(
                        colorCon.contrast,
                        colorCon.contrast.value,
                        effect.targetValue * effect.intensityMultiplier * intensity,
                        halfDuration,
                        effect.curve
                    );
                }
                break;

                    }

                    Debug.Log("[PP] Coroutine END");
                }


    private IEnumerator AnimateFloat(
        FloatParameter parameter,
        float startValue,
        float targetValue,
        float duration,
        AnimationCurve curve)
    {
        if (parameter == null)
        {
            Debug.LogError("[PP] FloatParameter NULL");
            yield break;
        }

        Debug.Log($"[PP] AnimateFloat START | {startValue} → {targetValue} | duration={duration}");

        parameter.overrideState = true;

        float t = 0f;
        while (t < duration)
        {
            float normalized = t / duration;
            parameter.value = Mathf.Lerp(
                startValue,
                targetValue,
                curve.Evaluate(normalized)
            );
            t += Time.deltaTime;
            yield return null;
        }

        t = 0f;
        while (t < duration)
        {
            float normalized = t / duration;
            parameter.value = Mathf.Lerp(
                targetValue,
                startValue,
                curve.Evaluate(normalized)
            );
            t += Time.deltaTime;
            yield return null;
        }

        parameter.value = startValue;
        parameter.overrideState = false;

        Debug.Log("[PP] AnimateFloat END");
    }

}
