using UnityEngine;

public abstract class S_QuestStep : MonoBehaviour
{
    public string stepNameFrench; //& ce sera le nom de la quete step dans l'éditeur
    public string stepNameEnglish;
    private bool isFinished = false;
    private string questId;
    private int stepIndex;

    public int StepIndex => stepIndex; // Getter public pour vérification

    //& Propriété qui retourne le nom traduit automatiquement
    public string stepName
    {
        get
        {
            // Vérifie si l'instance existe
            if (S_GameUserData.instance == null)
            {
                // Fallback: retourne le français par défaut, ou l'anglais si le français est vide
                return !string.IsNullOrEmpty(stepNameFrench) ? stepNameFrench : stepNameEnglish;
            }

            // Retourne le nom selon la langue actuelle
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                return !string.IsNullOrEmpty(stepNameFrench) ? stepNameFrench : stepNameEnglish;
            }
            else // English ou autre
            {
                return !string.IsNullOrEmpty(stepNameEnglish) ? stepNameEnglish : stepNameFrench;
            }
        }
    }

    //& --------------- Fonctions publics ---------------


    /**
     * Initialisation de la quest step
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, November 26th, 2025.
     * @access	public
     * @param	string	questId       	
     * @param	int   	stepIndex     	
     * @param	string	questStepState	
     * @return	void
     */
    public void InitializeQuestStep(string questId, int stepIndex, string questStepState)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
        if (questStepState != null && questStepState != "")
        {
            SetQuestStepState(questStepState);
        }
    }

    /**
     * Permet de terminer la quest step et d'avancer dans la quête
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, November 26th, 2025.
     * @access	protected
     * @return	void
     */
    protected void FinishQuestStep()
    {
        if (!isFinished)
        {
            isFinished = true;
            
            if (string.IsNullOrEmpty(questId))
            {
                Debug.LogError("[S_QuestStep] FinishQuestStep appelé mais questId est null! Assurez-vous que InitializeQuestStep() a été appelé.");
                return;
            }

            Debug.Log($"<color=magenta>[S_QuestStep]</color> Étape {stepIndex} terminée pour '{questId}'");
            S_GameManager.instance.questEvents.AdvanceQuest(questId);
            Destroy(this.gameObject);
        }
        else
        {
            Debug.LogWarning($"<color=yellow>[S_QuestStep]</color> FinishQuestStep ignoré - étape {stepIndex} déjà terminée pour '{questId}'");
        }
    }

    /**
     * Changement de l'état de la quest step
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, November 26th, 2025.
     * @access	protected
     * @param	string	newState 	
     * @param	string	newStatus	
     * @return	void
     */
    protected void ChangeState(string newState, string newStatus)
    {
        S_GameManager.instance.questEvents.QuestStepStateChange(
            questId, 
            stepIndex, 
            new S_QuestStepState(newState, newStatus)
        );
    }

    
    /**
     * Vérifie si la quest step a été correctement initialisée
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, January 5th, 2026.
     * @access	protected
     * @return	void
     */
    protected bool IsQuestStepInitialized()
    {
        return !string.IsNullOrEmpty(questId);
    }


    protected abstract void SetQuestStepState(string state); // Méthode abstraite à implémenter dans les classes dérivées
}
