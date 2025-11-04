using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class S_Objective : MonoBehaviour
{
    public string objectiveName;
    public string description;
    public bool isCompleted = false;

    public UnityEvent OnObjectiveCompleted; //& Événement déclenché lorsque l'objectif est complété

    public void Complete()
    {
        if (!isCompleted)
        {
            isCompleted = true;
            OnObjectiveCompleted?.Invoke();
        }
    }
}
