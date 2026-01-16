using UnityEngine;

public class S_Quest
{
    public SO_QuestInfo info;

    public E_QuestState state;

    private int currentQuestStepIndex;

    private S_QuestStepState[] questStepStates;

    // Tracking de l'instance active pour éviter les doublons
    private GameObject currentStepInstance;
    public int CurrentStepIndex => currentQuestStepIndex;

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
        Debug.Log($"[S_Quest] Quest '{info.displayName}' (ID: {info.id}) moved to step index: {currentQuestStepIndex}");
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
        // Détruire l'ancienne instance si elle existe encore
        if (currentStepInstance != null)
        {
            Debug.LogWarning($"<color=orange>[S_Quest]</color> Destruction de l'ancienne instance d'étape pour '{info.id}'");
            GameObject.Destroy(currentStepInstance);
            currentStepInstance = null;
        }

        GameObject questStepPrefab = CurrentQuestStepPrefab();
        if (questStepPrefab != null)
        {
            currentStepInstance = GameObject.Instantiate<GameObject>(questStepPrefab, parentTransform);
            S_QuestStep questStep = currentStepInstance.GetComponent<S_QuestStep>();
            
            if (questStep != null)
            {
                // Initialiser l'étape de quête avec son ID, index et état
                string questStepState = GetQuestStepState();
                questStep.InitializeQuestStep(info.id, currentQuestStepIndex, questStepState);
                Debug.Log($"<color=cyan>[S_Quest]</color> Étape {currentQuestStepIndex} instanciée pour '{info.id}'");
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
        if (currentQuestStepIndex < questStepStates.Length)
        {
            return questStepStates[currentQuestStepIndex].state;
        }
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

    /**
     * Récupère le nom d'affichage de l'étape actuelle pour l'UI
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 7th, 2026.
     * @access	public
     * @return	string
     */
    public string GetCurrentStepDisplayName()
    {
        if (CurrentStepExists())
        {
            GameObject currentStepPrefab = info.questStepsPrefabs[currentQuestStepIndex];
            if (currentStepPrefab != null)
            {
                S_QuestStep questStep = currentStepPrefab.GetComponent<S_QuestStep>();
                if (questStep != null && !string.IsNullOrEmpty(questStep.stepName))
                {
                    return questStep.stepName;
                }
            }
        }
        // Fallback sur le displayName de la quête
        return info.displayName;
    }

    public S_QuestData GetQuestData()
    {
        return new S_QuestData(state, currentQuestStepIndex, questStepStates);
    }

    /**
     * Réinitialise la quête à son état initial
     * Utilisé pour les quêtes répétitives au début de chaque jour
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @return	void
     */
    public void ResetQuest()
    {
        // Nettoyer l'instance de step actuelle
        CleanupCurrentStep();
        
        // Réinitialiser l'état
        state = E_QuestState.REQUIREMENTS_NOT_MET;
        currentQuestStepIndex = 0;
        
        // Réinitialiser les états des étapes
        for (int i = 0; i < questStepStates.Length; i++)
        {
            questStepStates[i] = new S_QuestStepState();
        }
        
        Debug.Log($"<color=yellow>[S_Quest]</color> Quête '{info.displayName}' réinitialisée");
    }

    /**
     * Nettoie l'instance de l'étape actuelle
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 13th, 2026.
     * @access	public
     * @return	void
     */
    public void CleanupCurrentStep()
    {
        if (currentStepInstance != null)
        {
            GameObject.Destroy(currentStepInstance);
            currentStepInstance = null;
        }
    }

}
