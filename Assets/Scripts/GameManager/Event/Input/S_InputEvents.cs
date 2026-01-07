using UnityEngine;
using System;

public class S_InputEvents
{
    public E_InputEventContext inputEventContext { get; private set; } = E_InputEventContext.DEFAULT;

    public void ChangeInputEventContext(E_InputEventContext newContext) 
    {
        this.inputEventContext = newContext;
    }

    public event Action<Vector2> onMovePressed;
    public void MovePressed(Vector2 moveDir) 
    {
        if (onMovePressed != null) 
        {
            onMovePressed(moveDir);
        }
    }

    public event Action<E_InputEventContext> onSubmitPressed;
    public void SubmitPressed()
    {
        if (onSubmitPressed != null) 
        {
            onSubmitPressed(this.inputEventContext);
        }
    }

    public event Action onQuestLogTogglePressed;
    public void QuestLogTogglePressed()
    {
        if (onQuestLogTogglePressed != null) 
        {
            onQuestLogTogglePressed();
        }
    }
}
