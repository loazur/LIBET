using System;

/// <summary>
/// Classe pour gérer les événements d'input
/// </summary>
public class S_InputEvent 
{
    public event Action onSubmitPressed;

    public void SubmitPressed()
    {
        onSubmitPressed?.Invoke();
    }
}
