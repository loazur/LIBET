using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class S_MinijeuRepetition : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private TextMeshProUGUI feedbackText;
    
    [Header("Visual Feedback")]
    [SerializeField] private Image buttonBackground;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color clickedColor = Color.green;
    [SerializeField] private float flashDuration = 0.15f;
    
    [Header("Coherence Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float coherenceRate = 0.8f; // 80% de cohérence au début
    [Tooltip("Si true, le nombre de clics est toujours cohérent (pour debug)")]
    [SerializeField] private bool alwaysCoherent = false;
    
    [Header("Click Settings")]
    [SerializeField] private int minClicks = 1;
    [SerializeField] private int maxClicks = 5;
    [Tooltip("Nombre de clics 'logique' attendu")]
    [SerializeField] private int expectedClicks = 1;
    
    [Header("Reopen Settings")]
    [Tooltip("Probabilité que le menu se ferme et se rouvre (0-1)")]
    [Range(0f, 1f)]
    [SerializeField] private float reopenProbability = 0.7f;
    [Tooltip("Délai minimum avant de rouvrir le menu")]
    [SerializeField] private float reopenDelayMin = 0.5f;
    [Tooltip("Délai maximum avant de rouvrir le menu")]
    [SerializeField] private float reopenDelayMax = 3f;
    [Tooltip("Nombre maximum de réouvertures")]
    [SerializeField] private int maxReopens = 3;
    
    [Header("Texts")]
    [SerializeField] private string[] questionTextsFR = { 
        "Veuillez cliquer.",           // 0: Première ouverture - Formel
        "Encore ?",                    // 1: Première réouverture - Doute
        "J'ai pas déjà cliqué ?",      // 2: Deuxième réouverture - Confusion
        "Mais j'ai cliqué non ?",      // 3: Troisième réouverture - Interrogation
        "Combien de fois ?",           // 4: Quatrième - Perte de compte
        "C'était pas fini ?",          // 5: Cinquième - Doute du passé
        "Je dois encore cliquer ?",    // 6: Sixième - Résignation
        "Pourquoi encore ?",           // 7: Septième - Incompréhension
        "C'est vraiment pas fini ?",   // 8: Huitième - Frustration
        "Mais... j'ai cliqué.",        // 9: Neuvième - Certitude floue
        "Encore une fois ?",           // 10: Dixième - Lassitude
        "C'est quoi le problème ?"     // 11: Onzième+ - Désorientation totale
    };
    [SerializeField] private string[] questionTextsEN = { 
        "Please click.",                  // 0: First opening - Formal
        "Again?",                         // 1: First reopen - Doubt
        "Haven't I already clicked?",     // 2: Second reopen - Confusion
        "But I clicked, didn't I?",       // 3: Third reopen - Question
        "How many times?",                // 4: Fourth - Lost count
        "Wasn't it done?",                // 5: Fifth - Past doubt
        "Do I still need to click?",      // 6: Sixth - Resignation
        "Why again?",                     // 7: Seventh - Incomprehension
        "Is it really not done?",         // 8: Eighth - Frustration
        "But... I clicked.",              // 9: Ninth - Blurred certainty
        "One more time?",                 // 10: Tenth - Weariness
        "What's the problem?"             // 11: Eleventh+ - Total disorientation
    };
    [SerializeField] private string buttonTextFR = "Oui";
    [SerializeField] private string buttonTextEN = "Yes";
    
    // Privates
    private int requiredClicks = 1;
    private int currentClicks = 0;
    private bool isActive = false;
    private Action<bool> onComplete;
    
    // ✅ Session persistence
    private int sessionRequiredClicks = 1;
    private bool sessionInitialized = false;
    private int reopenCount = 0;
    private int currentTextIndex = 0; // ✅ Index linéaire pour les textes
    
    private void Awake()
    {
        if (minigamePanel != null)
            minigamePanel.SetActive(false);
            
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
            
        if (feedbackText != null)
            feedbackText.text = "";
    }
    
    /// <summary>
    /// Démarre le mini-jeu
    /// </summary>
    /// <param name="onCompleteCallback">Callback appelé à la fin (true = succès)</param>
    public void StartMinigame(Action<bool> onCompleteCallback)
    {
        if (S_MenuManager.instance != null)
        {
            if (!S_MenuManager.instance.RegisterMenuOpen(S_MenuManager.MenuType.MINIGAME))
            {
                Debug.LogWarning("[MenuMinijeu] Impossible de démarrer le menu minigame, un menu est ouvert");
                return;
            }
        }

        onComplete = onCompleteCallback;
        
        // ✅ Décide du nombre de clics SEULEMENT la première fois
        if (!sessionInitialized)
        {
            if (alwaysCoherent)
            {
                sessionRequiredClicks = expectedClicks; // Mode cohérent
            }
            else
            {
                float random = UnityEngine.Random.value;
                
                if (random <= coherenceRate)
                {
                    // Comportement cohérent
                    sessionRequiredClicks = expectedClicks;
                }
                else
                {
                    // Comportement incohérent : nombre aléatoire
                    sessionRequiredClicks = UnityEngine.Random.Range(minClicks, maxClicks + 1);
                }
            }
            
            sessionInitialized = true;
            reopenCount = 0;
            currentTextIndex = 0; // ✅ Commence à l'index 0
            Debug.Log($"[MinijeuRepetition] Session initialisée (cohérence: {coherenceRate * 100}%, requis: {sessionRequiredClicks} clics)");
        }
        
        // Réinitialise les variables de tentative
        currentClicks = 0;
        requiredClicks = sessionRequiredClicks; // ✅ Utilise le nombre de session
        isActive = true;
        
        // Affiche l'UI
        if (minigamePanel != null)
            minigamePanel.SetActive(true);
            
        UpdateTexts();
            
        if (feedbackText != null)
            feedbackText.text = "";
            
        if (confirmButton != null)
            confirmButton.interactable = true;
            
        if (buttonBackground != null)
            buttonBackground.color = normalColor;

        //DisableMovements();
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        Debug.Log($"[MinijeuRepetition] Démarré ({currentClicks}/{requiredClicks} clics)");
    }
    
    /// <summary>
    /// Met à jour les textes selon la langue
    /// </summary>
    private void UpdateTexts()
    {
        if (questionText != null)
        {
            // ✅ Sélectionne les textes selon la langue
            string[] texts = S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French 
                ? questionTextsFR 
                : questionTextsEN;
            
            // ✅ Utilise l'index actuel (boucle si dépasse le nombre de textes)
            int index = currentTextIndex % texts.Length;
            questionText.text = texts[index];
            
            Debug.Log($"[MinijeuRepetition] Texte affiché (index {currentTextIndex}): {texts[index]}");
        }
        
        if (confirmButton != null)
        {
            TextMeshProUGUI buttonText = confirmButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French 
                    ? buttonTextFR 
                    : buttonTextEN;
            }
        }
    }
    
    /// <summary>
    /// Appelé quand le joueur clique sur "Oui"
    /// </summary>
    private void OnConfirmButtonClicked()
    {
        if (!isActive)
            return;
            
        currentClicks++;
        
        Debug.Log($"[MinijeuRepetition] Clic {currentClicks}/{requiredClicks}");
        
        //PlaySound(clickSound);
        
        // ✅ Feedback visuel IMMÉDIAT
        StartCoroutine(FlashButton());
        
        // Vérifie si on a atteint le nombre requis
        if (currentClicks >= requiredClicks)
        {
            // ✅ Décide si on ferme/rouvre ou si on valide définitivement
            if (ShouldReopen())
            {
                StartCoroutine(ReopenMenu());
            }
            else
            {
                ValidateAction();
            }
        }
    }
    
    /// <summary>
    /// Détermine si le menu doit se fermer et se rouvrir
    /// </summary>
    private bool ShouldReopen()
    {
        // Ne pas rouvrir si on a dépassé le max
        if (reopenCount >= maxReopens)
            return false;
        
        // ✅ Probabilité diminue avec le nombre de réouvertures déjà faites
        float adjustedProbability = reopenProbability - (reopenCount * 0.2f);
        adjustedProbability = Mathf.Max(adjustedProbability, 0.3f); // Minimum 30%
        
        // Tire au sort selon la probabilité ajustée
        float random = UnityEngine.Random.value;
        return random < adjustedProbability;
    }
    
    /// <summary>
    /// Ferme et rouvre le menu (confusion)
    /// </summary>
    private IEnumerator ReopenMenu()
    {
        reopenCount++;
        currentTextIndex++; // ✅ Passe au texte suivant
        isActive = false;
        
        Debug.Log($"[MinijeuRepetition] Fermeture/Réouverture ({reopenCount}/{maxReopens})");
        
        // Message temporaire
        if (feedbackText != null)
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                feedbackText.text = "C'est bon.";
            }
            else
            {
                feedbackText.text = "Done.";
            }
        }
        
        if (confirmButton != null)
            confirmButton.interactable = false;
        
        // Attend un peu
        yield return new WaitForSeconds(0.5f);
        
        // ✅ FERME le menu (sans réactiver le joueur)
        if (minigamePanel != null)
            minigamePanel.SetActive(false);
        
        // ✅ Attend un délai ALÉATOIRE entre min et max
        float randomDelay = UnityEngine.Random.Range(reopenDelayMin, reopenDelayMax);
        Debug.Log($"[MinijeuRepetition] Attente de {randomDelay:F2}s avant réouverture...");
        yield return new WaitForSeconds(randomDelay);
        
        // ✅ ROUVRE le menu
        if (minigamePanel != null)
            minigamePanel.SetActive(true);
            
        // Réinitialise l'UI (mais pas le compteur de clics requis)
        currentClicks = 0;
        isActive = true;
        
        // ✅ Met à jour avec un NOUVEAU texte ALÉATOIRE
        UpdateTexts();
        
        if (feedbackText != null)
            feedbackText.text = "";
            
        if (confirmButton != null)
            confirmButton.interactable = true;
            
        if (buttonBackground != null)
            buttonBackground.color = normalColor;
        
        Debug.Log($"[MinijeuRepetition] Menu rouvert (il faut re-cliquer {requiredClicks} fois)");
    }
    
    /// <summary>
    /// Flash visuel du bouton (feedback immédiat)
    /// </summary>
    private IEnumerator FlashButton()
    {
        if (buttonBackground == null)
            yield break;
            
        buttonBackground.color = clickedColor;
        yield return new WaitForSeconds(flashDuration);
        buttonBackground.color = normalColor;
    }
    
    /// <summary>
    /// Valide l'action et ferme le mini-jeu
    /// </summary>
    private void ValidateAction()
    {
        isActive = false;
        
        if (confirmButton != null)
            confirmButton.interactable = false;
        
        // Message de validation
        if (feedbackText != null)
        {
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                feedbackText.text = "C'est bon.";
            }
            else
            {
                feedbackText.text = "Done.";
            }
        }
        
        //PlaySound(successSound);
        
        // Ferme après un court délai
        StartCoroutine(CloseAfterDelay());
    }
    
    /// <summary>
    /// Ferme le mini-jeu après validation
    /// </summary>
    private IEnumerator CloseAfterDelay()
    {
        yield return new WaitForSeconds(1f);

        if (S_MenuManager.instance != null)
        {
            S_MenuManager.instance.RegisterMenuClose(S_MenuManager.MenuType.MINIGAME);
        }
        
        // Ferme l'UI
        if (minigamePanel != null)
            minigamePanel.SetActive(false);
        
        //EnableMovements();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // ✅ Réinitialise la session après succès
        sessionInitialized = false;
        
        // Appelle le callback
        onComplete?.Invoke(true);
        
        Debug.Log($"[MinijeuRepetition] Terminé avec succès ({currentClicks} clics, {reopenCount} réouvertures)");

        S_GameManager.instance.playerEvents.DrawerUnlock("drawer_office");
    }
    
    /// <summary>
    /// Change le taux de cohérence (appelé selon la progression)
    /// </summary>
    public void SetCoherenceRate(float rate)
    {
        coherenceRate = Mathf.Clamp01(rate);
        Debug.Log($"[MinijeuRepetition] Cohérence changée à {coherenceRate * 100}%");
    }

    //private void PlaySound(AudioClip clip)
    //{
    //    if (audioSource != null && clip != null)
    //    {
    //        audioSource.PlayOneShot(clip);
    //    }
    //}

    private void OnDestroy()
    {
        if (confirmButton != null)
            confirmButton.onClick.RemoveListener(OnConfirmButtonClicked);
    }
}
