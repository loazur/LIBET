using UnityEngine;
using UnityEngine.Rendering;
using Assert = UnityEngine.Assertions.Assert;

public class S_DayNight : MonoBehaviour
{
    [Header("Références")]
    public Light directionalLight;
    public Material skyboxMaterial; // Assigner le material Skybox/Procedural
    
    [Header("Cycle")]
    public float dayLength = 120f; // Length of a full day in seconds
    private float time;

    [Header("Couleurs du ciel")]
    [Tooltip("Couleur du ciel à midi")]
    public Color daySkyTint = new Color(0.5f, 0.5f, 0.5f, 1f);
    [Tooltip("Couleur du ciel au lever/coucher du soleil")]
    public Color sunsetSkyTint = new Color(1f, 0.5f, 0.3f, 1f);
    [Tooltip("Couleur du ciel la nuit")]
    public Color nightSkyTint = new Color(0.1f, 0.1f, 0.2f, 1f);
    
    [Header("Couleurs de la lumière")]
    public Color dayLightColor = new Color(1f, 0.95f, 0.85f, 1f);
    public Color sunsetLightColor = new Color(1f, 0.6f, 0.3f, 1f);
    public Color nightLightColor = new Color(0.3f, 0.3f, 0.5f, 1f);

    [Header("Atmosphère")]
    [Range(0f, 5f)]
    public float dayAtmosphereThickness = 1f;
    [Range(0f, 5f)]
    public float sunsetAtmosphereThickness = 2f;
    [Range(0f, 5f)]
    public float nightAtmosphereThickness = 0.5f;

    // convertir time en heures et minutes pour affichage si besoin

    /**
     * Au lancement, le jour est instancié par défaut
     *
     * @return	void
     */
    void Start()
    {
        Assert.IsNotNull(directionalLight, "Aucune lumière directionnelle assignée pour le cycle jour/nuit.");

        // Si pas de skybox assigné, essayer de récupérer celui de RenderSettings
        if (skyboxMaterial == null)
        {
            skyboxMaterial = RenderSettings.skybox;
        }

        // Par défaut, démarrer en jour
        StartDay();
    }

    /**
     * Change la lumière au fur et à mesure de la journée pour faire un cycle jour/nuit
     *
     * @return	void
     */
    void Update()
    {
        // Increment time
        time += Time.deltaTime / dayLength;
        time %= 1; // Keep time in range [0, 1]
        // Apply lighting/rotation for the current time
        UpdateLighting(time);

    }

    /**
     * Met à jour la rotation du soleil et l'intensité de la lumière en fonction du temps (0..1).
     *
     * @access	private
     * @param	mixed	float
     * @return	void
     */
    private void UpdateLighting(float t)
    {
        // Applique la rotation du soleil pour simuler un vrai soleil
        float sunAngle = t * 360f - 90f;
        transform.localRotation = Quaternion.Euler(sunAngle, 170f, 0f);

        if (directionalLight == null)
            return;

        // Calcul des phases de la journée
        float intensity;
        Color lightColor;
        Color skyTint;
        float atmosphereThickness;

        // Nuit profonde (0.0 - 0.20 et 0.80 - 1.0)
        if (t <= 0.20f || t >= 0.80f)
        {
            intensity = 0.1f;
            lightColor = nightLightColor;
            skyTint = nightSkyTint;
            atmosphereThickness = nightAtmosphereThickness;
        }
        // Lever du soleil (0.20 - 0.30)
        else if (t <= 0.30f)
        {
            float blend = (t - 0.20f) * 10f; // 0 à 1
            intensity = Mathf.Lerp(0.1f, 1f, blend);
            lightColor = Color.Lerp(nightLightColor, sunsetLightColor, blend);
            skyTint = Color.Lerp(nightSkyTint, sunsetSkyTint, blend);
            atmosphereThickness = Mathf.Lerp(nightAtmosphereThickness, sunsetAtmosphereThickness, blend);
        }
        // Transition lever → jour (0.30 - 0.40)
        else if (t <= 0.40f)
        {
            float blend = (t - 0.30f) * 10f;
            intensity = 1f;
            lightColor = Color.Lerp(sunsetLightColor, dayLightColor, blend);
            skyTint = Color.Lerp(sunsetSkyTint, daySkyTint, blend);
            atmosphereThickness = Mathf.Lerp(sunsetAtmosphereThickness, dayAtmosphereThickness, blend);
        }
        // Journée (0.40 - 0.60)
        else if (t <= 0.60f)
        {
            intensity = 1f;
            lightColor = dayLightColor;
            skyTint = daySkyTint;
            atmosphereThickness = dayAtmosphereThickness;
        }
        // Transition jour → coucher (0.60 - 0.70)
        else if (t <= 0.70f)
        {
            float blend = (t - 0.60f) * 10f;
            intensity = 1f;
            lightColor = Color.Lerp(dayLightColor, sunsetLightColor, blend);
            skyTint = Color.Lerp(daySkyTint, sunsetSkyTint, blend);
            atmosphereThickness = Mathf.Lerp(dayAtmosphereThickness, sunsetAtmosphereThickness, blend);
        }
        // Coucher du soleil (0.70 - 0.80)
        else
        {
            float blend = (t - 0.70f) * 10f;
            intensity = Mathf.Lerp(1f, 0.1f, blend);
            lightColor = Color.Lerp(sunsetLightColor, nightLightColor, blend);
            skyTint = Color.Lerp(sunsetSkyTint, nightSkyTint, blend);
            atmosphereThickness = Mathf.Lerp(sunsetAtmosphereThickness, nightAtmosphereThickness, blend);
        }

        // Appliquer à la lumière
        directionalLight.intensity = intensity;
        directionalLight.color = lightColor;

        // Appliquer au skybox (si c'est un Skybox/Procedural)
        UpdateSkybox(skyTint, atmosphereThickness);
    }

