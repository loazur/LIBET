using System;
using JetBrains.Annotations;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class S_DayNight : MonoBehaviour
{
    public Light directionalLight;
    public float dayLength = 120f; // Length of a full day in seconds
    private float time;

    // convertir time en heures et minutes pour affichage si besoin
    

    /**
     * Au lancement, le jour est instancié par défaut
     *
     * @return	void
     */
    void Start()
    {
        Assert.IsNotNull(directionalLight, "Aucune lumière directionnelle assignée pour le cycle jour/nuit.");

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

        Debug.Log(GetCurrentTimeString());
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
        // Rotate the directional light to simulate the sun's movement
        float sunAngle = t * 360f - 90f;
        transform.localRotation = Quaternion.Euler(sunAngle, 170f, 0f);

        if (directionalLight == null)
            return;

        // Adjust the light's intensity based on the time of day
        if (t <= 0.23f || t >= 0.75f)
        {
            directionalLight.intensity = 0;
        }
        else if (t <= 0.25f)
        {
            directionalLight.intensity = Mathf.Lerp(0, 1, (t - 0.23f) * 50);
        }
        else if (t >= 0.73f)
        {
            directionalLight.intensity = Mathf.Lerp(1, 0, (t - 0.73f) * 50);
        }
        else
        {
            directionalLight.intensity = 1;
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