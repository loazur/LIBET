using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackgroundPulse : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Image targetImage;

    [Header("Timing")]
    [SerializeField] private float pulseDuration = 0.8f;
    [SerializeField] private float attackRatio = 0.4f;
    [SerializeField] private float delayBeforePulse = 0f;

    [Header("Pulse Strength")]
    [SerializeField] private Vector2 pulseStrengthRange = new Vector2(0.6f, 1.2f);

    [Header("Organic Variations")]
    [SerializeField] private Vector2 pulseNoiseScaleRange = new Vector2(1.2f, 2.0f);
    [SerializeField] private Vector2 pulseDesyncRange = new Vector2(0.3f, 1.0f);
    [SerializeField] private Vector2 pulseIntensityVarRange = new Vector2(0.8f, 1.6f);

    [Header("Curve")]
    [SerializeField] private AnimationCurve pulseCurve =
        AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Material runtimeMat;
    private Coroutine pulseRoutine;

    void Awake()
    {
        runtimeMat = Instantiate(targetImage.material);
        targetImage.material = runtimeMat;

        runtimeMat.SetFloat("_PulseStrength", 0f);
    }

    public void TriggerPulse()
    {
        if (pulseRoutine != null)
            StopCoroutine(pulseRoutine);

        pulseRoutine = StartCoroutine(PulseOnce());
    }

    IEnumerator PulseOnce()
    {
        if (delayBeforePulse > 0f)
            yield return new WaitForSeconds(delayBeforePulse);

        // ============================
        // RANDOMISATION PAR PULSE
        // ============================

        float pulseStrengthMax = Random.Range(
            pulseStrengthRange.x,
            pulseStrengthRange.y
        );

        float pulseNoiseScale = Random.Range(
            pulseNoiseScaleRange.x,
            pulseNoiseScaleRange.y
        );

        float pulseDesync = Random.Range(
            pulseDesyncRange.x,
            pulseDesyncRange.y
        );

        float pulseIntensityVar = Random.Range(
            pulseIntensityVarRange.x,
            pulseIntensityVarRange.y
        );

        runtimeMat.SetFloat("_PulseNoiseScale", pulseNoiseScale);
        runtimeMat.SetFloat("_PulseDesync", pulseDesync);
        runtimeMat.SetFloat("_PulseIntensityVar", pulseIntensityVar);

        // ============================
        // TIMING
        // ============================

        float attackTime = pulseDuration * Mathf.Clamp01(attackRatio);
        float releaseTime = pulseDuration - attackTime;

        float t = 0f;

        // --- ATTACK ---
        while (t < attackTime)
        {
            t += Time.deltaTime;
            float n = pulseCurve.Evaluate(t / attackTime);
            runtimeMat.SetFloat("_PulseStrength", n * pulseStrengthMax);
            yield return null;
        }

        t = 0f;

        // --- RELEASE ---
        while (t < releaseTime)
        {
            t += Time.deltaTime;
            float n = 1f - pulseCurve.Evaluate(t / releaseTime);
            runtimeMat.SetFloat("_PulseStrength", n * pulseStrengthMax);
            yield return null;
        }

        runtimeMat.SetFloat("_PulseStrength", 0f);
    }
}
