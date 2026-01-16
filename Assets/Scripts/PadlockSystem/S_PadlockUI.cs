using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_PadlockUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private S_Padlock padlock;
    
    [Header("Display")]
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private char hiddenChar = '*'; // Caractère pour masquer le mot de passe
    [SerializeField] private bool showRealPassword = false; // Affiche le vrai mot de passe (debug)

    
    private void Start()
    {
        if (padlock == null)
        {
            Debug.LogError("[PadlockUI] Aucun S_Padlock assigné !");
            return;
        }

        // Initialise l'affichage
        UpdateDisplay();
        
    }

    private void Update()
    {
        // Met à jour l'affichage en temps réel
        UpdateDisplay();
    }

    /// <summary>
    /// Met à jour l'affichage du mot de passe
    /// </summary>
    private void UpdateDisplay()
    {
        if (displayText == null || padlock == null)
            return;

        string input = padlock.GetCurrentInput();
        int passwordLength = padlock.GetPasswordLength();

        if (showRealPassword)
        {
            // Mode debug : affiche le vrai mot de passe
            displayText.text = input.PadRight(passwordLength, '_');
        }
        else
        {
            // Mode normal : masque avec des *
            string masked = new string(hiddenChar, input.Length);
            displayText.text = masked.PadRight(passwordLength, '_');
        }
    }

    /// <summary>
    /// Appelé par les boutons numériques (0-9)
    /// </summary>
    public void OnDigitPressed(string digit)
    {
        if (padlock.IsUnlocked())
            return;

        padlock.AddDigit(digit);
    }

    /// <summary>
    /// Appelé par le bouton "Effacer"
    /// </summary>
    public void OnDeletePressed()
    {
        padlock.RemoveLastDigit();
    }

    /// <summary>
    /// Appelé par le bouton "Reset"
    /// </summary>
    public void OnClearPressed()
    {
        padlock.ClearInput();
    }

}
