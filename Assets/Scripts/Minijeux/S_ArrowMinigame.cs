using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class S_ArrowMinigame : S_AbstractMinigame
{
    [Header("UI References")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private Transform sequenceContainer; // Container des flèches de séquence
    [SerializeField] private Transform inputContainer; // Container des cases d'input
    [SerializeField] private GameObject arrowImagePrefab; // Prefab d'une Image de flèche
    
    [Header("Arrow Sprites")]
    [SerializeField] private Sprite arrowUpSprite;
    [SerializeField] private Sprite arrowRightSprite;
    [SerializeField] private Sprite arrowDownSprite;
    [SerializeField] private Sprite arrowLeftSprite;
    
    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.green;
    [SerializeField] private Color errorColor = Color.red;
    [SerializeField] private Color emptyColor = new Color(1, 1, 1, 0.3f);
    
    [Header("Sequence Settings")]
    [SerializeField] private int minSequenceLength = 4;
    [SerializeField] private int maxSequenceLength = 6;
    
    private List<int> sequence = new List<int>(); // Séquence générée aléatoirement
    private List<Image> sequenceArrows = new List<Image>();
    private List<Image> inputArrows = new List<Image>();
    private int currentIndex = 0;
    private int sequenceLength = 0; // Longueur de la séquence actuelle
    private float timeRemaining = 5f;
    private bool isPlaying = false;

    private Vector2 lastMoveInput = Vector2.zero;

    public override void TriggerMinigame()
    {
        if (S_MenuManager.instance != null)
        {
            if (!S_MenuManager.instance.RegisterMenuOpen(S_MenuManager.MenuType.MINIGAME))
            {
                Debug.LogWarning("[ArrowMinigame] Impossible de démarrer le menu ArrowMinigame, un menu est ouvert");
                return;
            }
        }

        StartMinigame();
    }

    private void StartMinigame()
    {
        Debug.Log("Minijeu commencé!");

        minigamePanel.SetActive(true);
        
        // Générer une nouvelle séquence aléatoire
        GenerateRandomSequence();
        
        // Régénérer les UI à chaque fois pour s'adapter à la nouvelle taille
        GenerateArrowsUI();
        
        currentIndex = 0;
        timeRemaining = 5f;
        isPlaying = true;

        // Afficher la séquence à reproduire
        DisplaySequence();
        
        // Réinitialiser les cases d'input (vides et transparentes)
        ResetInputDisplay();
    }

    private void GenerateRandomSequence()
    {
        sequence.Clear();

        // Taille de la séquence aléatoire entre min et max (inclus)
        sequenceLength = Random.Range(minSequenceLength, maxSequenceLength + 1);

        // Ajout des touches aléatoires
        for (int i = 0; i < sequenceLength; i++)
        {
            sequence.Add(Random.Range(0, 4)); // 0=haut, 1=droite, 2=bas, 3=gauche
        }
        
        Debug.Log("Nouvelle séquence générée (longueur " + sequenceLength + ") : " + string.Join(", ", sequence));
    }

    private void GenerateArrowsUI()
    {
        // Nettoyer les anciennes flèches si elles existent
        foreach (var arrow in sequenceArrows)
        {
            if (arrow != null) Destroy(arrow.gameObject);
        }
        foreach (var arrow in inputArrows)
        {
            if (arrow != null) Destroy(arrow.gameObject);
        }
        
        sequenceArrows.Clear();
        inputArrows.Clear();
        
        // Créer les flèches de séquence (nombre basé sur sequenceLength)
        for (int i = 0; i < sequenceLength; i++)
        {
            GameObject arrowObj = Instantiate(arrowImagePrefab, sequenceContainer);
            Image arrowImg = arrowObj.GetComponent<Image>();
            sequenceArrows.Add(arrowImg);
        }
        
        // Créer les cases d'input
        for (int i = 0; i < sequenceLength; i++)
        {
            GameObject arrowObj = Instantiate(arrowImagePrefab, inputContainer);
            Image arrowImg = arrowObj.GetComponent<Image>();
            inputArrows.Add(arrowImg);
        }
    }

    private void DisplaySequence()
    {
        for (int i = 0; i < sequenceArrows.Count && i < sequence.Count; i++)
        {
            sequenceArrows[i].sprite = GetArrowSprite(sequence[i]);
            sequenceArrows[i].color = normalColor;
            sequenceArrows[i].gameObject.SetActive(true);
        }
    }

    private void ResetInputDisplay()
    {
        foreach (var inputArrow in inputArrows)
        {
            inputArrow.color = emptyColor;
            inputArrow.sprite = null;
            inputArrow.gameObject.SetActive(true);
        }
    }

    private Sprite GetArrowSprite(int direction)
    {
        switch (direction)
        {
            case 0: return arrowUpSprite;
            case 1: return arrowRightSprite;
            case 2: return arrowDownSprite;
            case 3: return arrowLeftSprite;
            default: return null;
        }
    }

    private void Update()
    {
        if (!isPlaying) return;

        // Gestion du timer
        timeRemaining -= Time.deltaTime;
        timerText.text = Mathf.Ceil(timeRemaining).ToString() + "s";
        
        if (timeRemaining <= 0)
        {
            isPlaying = false;
            minigamePanel.SetActive(false);

            if (S_MenuManager.instance != null) 
            {
                S_MenuManager.instance.RegisterMenuClose(S_MenuManager.MenuType.MINIGAME);
            }

            TriggerLose();
            return;
        }

        // Détection des inputs (détecter uniquement le moment de la pression)
        Vector2 currentMoveInput = S_UserInput.instance.MoveInput;
        
        // Haut
        if (currentMoveInput.y == 1 && lastMoveInput.y != 1)
        {
            CheckInput(0);
        }
        // Droite
        else if (currentMoveInput.x == 1 && lastMoveInput.x != 1)
        {
            CheckInput(1);
        }
        // Bas
        else if (currentMoveInput.y == -1 && lastMoveInput.y != -1)
        {
            CheckInput(2);
        }
        // Gauche
        else if (currentMoveInput.x == -1 && lastMoveInput.x != -1)
        {
            CheckInput(3);
        }
        
        lastMoveInput = currentMoveInput;
    }

    private void CheckInput(int arrowIndex)
    {
        // Vérifier si c'est la bonne flèche
        if (arrowIndex == sequence[currentIndex])
        {
            // Bonne flèche - afficher dans la case d'input
            inputArrows[currentIndex].sprite = GetArrowSprite(arrowIndex);
            inputArrows[currentIndex].color = highlightColor;
            
            // Mettre en surbrillance la flèche validée dans la séquence
            sequenceArrows[currentIndex].color = highlightColor;
            
            currentIndex++;

            // Vérifier si la séquence est terminée
            if (currentIndex >= sequence.Count)
            {
                isPlaying = false;
                minigamePanel.SetActive(false);
                
                if (S_MenuManager.instance != null) 
                {
                    S_MenuManager.instance.RegisterMenuClose(S_MenuManager.MenuType.MINIGAME);
                }
                
                TriggerWin();
            }
        }
        else
        {
            // Mauvaise flèche - redémarrer la séquence
            StartCoroutine(ShowErrorAndRestart(arrowIndex));
        }
    }

    private IEnumerator ShowErrorAndRestart(int arrowIndex)
    {
        // Afficher l'erreur dans la case actuelle
        if (currentIndex < inputArrows.Count)
        {
            inputArrows[currentIndex].sprite = GetArrowSprite(arrowIndex);
            inputArrows[currentIndex].color = errorColor;
        }
        
        yield return new WaitForSeconds(0.5f);
        
        // Réinitialiser l'affichage de la séquence
        DisplaySequence();
        
        // Réinitialiser les cases d'input
        ResetInputDisplay();
        
        // Redémarrer la séquence depuis le début
        currentIndex = 0;
    }
}
