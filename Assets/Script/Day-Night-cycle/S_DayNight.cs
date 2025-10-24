using UnityEngine;

/**
 * change le direction of light pour simuler un cycle jour et la nuit
 * besoins d'être attaché à un diresctional light 
 *
 * @author	Unknown
 * @since	v0.0.1
 * @version	v1.0.0	Friday, October 24th, 2025.
 * @global
 */
public class S_DayNight : MonoBehaviour
{

    public bool isDay = true;
    public float speed = 1f;
    public float dayTime = 10f; // entre 6h et 18h (12h de journée)
    public float ActualTime = 0f;


    /**
     * initialisation du ciscle jour/nuit
     *
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Friday, October 24th, 2025.
     * @return	void
     */
    void Start()
    {
        transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    /**
     * Change le temps petit à petit en fonction de la vitesse, du dayTime et du ActualTime
     *
     * @author	Unknown
     * @since	v0.0.1
     * @version	v1.0.0	Friday, October 24th, 2025.
     * @return	void
     */
    void Update()
    {
        
    }
}
