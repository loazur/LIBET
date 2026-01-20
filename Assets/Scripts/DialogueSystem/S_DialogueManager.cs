using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class S_DialogueManager : S_Menu
{
    //~ Instance du DialogueManager (pour l'utiliser partout)
    public static S_DialogueManager instance;

    //~ Gestion des éléments d'UI
    [Header("Gestion éléments d'UI")]
    [Tooltip("Element de l'UI contenant les visuels du dialogues, variable utile pour le cacher/montrer")]
    [SerializeField] private GameObject uiContainer;

    [Tooltip("Bouton continuer du menu de dialogue")]
    [SerializeField] private GameObject continueButton;

    public TextMeshProUGUI npcName;
    public TextMeshProUGUI dialogueText;
    private Queue<S_DialogueLine> lines;
    [HideInInspector] public bool isDialogueActive;

    void Start()
    {
        EndDialogue();
        lines = new Queue<S_DialogueLine>();

        if (instance == null)
        {
            instance = this;
        }
    }

    //! --------------- Fonctions privés ---------------

    public void StartDialogue(S_Dialogue dialogue) //& Démarre le dialogue
    {
        if (S_MenuManager.instance != null)
        {
            if (!S_MenuManager.instance.RegisterMenuOpen(S_MenuManager.MenuType.DIALOGUE))
            {
                Debug.LogWarning("[DialogueManager] Impossible de démarrer le dialogue, un menu est ouvert");
                return;
            }
        }

        uiContainer.SetActive(true); // Active le visuel

        EventSystem.current.SetSelectedGameObject(continueButton);

        lines.Clear();

        foreach (S_DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine() //& Passe à la ligne d'après
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        S_DialogueLine currentLine = lines.Dequeue();

        npcName.text = currentLine.npc.npcName;

        StopAllCoroutines();

        StartCoroutine(TypeSentence(currentLine));
    }

    private IEnumerator TypeSentence(S_DialogueLine dialogueLine) //& Ecrit une ligne
    {
        dialogueText.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(S_GameUserData.instance.currentTypingSpeed / 100); // Divisé par 100 car c'est plus facile de regler de 1 à 100 que 0.1 à 1
        }
    }

    private void EndDialogue() //& Termine le dialogue
    {
        if (S_MenuManager.instance != null)
        {
            S_MenuManager.instance.RegisterMenuClose(S_MenuManager.MenuType.DIALOGUE);
        }

        isDialogueActive = false;
        uiContainer.SetActive(false); // Désactive le visuel
    }

}
