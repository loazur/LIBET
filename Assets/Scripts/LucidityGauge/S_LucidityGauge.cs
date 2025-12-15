// Jauge de lucidité
// PLus c'est bas plus l'intervale des invent est cours + plus vnr


using UnityEngine;



class S_LucidityGauge : MonoBehaviour
{
    #region Attributes
    public float Gauge = 100.0f; // plus c'est bas plus c'est fort


    public S_AlzheimerEventsManager alzheimerEventsManager;


    #endregion Attributes

    #region Methods



    // gestion de la jauge

    














    /**
     * Baisser la jauge
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	public
     * @param	float	amount	
     * @return	void
     */
    public void DecreaseGauge(float amount)
    {
        Gauge -= amount;

        // Cas où la jauge descend en dessous de 0
        if (Gauge < 0)
        {
            Gauge = 0;
        }
    }

    /**
     * Augmente la jauge
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, December 15th, 2025.
     * @access	public
     * @param	float	amount	
     * @return	void
     */
    public void IncreaseGauge(float amount)
    {
        Gauge += amount;

        // Cas où la jauge dépasse 100
        if (Gauge > 100)
        {
            Gauge = 100;
        }
    }

    #endregion Methods


    // Debug 
    #region  Debug Methods


    [ContextMenu("Afficher Jauge")]
    void TestAfficherJauge()
    {
        Debug.Log("Jauge de lucidité actuelle : " + Gauge);
    }

    [ContextMenu("Test Increase Jauge")]
    void TestIncreaseJauge()
    {
        IncreaseGauge(10.0f);
        Debug.Log("Jauge de lucidité après augmentation : " + Gauge);
    }

    [ContextMenu("Test Decrease Jauge")]
    void TestDecreaseJauge()
    {
        DecreaseGauge(10.0f);
        Debug.Log("Jauge de lucidité après diminution : " + Gauge);
    }

    #endregion Debug Methods

}