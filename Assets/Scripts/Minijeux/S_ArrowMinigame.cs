using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mécanique du mini-jeu Arrow. Peut être placé sur plusieurs objets.
/// Délègue tout l'affichage au singleton S_ArrowMinigameUI.
/// </summary>
public class S_ArrowMinigame : S_AbstractMinigame
{
    [Header("Sequence Settings")]
    [SerializeField] private int minSequenceLength = 4;
    [SerializeField] private int maxSequenceLength = 6;
    [SerializeField] private float gameDuration = 5f;

    private List<int> sequence = new List<int>();
    private int currentIndex = 0;
    private int sequenceLength = 0;
    private float timeRemaining;
    private bool isPlaying = false;

    private Vector2 lastMoveInput = Vector2.zero;

    private S_ArrowMinigameUI ui;

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
        ui = S_ArrowMinigameUI.instance;

        if (ui == null)
        {
            Debug.LogError("[ArrowMinigame] S_ArrowMinigameUI.instance est null ! Assurez-vous qu'un GameObject avec S_ArrowMinigameUI existe dans la scène.");
            return;
        }

        Debug.Log("[ArrowMinigame] Minijeu commencé!");

        GenerateRandomSequence();

        ui.GenerateArrowsUI(sequenceLength);
        ui.Show();
        ui.DisplaySequence(sequence);
        ui.ResetInputDisplay();

        currentIndex = 0;
        timeRemaining = gameDuration;
        isPlaying = true;
    }

    private void GenerateRandomSequence()
    {
        sequence.Clear();
        sequenceLength = Random.Range(minSequenceLength, maxSequenceLength + 1);

        for (int i = 0; i < sequenceLength; i++)
        {
            sequence.Add(Random.Range(0, 4)); // 0=haut, 1=droite, 2=bas, 3=gauche
        }

        Debug.Log("[ArrowMinigame] Séquence générée (longueur " + sequenceLength + ") : " + string.Join(", ", sequence));
    }

    private void EndMinigame()
    {
        isPlaying = false;

        if (ui != null)
        {
            ui.ClearArrowsUI();
            ui.Hide();
        }

        if (S_MenuManager.instance != null)
        {
            S_MenuManager.instance.RegisterMenuClose(S_MenuManager.MenuType.MINIGAME);
        }
    }

    private void Update()
    {
        if (!isPlaying) return;

        // Gestion du timer
        timeRemaining -= Time.deltaTime;
        ui.UpdateTimer(timeRemaining);

        if (timeRemaining <= 0)
        {
            EndMinigame();
            TriggerLose();
            return;
        }

        // Détection des inputs (détecter uniquement le moment de la pression)
        Vector2 currentMoveInput = S_UserInput.instance.MoveInput;

        if (currentMoveInput.y == 1 && lastMoveInput.y != 1)
        {
            CheckInput(0);
        }
        else if (currentMoveInput.x == 1 && lastMoveInput.x != 1)
        {
            CheckInput(1);
        }
        else if (currentMoveInput.y == -1 && lastMoveInput.y != -1)
        {
            CheckInput(2);
        }
        else if (currentMoveInput.x == -1 && lastMoveInput.x != -1)
        {
            CheckInput(3);
        }

        lastMoveInput = currentMoveInput;
    }

    private void CheckInput(int arrowIndex)
    {
        if (arrowIndex == sequence[currentIndex])
        {
            // Bonne flèche
            ui.MarkInputCorrect(currentIndex, arrowIndex);
            ui.MarkSequenceValidated(currentIndex);

            currentIndex++;

            if (currentIndex >= sequence.Count)
            {
                EndMinigame();
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
        ui.MarkInputError(currentIndex, arrowIndex);

        yield return new WaitForSeconds(0.5f);

        ui.DisplaySequence(sequence);
        ui.ResetInputDisplay();

        currentIndex = 0;
    }
}
