using UnityEngine;
using UnityEngine.Events;

public class S_Padlock : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private string password = "1234";
    [Tooltip("Mot de passe actuellement entré par le joueur")]
    [SerializeField] private string currentInput = "";
    
    [Header("Events")]
    [SerializeField] private UnityEvent onPasswordCorrect;
    [SerializeField] private UnityEvent onPasswordIncorrect;
    
    [Header("Settings")]
    [SerializeField] private bool unlockOnce = true; // Se déverrouille qu'une seule fois    
    private bool isUnlocked = false;

    /// <summary>
    /// Ajoute un caractère au mot de passe en cours
    /// </summary>
    public void AddDigit(string digit)
    {
        if (isUnlocked && unlockOnce)
            return;

        currentInput += digit;

        // Vérifie si la longueur correspond
        if (currentInput.Length == password.Length)
        {
            CheckPassword();
        }
    }

    /// <summary>
    /// Efface le dernier caractère
    /// </summary>
    public void RemoveLastDigit()
    {
        if (currentInput.Length > 0)
        {
            currentInput = currentInput.Substring(0, currentInput.Length - 1);
        }
    }

    /// <summary>
    /// Réinitialise complètement l'entrée
    /// </summary>
    public void ClearInput()
    {
        currentInput = "";
    }

    /// <summary>
    /// Vérifie si le mot de passe est correct
    /// </summary>
    private void CheckPassword()
    {
        if (currentInput == password)
        {
            isUnlocked = true;
            onPasswordCorrect?.Invoke();

            // Déclenche l'événement de quête pour le cadenas déverrouillé
            if (S_GameManager.instance != null)
            {
                S_GameManager.instance.playerEvents.PadlockUnlocked();
            }
        }
        else
        {
            onPasswordIncorrect?.Invoke();
            ClearInput(); // Reset après échec
        }
    }

    /// <summary>
    /// Retourne le mot de passe actuel (pour l'affichage UI)
    /// </summary>
    public string GetCurrentInput()
    {
        return currentInput;
    }

    /// <summary>
    /// Retourne le nombre de chiffres attendus
    /// </summary>
    public int GetPasswordLength()
    {
        return password.Length;
    }

    /// <summary>
    /// Vérifie si le cadenas est déverrouillé
    /// </summary>
    public bool IsUnlocked()
    {
        return isUnlocked;
    }

    /// <summary>
    /// Réinitialise le cadenas (utile pour les tests)
    /// </summary>
    public void ResetPadlock()
    {
        isUnlocked = false;
        ClearInput();
    }
}
