using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class S_DialogueManager : MonoBehaviour
{
    //~ Instance du DialogueManager (pour l'utiliser partout)
    public static S_DialogueManager instance;

    //~ Gestion des éléments d'UI
    [Header("Gestion éléments d'UI")]
    [SerializeField] private GameObject uiContainer;
    public TextMeshProUGUI npcName;
    public TextMeshProUGUI dialogueText;
    private Queue<S_DialogueLine> lines;
    [HideInInspector] public bool isDialogueActive;

    //~ Références d'autres scripts
    [Header("Références vers d'autres scripts")]
    [SerializeField] private S_PlayerController playerController;
    [SerializeField] private S_PlayerInteract playerInteract;
    [SerializeField] private S_FirstPersonCamera firstPersonCamera;
    [SerializeField] private S_DisplayMenus displayMenu;

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
        // Active désactive des scripts
        playerController.setMovementsEnabled(false); // Mouvements
        playerInteract.setInteractionEnabled(false); // Interactions
        firstPersonCamera.setCursorEnabled(true); // Curseur
        firstPersonCamera.setRotationEnabled(false); // Rotation camera
        displayMenu.setAbleToOpenCloseMenu(false);

        uiContainer.SetActive(true); // Active le visuel

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
            yield return new WaitForSeconds(S_GameSettingsData.instance.currentTypingSpeed / 100); // Divisé par 100 car c'est plus facile de regler de 1 à 100 que 0.1 à 1
        }
    }

    private void EndDialogue() //& Termine le dialogue
    {
        // Active désactive des scripts
        playerController.setMovementsEnabled(true); // Mouvements
        playerInteract.setInteractionEnabled(true); // Interactions
        firstPersonCamera.setCursorEnabled(false); // Curseur
        firstPersonCamera.setRotationEnabled(true); // Rotation camera
        displayMenu.setAbleToOpenCloseMenu(true);

        isDialogueActive = false;
        uiContainer.SetActive(false); // Désactive le visuel
    }
}
