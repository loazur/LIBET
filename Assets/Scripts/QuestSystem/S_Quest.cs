using UnityEngine;

public class S_Quest
{
    public SO_QuestInfo info;

    public E_QuestState state;

    private int currentQuestStepIndex;

    private S_QuestStepState[] questStepStates;

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
        this.questStepStates = new S_QuestStepState[questInfo.questStepsPrefabs.Length];
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new S_QuestStepState();
        }
    }


    public S_Quest(SO_QuestInfo questInfo, E_QuestState questState, int currentQuestStepIndex, S_QuestStepState[] questStepStates)
    {
        this.info = questInfo;
        this.state = questState;
        this.currentQuestStepIndex = currentQuestStepIndex;
        this.questStepStates = questStepStates;

        // if the quest step states and prefabs are different lengths,
        // something has changed during development and the saved data is out of sync.
        if (this.questStepStates.Length != this.info.questStepsPrefabs.Length)
        {
            Debug.LogWarning("Quest Step Prefabs and Quest Step States are "
                + "of different lengths. This indicates something changed "
                + "with the QuestInfo and the saved data is now out of sync. "
                + "Reset your data - as this might cause issues. QuestId: " + this.info.id);
        }
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

    /**
     * Instancie l'étape actuelle de la quête dans le parent donné
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 16th, 2025.
     * @access	public
     * @param	transform	parentTransform	
     * @return	void
     */
    public void InstantiateCurrentQuestStep(Transform parentTransform)
    {
        GameObject questStepPrefab = CurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            GameObject questStepInstance = GameObject.Instantiate<GameObject>(questStepPrefab, parentTransform);
            S_QuestStep questStep = questStepInstance.GetComponent<S_QuestStep>();
            
            if (questStep != null)
            {
                // Initialiser l'étape de quête avec son ID, index et état
                string questStepState = GetQuestStepState();
                questStep.InitializeQuestStep(info.id, currentQuestStepIndex, questStepState);
            }
            else
            {
                Debug.LogError("InstantiateCurrentQuestStep: Le prefab ne contient pas de composant S_QuestStep!");
            }
        }
        else
        {
            Debug.LogWarning("Cannot instantiate quest step: prefab is null for quest: " + info.id + " at index: " + currentQuestStepIndex);
        }
    }

    /**
     * Récupère le prefab de l'étape actuelle de la quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Sunday, November 16th, 2025.
     * @access	private
     * @return	mixed
     */
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

    /**
     * Récupère l'état sauvegardé de l'étape actuelle (pour la persistence)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Saturday, November 23rd, 2025.
     * @access	private
     * @return	string
     */
    private string GetQuestStepState()
    {
        // TODO: Implémenter la récupération de l'état depuis le système de sauvegarde
        // Pour l'instant, retourner une chaîne vide
        return "";
    }

    

    public void StoreQuestStepState(S_QuestStepState questStepState, int stepIndex)
    {
        if (stepIndex < questStepStates.Length)
        {
            questStepStates[stepIndex].state = questStepState.state;
            questStepStates[stepIndex].status = questStepState.status;
        }
        else 
        {
            Debug.LogWarning("Tried to access quest step data, but stepIndex was out of range: "
                + "Quest Id = " + info.id + ", Step Index = " + stepIndex);
        }
    }

    public S_QuestData GetQuestData()
    {
        return new S_QuestData(state, currentQuestStepIndex, questStepStates);
    }
}
