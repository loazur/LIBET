using UnityEngine;

/// <summary>
/// Contrôle simple d'une lumière intérieure pour simuler le passage du temps
/// Sans avoir besoin d'un vrai soleil/skybox
/// </summary>
public class S_IndoorLightController : MonoBehaviour
{
    [Header("Référence")]
    public Light spotLight;
    
    [Header("Couleurs selon l'heure")]
    public Color morningColor = new Color(1f, 0.9f, 0.7f);      // Jaune doux
    public Color middayColor = new Color(1f, 1f, 0.95f);        // Blanc chaud
    public Color eveningColor = new Color(1f, 0.6f, 0.3f);      // Orange coucher
    public Color nightColor = new Color(0.3f, 0.3f, 0.5f);      // Bleu nuit (très faible)
    
    [Header("Intensité selon l'heure")]
    public float morningIntensity = 1.5f;
    public float middayIntensity = 2.5f;
    public float eveningIntensity = 2f;
    public float nightIntensity = 0.1f;
    
    [Header("Temps")]
    [Range(0f, 1f)]
    public float timeOfDay = 0.5f;  // 0 = minuit, 0.25 = 6h, 0.5 = midi, 0.75 = 18h
    public bool animateTime = false;
    public float dayDuration = 120f; // Durée d'un jour en secondes
    
    private void Start()
    {
        if (spotLight == null)
            spotLight = GetComponent<Light>();
    }
    
    private void Update()
    {
        if (animateTime)
        {
            timeOfDay += Time.deltaTime / dayDuration;
            timeOfDay %= 1f;
        }
        
        UpdateLight();
    }
    
    private void UpdateLight()
    {
        if (spotLight == null) return;
        
        Color targetColor;
        float targetIntensity;
        
        // Matin (0.2 - 0.35)
        if (timeOfDay >= 0.2f && timeOfDay < 0.35f)
        {
            float t = (timeOfDay - 0.2f) / 0.15f;
            targetColor = Color.Lerp(nightColor, morningColor, t);
            targetIntensity = Mathf.Lerp(nightIntensity, morningIntensity, t);
        }
        // Matin → Midi (0.35 - 0.5)
        else if (timeOfDay >= 0.35f && timeOfDay < 0.5f)
        {
            float t = (timeOfDay - 0.35f) / 0.15f;
            targetColor = Color.Lerp(morningColor, middayColor, t);
            targetIntensity = Mathf.Lerp(morningIntensity, middayIntensity, t);
        }
        // Midi (0.5 - 0.65)
        else if (timeOfDay >= 0.5f && timeOfDay < 0.65f)
        {
            targetColor = middayColor;
            targetIntensity = middayIntensity;
        }
        // Midi → Soir (0.65 - 0.75)
        else if (timeOfDay >= 0.65f && timeOfDay < 0.75f)
        {
            float t = (timeOfDay - 0.65f) / 0.1f;
            targetColor = Color.Lerp(middayColor, eveningColor, t);
            targetIntensity = Mathf.Lerp(middayIntensity, eveningIntensity, t);
        }
        // Soir → Nuit (0.75 - 0.85)
        else if (timeOfDay >= 0.75f && timeOfDay < 0.85f)
        {
            float t = (timeOfDay - 0.75f) / 0.1f;
            targetColor = Color.Lerp(eveningColor, nightColor, t);
            targetIntensity = Mathf.Lerp(eveningIntensity, nightIntensity, t);
        }
        // Nuit
        else
        {
            targetColor = nightColor;
            targetIntensity = nightIntensity;
        }
        
        spotLight.color = targetColor;
        spotLight.intensity = targetIntensity;
    }
    
    // Méthodes publiques pour contrôler depuis d'autres scripts
    public void SetMorning() { timeOfDay = 0.3f; UpdateLight(); }
    public void SetMidday() { timeOfDay = 0.5f; UpdateLight(); }
    public void SetEvening() { timeOfDay = 0.7f; UpdateLight(); }
    public void SetNight() { timeOfDay = 0f; UpdateLight(); }
}
