using UnityEngine;

/**
 * classe représentant une instance de quête pour un joueur
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Monday, November 10th, 2025.
 * @global
 */
public class S_QuestInstance : MonoBehaviour
{

    public string questId;
    public int currentStageIndex;
    public System.Collections.Generic.Dictionary<string, float> progress;
    public bool isCompleted;
    public System.DateTime lastCompletedTime;



    /**
     * change l'étape actuelle de la quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @access	public
     * @return	void
     */
    public void AdvanceStage()
    {
        currentStageIndex++;
    }

    /**
     * réinitialise la progression de la quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @access	public
     * @return	void
     */
    public void ResetProgress()
    {
        progress = new System.Collections.Generic.Dictionary<string, float>();
        isCompleted = false;
    }

    /**
     * met la quête comme complétée
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @access	public
     * @return	void
     */
    public void MarkCompleted()
    {
        isCompleted = true;
        lastCompletedTime = System.DateTime.Now;
    }


}
