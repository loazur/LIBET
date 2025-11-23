
using UnityEngine;

public abstract class S_QuestStep : MonoBehaviour
{
    private bool isFinished = false;
    private string questId;
    private int stepIndex;

    public void InitializeQuestStep(string questId, int stepIndex, string questStepState)
    {
        this.questId = questId;
        this.stepIndex = stepIndex;
        if (questStepState != null && questStepState != "")
        {
            SetQuestStepState(questStepState);
        }
    }

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
            
            S_GameManager.instance.questEvents.AdvanceQuest(questId);
            Destroy(this.gameObject);
        }
    }

    protected void ChangeState(string newState, string newStatus)
    {
        S_GameManager.instance.questEvents.QuestStepStateChange(
            questId, 
            stepIndex, 
            new S_QuestStepState(newState, newStatus)
        );
    }

    /// <summary>
    /// Vérifie si la quest step a été correctement initialisée
    /// </summary>
    protected bool IsQuestStepInitialized()
    {
        return !string.IsNullOrEmpty(questId);
    }

    protected abstract void SetQuestStepState(string state);
}
