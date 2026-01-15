using UnityEngine;

public class MenuBackgroundShaderController : MonoBehaviour
{
    [SerializeField] private Material backgroundMaterial;

    [Header("Noise")]
    [SerializeField] private float baseNoiseScale = 2.0f;
    [SerializeField] private float hoverNoiseScale = 2.6f;
    [SerializeField] private float smoothSpeed = 3.0f;

    private float current;
    private float target;

    void Awake()
    {
        backgroundMaterial = Instantiate(backgroundMaterial);
        current = baseNoiseScale;
        target = baseNoiseScale;
        backgroundMaterial.SetFloat("_NoiseScale", baseNoiseScale);
    }

    void Update()
    {
        current = Mathf.Lerp(current, target, Time.deltaTime * smoothSpeed);
        backgroundMaterial.SetFloat("_NoiseScale", current);
    }

    public void OnHoverEnter() => target = hoverNoiseScale;
    public void OnHoverExit()  => target = baseNoiseScale;
}
