using System;

/**
 * Classe pour gérer les événements d'input
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Thursday, January 8th, 2026.
 * @global
 */
public class S_InputEvent 
{
    public event Action onSubmitPressed;

    public void SubmitPressed()
    {
        onSubmitPressed?.Invoke();
    }
}
