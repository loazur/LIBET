using System;
using UnityEngine;

public abstract class S_AbstractMinigame : MonoBehaviour
{
    public event Action OnMinigameWin; 
    public event Action OnMinigameLose; 
    public abstract void TriggerMinigame(); // Doit redefinir la fonction qui lance le minijeu
    
    protected void TriggerWin()
    {
        OnMinigameWin?.Invoke();
    }

    protected void TriggerLose()
    {
        OnMinigameLose?.Invoke();
    }
}