    /**
     * Met à jour le matériau du skybox procédural
     */
    private void UpdateSkybox(Color skyTint, float atmosphereThickness)
    {
        if (skyboxMaterial == null) return;

        // Propriétés du shader Skybox/Procedural
        if (skyboxMaterial.HasProperty("_SkyTint"))
        {
            skyboxMaterial.SetColor("_SkyTint", skyTint);
        }
        if (skyboxMaterial.HasProperty("_AtmosphereThickness"))
        {
            skyboxMaterial.SetFloat("_AtmosphereThickness", atmosphereThickness);
        }
        if (skyboxMaterial.HasProperty("_Exposure"))
        {
            // Réduire l'exposition la nuit
            float exposure = Mathf.Lerp(0.5f, 1.3f, directionalLight.intensity);
            skyboxMaterial.SetFloat("_Exposure", exposure);
        }
    }

    /**
     * Forcer le démarrage en mode jour (milieu de journée).
     * Appeler depuis d'autres scripts ou via l'Inspector (si attaché).
     *
     * @access	public
     * @return	void
     */
    public void StartDay()
    {
        time = 0.25f; // midi approximatif
        UpdateLighting(time);
    }

    /**
     * Forcer le démarrage en mode nuit (minuit).
     *
     * @access	public
     * @return	void
     */
    public void StartNight()
    {
        time = 0.75f; // minuit
        UpdateLighting(time);
    }

    /**
     * Choisir l'heure du jour (entre 0 et 1 - 0.25 à 0.75 jour, 0.75 à 0.25 nuit)
     *
     * @access	public
     * @param	float	newTime	
     * @return	mixed
     */
    public float SetTime(float newTime)
    {
        if (newTime < 0f || newTime > 1f)
        {
            Debug.LogError("Le temps doit être compris entre 0 et 1", this);
            return newTime;
        }

        // Met à jour la variable interne puis applique l'éclairage
        time = newTime;
        UpdateLighting(time);
        return time;
    }

    
    /**
     * Convertit une valeur de temps normalisée (0..1) en heure/minute (format 24h).
     * Renvoie un Vector2Int(x=hour, y=minute).
     *
     * @access	public
     * @param	mixed	floa	
     * @return	mixed
     */
    public Vector2Int TimeToHourMinute(float t)
    {
        int minutesInDay = 1440; // nb minutes dans une journée
        
        int totalMinutes = Mathf.FloorToInt(Mathf.Repeat(t, 1f) * minutesInDay); // Mathf.Repeat garantit que totalMinutes soit entre 0 et 1
        int hour = totalMinutes / 60;
        int minute = totalMinutes % 60;
        return new Vector2Int(hour, minute);
    }


    /**
     * Renvoie l'heure et la minute courantes basées sur la variable interne `time`.
     *
     * @access	public
     * @return	mixed
     */
    public Vector2Int GetCurrentHourMinute()
    {
        return TimeToHourMinute(time);
    }

    /**
     * Retourne une chaîne formatée "HH:MM" pour un temps normalisé donné.
     *
     * @access	public
     * @param	mixed	float
     * @return	mixed
     */
    public string GetTimeString(float t)
    {
        Vector2Int hm = TimeToHourMinute(t);
        return hm.x.ToString("D2") + ":" + hm.y.ToString("D2");
    }

    /**
     * Retourne la chaîne "HH:MM" pour le temps courant.
     *
     * @access	public
     * @return	mixed
     */
    public string GetCurrentTimeString()
    {
        Vector2Int hm = TimeToHourMinute(time);
        return hm.x.ToString("D2") + ":" + hm.y.ToString("D2");
    }

}