using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class S_DayNightManager : MonoBehaviour
{
    public static S_DayNightManager instance { get; private set; }

    [Header("Mode")]
    [Tooltip("Utiliser une Spot Light au lieu d'une Directional Light (pour les intérieurs)")]
    public bool useSpotLight = false;
    
    [Header("Références")]
    public Light directionalLight;
    [Tooltip("Spot Light pour les scènes intérieures (alternative à directionalLight)")]
    public Light spotLight;
    public Material skyboxMaterial; // Assigner le material Skybox/Procedural
    
    //~ Important
    [Header("Gestion du temps du jour")]
    public float dayLength = 420f; // Length of a full day in seconds
    [Space]
    [Tooltip("Cocher pour contrôler manuellement l'heure du jour")]
    public bool useManualTime = false;
    [Tooltip("Contrôle manuel du temps (0 = minuit, 0.5 = midi, 1 = minuit)")]
    [Range(0f, 1f)]
    public float manualTime = 0.5f;
    public float timeLasted;

    [SerializeField] private float timeStart = 0.25f; // 8h
    [SerializeField] private float timeEnd = 0.75f; // 18h

    public event Action onDayEnd; // Event lancé quand le jour ce termine
    
    // Flag pour éviter d'appeler onDayEnd plusieurs fois
    private bool dayEndTriggered = false;

    [Header("Couleurs du ciel")]
    [Tooltip("Couleur du ciel à midi")]
    public Color daySkyTint = new Color(0.5f, 0.5f, 0.5f, 1f);
    [Tooltip("Couleur du ciel au lever")]
    public Color sunriseSkyTint = new Color(0.9f, 0.6f, 0.4f, 1f);
    [Tooltip("Couleur du ciel au coucher")]
    public Color sunsetSkyTint = new Color(1f, 0.5f, 0.3f, 1f);
    [Tooltip("Couleur du ciel la nuit")]
    public Color nightSkyTint = new Color(0.1f, 0.1f, 0.2f, 1f);
    
    [Header("Couleurs de la lumière")]
    public Color dayLightColor = new Color(1f, 0.95f, 0.85f, 1f);
    public Color sunriseLightColor = new Color(1f, 0.7f, 0.5f, 1f);
    public Color sunsetLightColor = new Color(1f, 0.6f, 0.3f, 1f);
    public Color nightLightColor = new Color(0.3f, 0.3f, 0.5f, 1f);
    
    [Header("Intensité (Spot Light)")]
    [Tooltip("Intensité de la Spot Light en journée")]
    public float daySpotIntensity = 3f;
    [Tooltip("Intensité de la Spot Light la nuit")]
    public float nightSpotIntensity = 0.2f;

    [Header("Position Spot Light")]
    [Tooltip("Distance du joueur pour le Spot Light (simule un soleil intérieur)")]
    public float spotLightDistance = 15f;
    [Tooltip("Hauteur moyenne du Spot Light par rapport au joueur")]
    public float spotLightHeight = 8f;
    [Tooltip("Amplitude de la variation de hauteur (monte et descend)")]
    public float spotLightHeightVariation = 6f;
    [Tooltip("Axe de rotation de l'arc du soleil (ex: (0,0,1) pour rotation autour de Z)")]
    public Vector3 sunArcRotationAxis = Vector3.forward;
    [Tooltip("Décalage de l'angle de départ (en degrés)")]
    public float sunArcStartAngleOffset = 0f;
    [Tooltip("Pivot autour duquel le Spot Light orbite (gameObject vide)")]
    public Transform pivotTransform;
    [Tooltip("Transform du joueur (pour que la lumière le regarde)")]
    public Transform playerTransform;

    [Header("Fenêtre jour/nuit (heures normalisées)")]
    [Tooltip("Début du lever (0-1). 0.33 ≈ 8h")]
    [Range(0f, 1f)] public float sunriseStart = 0.33f;
    [Tooltip("Durée du lever (0-1). 0.1 ≈ 2h30")]
    [Range(0.01f, 0.5f)] public float sunriseDuration = 0.10f;
    [Tooltip("Début du coucher (0-1). 0.92 ≈ 22h")]
    [Range(0f, 1f)] public float sunsetStart = 0.92f;
    [Tooltip("Durée du coucher (0-1). 0.08 ≈ 2h")]
    [Range(0.01f, 0.5f)] public float sunsetDuration = 0.08f;

    [Header("Atmosphère")]
    [Range(0f, 5f)]
    public float dayAtmosphereThickness = 1f;
    [Range(0f, 5f)]
    public float sunriseAtmosphereThickness = 1.3f;
    [Range(0f, 5f)]
    public float sunsetAtmosphereThickness = 2f;
    [Range(0f, 5f)]
    public float nightAtmosphereThickness = 0.5f;

    [Header("Ambient Lighting")]
    [Tooltip("Activer le contrôle automatique de l'ambient lighting")]
    public bool controlAmbientLighting = true;
    [Tooltip("Couleur ambiante en journée")]
    public Color dayAmbientColor = new Color(0.6f, 0.6f, 0.7f, 1f);
    [Tooltip("Couleur ambiante la nuit (très sombre)")]
    public Color nightAmbientColor = new Color(0.02f, 0.02f, 0.05f, 1f);
    [Tooltip("Intensité ambiante en journée")]
    [Range(0f, 2f)]
    public float dayAmbientIntensity = 1f;
    [Tooltip("Intensité ambiante la nuit")]
    [Range(0f, 2f)]
    public float nightAmbientIntensity = 0.1f;
    
    // Référence à la lumière active
    private Light activeLight;

    // convertir time en heures et minutes pour affichage si besoin

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    /**
     * Au lancement, le jour est instancié par défaut
     *
     * @return	void
     */
    void Start()
    {
        // Choisir la lumière active selon le mode
        if (useSpotLight)
        {
            activeLight = spotLight;
            if (spotLight == null)
            {
                Debug.LogError("Aucune Spot Light assignée pour le cycle jour/nuit en mode intérieur.");
                return;
            }
        }
        else
        {
            activeLight = directionalLight;
            if (directionalLight == null)
            {
                Debug.LogError("Aucune lumière directionnelle assignée pour le cycle jour/nuit.");
                return;
            }
        }

        // Si pas de skybox assigné, essayer de récupérer celui de RenderSettings
        if (skyboxMaterial == null)
        {
            skyboxMaterial = RenderSettings.skybox;
        }

        // Par défaut, démarrer en jour
        //StartDay();
    }

    /**
     * Change la lumière au fur et à mesure de la journée pour faire un cycle jour/nuit
     *
     * @return	void
     */
    void Update()
    {
        // Utiliser le temps manuel ou auto-incrémenter
        if (useManualTime)
        {
            timeLasted = manualTime;
        }
        else
        {
            // Increment time automatiquement
            timeLasted += Time.deltaTime / dayLength;
            
            // NE PAS utiliser modulo ici, sinon le jour redémarre automatiquement
            // timeLasted %= 1; // ENLEVER CETTE LIGNE
        }

        // Lancement de l'event pour dire que la fin du jour a été atteint (UNE SEULE FOIS)
        if (timeLasted >= timeEnd && !dayEndTriggered)
        {
            dayEndTriggered = true;
            onDayEnd?.Invoke();
            Debug.Log($"<color=orange>[DayNightManager]</color> Fin du jour déclenchée à {GetCurrentTimeString()}");
        }
        
        // Apply lighting/rotation for the current time
        UpdateLighting(timeLasted);
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
        if (useSpotLight)
        {
            // Pour Spot Light: positionnement autour du pivot, regardant vers le joueur
            if (spotLight != null && pivotTransform != null && playerTransform != null)
            {
                // Rotation sur un seul axe : 0..1 -> 0..360°
                float angle = t * 360f + sunArcStartAngleOffset;
                
                // Position de base (point de départ avant rotation)
                Vector3 basePosition = Vector3.right * spotLightDistance;
                
                // Appliquer la rotation autour de l'axe configuré
                Quaternion rotation = Quaternion.AngleAxis(angle, sunArcRotationAxis.normalized);
                Vector3 rotatedPosition = rotation * basePosition;
                
                // Ajouter la hauteur
                Vector3 offsetPos = rotatedPosition + Vector3.up * spotLightHeight;
                
                // Orbiter autour du pivot (pas du joueur)
                spotLight.transform.position = pivotTransform.position + offsetPos;
                
                // Orienter le Spot Light vers le joueur
                Vector3 directionToPlayer = (playerTransform.position - spotLight.transform.position).normalized;
                spotLight.transform.rotation = Quaternion.LookRotation(directionToPlayer);
                
                // Synchroniser la Directional Light avec le Spot Light
                if (directionalLight != null)
                {
                    // La directional light pointe dans la même direction que le spot
                    directionalLight.transform.rotation = spotLight.transform.rotation;
                }
            }
        }
        else
        {
            // Applique la rotation du soleil (Directional Light)
            float sunAngle = t * 360f - 90f;
            transform.localRotation = Quaternion.Euler(sunAngle, 170f, 0f);
        }

        if (activeLight == null)
            return;

        // Calcul des phases de la journée avec fenêtres configurables
        float intensity;
        Color lightColor;
        Color skyTint;
        float atmosphereThickness;

        float sunriseEnd = sunriseStart + sunriseDuration;
        float sunsetEnd = sunsetStart + sunsetDuration;
        
        // Durée de transition sunrise->jour et jour->sunset (moitié de la durée du lever/coucher)
        float morningTransitionEnd = sunriseEnd + sunriseDuration;
        float eveningTransitionStart = sunsetStart - sunsetDuration;

        if (t < sunriseStart || t >= sunsetEnd)
        {
            // Nuit
            intensity = 0.1f;
            lightColor = nightLightColor;
            skyTint = nightSkyTint;
            atmosphereThickness = nightAtmosphereThickness;
        }
        else if (t < sunriseEnd)
        {
            // Lever (nuit -> sunrise)
            float blend = Mathf.InverseLerp(sunriseStart, sunriseEnd, t);
            // Utiliser SmoothStep pour une transition plus douce
            blend = Mathf.SmoothStep(0f, 1f, blend);
            intensity = Mathf.Lerp(0.1f, 0.8f, blend);
            lightColor = Color.Lerp(nightLightColor, sunriseLightColor, blend);
            skyTint = Color.Lerp(nightSkyTint, sunriseSkyTint, blend);
            atmosphereThickness = Mathf.Lerp(nightAtmosphereThickness, sunriseAtmosphereThickness, blend);
        }
        else if (t < morningTransitionEnd)
        {
            // Transition matin (sunrise -> jour)
            float blend = Mathf.InverseLerp(sunriseEnd, morningTransitionEnd, t);
            blend = Mathf.SmoothStep(0f, 1f, blend);
            intensity = Mathf.Lerp(0.8f, 1f, blend);
            lightColor = Color.Lerp(sunriseLightColor, dayLightColor, blend);
            skyTint = Color.Lerp(sunriseSkyTint, daySkyTint, blend);
            atmosphereThickness = Mathf.Lerp(sunriseAtmosphereThickness, dayAtmosphereThickness, blend);
        }
        else if (t < eveningTransitionStart)
        {
            // Jour (plein)
            intensity = 1f;
            lightColor = dayLightColor;
            skyTint = daySkyTint;
            atmosphereThickness = dayAtmosphereThickness;
        }
        else if (t < sunsetStart)
        {
            // Transition après-midi (jour -> sunset)
            float blend = Mathf.InverseLerp(eveningTransitionStart, sunsetStart, t);
            blend = Mathf.SmoothStep(0f, 1f, blend);
            intensity = 1f;
            lightColor = Color.Lerp(dayLightColor, sunsetLightColor, blend);
            skyTint = Color.Lerp(daySkyTint, sunsetSkyTint, blend);
            atmosphereThickness = Mathf.Lerp(dayAtmosphereThickness, sunsetAtmosphereThickness, blend);
        }
        else // t >= sunsetStart && t < sunsetEnd
        {
            // Coucher (sunset -> nuit)
            float blend = Mathf.InverseLerp(sunsetStart, sunsetEnd, t);
            blend = Mathf.SmoothStep(0f, 1f, blend);
            intensity = Mathf.Lerp(0.8f, 0.1f, blend);
            lightColor = Color.Lerp(sunsetLightColor, nightLightColor, blend);
            skyTint = Color.Lerp(sunsetSkyTint, nightSkyTint, blend);
            atmosphereThickness = Mathf.Lerp(sunsetAtmosphereThickness, nightAtmosphereThickness, blend);
        }

        // Appliquer à la lumière
        if (useSpotLight)
        {
            // Pour Spot Light, utiliser les fenêtres configurables lever/coucher
            // sunriseEnd et sunsetEnd déjà calculés plus haut
            float dayBlend = 0f;

            if (t < sunriseStart)
            {
                dayBlend = 0f; // nuit avant lever
            }
            else if (t < sunriseEnd)
            {
                // montée progressive au lever
                dayBlend = Mathf.InverseLerp(sunriseStart, sunriseEnd, t);
            }
            else if (t < sunsetStart)
            {
                dayBlend = 1f; // plein jour
            }
            else if (t < sunsetEnd)
            {
                // descente progressive au coucher
                dayBlend = 1f - Mathf.InverseLerp(sunsetStart, sunsetEnd, t);
            }
            else
            {
                dayBlend = 0f; // nuit après coucher
            }

            float spotIntensity = Mathf.Lerp(nightSpotIntensity, daySpotIntensity, dayBlend);
            activeLight.intensity = spotIntensity;
            
            // Synchroniser aussi la couleur et l'intensité de la Directional Light
            if (directionalLight != null)
            {
                directionalLight.color = lightColor;
                directionalLight.intensity = intensity;
            }
        }
        else
        {
            activeLight.intensity = intensity;
        }
        activeLight.color = lightColor;

        // Appliquer au skybox (si c'est un Skybox/Procedural)
        UpdateSkybox(skyTint, atmosphereThickness);
        
        // Appliquer l'ambient lighting
        UpdateAmbientLighting(intensity);
    }

    /**
     * Met à jour l'ambient lighting selon l'heure
     */
    private void UpdateAmbientLighting(float dayIntensity)
    {
        if (!controlAmbientLighting) return;
        
        // Calculer le blend jour/nuit basé sur les fenêtres sunrise/sunset
        float sunriseEnd = sunriseStart + sunriseDuration;
        float sunsetEnd = sunsetStart + sunsetDuration;
        
        float ambientBlend = 0f;
        
        if (timeLasted < sunriseStart)
        {
            ambientBlend = 0f;
        }
        else if (timeLasted < sunriseEnd)
        {
            ambientBlend = Mathf.InverseLerp(sunriseStart, sunriseEnd, timeLasted);
        }
        else if (timeLasted < sunsetStart)
        {
            ambientBlend = 1f;
        }
        else if (timeLasted < sunsetEnd)
        {
            ambientBlend = 1f - Mathf.InverseLerp(sunsetStart, sunsetEnd, timeLasted);
        }
        else
        {
            ambientBlend = 0f;
        }
        
        // Appliquer la couleur ambiante
        RenderSettings.ambientMode = AmbientMode.Flat;
        RenderSettings.ambientLight = Color.Lerp(nightAmbientColor, dayAmbientColor, ambientBlend);
        RenderSettings.ambientIntensity = Mathf.Lerp(nightAmbientIntensity, dayAmbientIntensity, ambientBlend);
        
        // Réduire aussi les réflexions la nuit
        RenderSettings.reflectionIntensity = Mathf.Lerp(0.2f, 1f, ambientBlend);
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
            float exposure = Mathf.Lerp(0.5f, 1.3f, activeLight != null ? activeLight.intensity / (useSpotLight ? daySpotIntensity : 1f) : 1f);
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
        timeLasted = timeStart;
        dayEndTriggered = false; // Réinitialiser le flag
        UpdateLighting(timeLasted);
        Debug.Log($"<color=green>[DayNightManager]</color> Nouveau jour commencé à {GetCurrentTimeString()}");
    }

    /**
     * Forcer le démarrage en mode nuit (minuit).
     *
     * @access	public
     * @return	void
     */
    public void StartNight()
    {
        timeLasted = timeEnd;
        dayEndTriggered = false; // Réinitialiser le flag
        UpdateLighting(timeLasted);
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

        // Réinitialiser le flag si on revient avant timeEnd
        if (newTime < timeEnd)
        {
            dayEndTriggered = false;
        }

        // Met à jour la variable interne puis applique l'éclairage
        timeLasted = newTime;
        UpdateLighting(timeLasted);
        return timeLasted;
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
        return TimeToHourMinute(timeLasted);
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
        Vector2Int hm = TimeToHourMinute(timeLasted);
        return hm.x.ToString("D2") + ":" + hm.y.ToString("D2");
    }

    public float GetTimeStart() => timeStart;
    public float GetTimeEnd() => timeEnd;


}