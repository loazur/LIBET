using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class S_DialogueManager : MonoBehaviour
{
    public static S_DialogueManager instance;

    //~ Gestion des éléments d'UI
    public GameObject containerGameObject;
    public TextMeshProUGUI npcName;
    public TextMeshProUGUI dialogueText;

    private Queue<S_DialogueLine> lines;

    public bool isDialogueActive;
    public float typingSpeed = 0.2f;


    void Start()
    {
        HideUI();

        if (instance == null)
        {
            instance = this;

        }
    }


    //! --------------- Fonctions privés ---------------

    public void StartDialogue(S_Dialogue dialogue)
    {
        ShowUI();

        lines.Clear();

        foreach (S_DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            HideUI();
            return;
        }

        S_DialogueLine currentLine = lines.Dequeue();

        npcName.text = currentLine.npc.npcName;

        StopAllCoroutines();

        StartCoroutine(TypeSentence(currentLine));
    }

    private IEnumerator TypeSentence(S_DialogueLine dialogueLine)
    {
        dialogueText.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }



    //
    private void ShowUI() //& Affiche l'UI
    {
        isDialogueActive = true;
        containerGameObject.SetActive(true);
    }
    
    private void HideUI() //& Cache l'UI
    {
        isDialogueActive = false;
        containerGameObject.SetActive(false);
    }
}
