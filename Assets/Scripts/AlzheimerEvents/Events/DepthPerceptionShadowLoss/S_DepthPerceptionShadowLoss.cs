using UnityEngine;



/**
 * Supprimer les ombres dans le jeu pour que les joueurs ne puissent vraiment pas
 * évaluer les distances
 * (DepthPerceptionShadowLoss)
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Saturday, December 6th, 2025.
 * @global
 */
class S_DepthPerceptionShadowLoss : MonoBehaviour
{
    //~ DepthPerceptionShadowLoss -> Désactive les ombres

     void OnEnable() //& Activation de l'event
    {   
        EnableAllShadows();
    }

    void OnDisable() //& Désactivation de l'event
    {
        DisableAllShadows();
    }

    //!---------------------------------------------------------------

    /**
     * désactive toutes les ombres dans la scène
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, December 6th, 2025.
     * @return	void
     */
    void DisableAllShadows()
    {
        foreach (var light in Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            light.shadows = LightShadows.None;
        }

    }

    /**
     * Active toutes les ombres dans la scène
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, December 6th, 2025.
     * @return	void
     */
    void EnableAllShadows()
    {
        foreach (var light in Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            light.shadows = LightShadows.Soft;
        }

    }

    [ContextMenu("Test Disable Shadows")]
    void TestDisableShadows()
    {
        DisableAllShadows();
        Debug.Log("Shadows Disabled");
    }

    [ContextMenu("Test Enable Shadows")]
    void TestEnableShadows()
    {
        EnableAllShadows();
        Debug.Log("Shadows Enabled");
    }


}