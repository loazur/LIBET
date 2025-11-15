using UnityEngine;

public class S_Quest
{
    public SO_QuestInfo info;

    public E_QuestState state;

    private int currentQuestStepIndex;

    /**
     * constructeur qui initialise les variables
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 15th, 2025.
     * @param	so_questinfo	questInfo	
     * @return	void
     */
    public S_Quest(SO_QuestInfo questInfo)
    {
        this.info = questInfo;
        this.state = E_QuestState.REQUIREMENTS_NOT_MET;
        this.currentQuestStepIndex = 0;
    }

    /**
     * permet de passer à l'étape suivante de la quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 15th, 2025.
     * @access	public
     * @return	void
     */
    public void MoveToNextStep()
    {
        currentQuestStepIndex++;
    }

    /**
     * Vérifie si l'étape actuelle existe
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 15th, 2025.
     * @access	public
     * @return	mixed
     */
    public bool CurrentStepExists()
    {
        return currentQuestStepIndex < info.questStepsPrefabs.Length;
    }

    public void InstantiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = CurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            GameObject.Instantiate<GameObject>(questStepPrefab, parentTransform);
        }
        else
        {
            Debug.LogWarning("Cannot instantiate quest step: prefab is null for quest: " + info.id + " at index: " + currentQuestStepIndex);
        }
    }

    private GameObject CurrentQuestStepPrefab()
    {
        GameObject questStepPrefab = null;
        if (CurrentStepExists())
        {
            questStepPrefab = info.questStepsPrefabs[currentQuestStepIndex];
        }
        else
        {
            Debug.LogWarning("No current quest step exists for quest: " + info.id + " at index: " + currentQuestStepIndex);
        }
        return questStepPrefab;
    }
}
