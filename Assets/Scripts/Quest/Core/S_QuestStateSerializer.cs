using UnityEngine;


/**
 * classe pour sérialiser et désérialiser l'état des quêtes
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Monday, November 10th, 2025.
 * @global
 */
public class S_QuestStateSerializer : MonoBehaviour
{
    /**
     * Méthode pour sauvegarder l'état des quêtes
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @access	public
     * @param	mixed	quests	
     * @return	void
     */
    public void SaveState(System.Collections.Generic.List<S_QuestInstance> quests)
    {
        // TODO
    }
    
    /**
     * Charge une sauvegarde
     *
     * @var		mixed	LoadState()
     */
    public System.Collections.Generic.List<S_QuestInstance> LoadState()
    {
        return new System.Collections.Generic.List<S_QuestInstance>();
    }
}
