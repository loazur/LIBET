using System;
using NUnit.Framework;
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
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Friday, October 24th, 2025.
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
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Friday, October 24th, 2025.
     * @return	void
     */
    void Update()
    {
        // Increment time
        time += Time.deltaTime / dayLength;
        time %= 1; // Keep time in range [0, 1]
        // Apply lighting/rotation for the current time
        UpdateLighting(time);

        print(time);
    }

    /**
     * Met à jour la rotation du soleil et l'intensité de la lumière en fonction du temps (0..1).
     *
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Friday, October 24th, 2025.
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
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Friday, October 24th, 2025.
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
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Friday, October 24th, 2025.
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
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Friday, October 24th, 2025.
     * @access	public
     * @param	float	newTime	
     * @return	mixed
     */
    public float SetTime(float newTime)
    {
        time = newTime % 1f;
        UpdateLighting(time);
        return time;
    }

    
}