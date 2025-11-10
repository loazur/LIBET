using NUnit.Framework;
using UnityEngine;

public abstract class S_QuestStep : MonoBehaviour
{
    private bool isFinished = false;

    protected void FinishStepQuest()
    {
        if (!isFinished)
        {
            isFinished = true;
        }
    }
}
