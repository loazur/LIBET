using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S_DialogueNPC //& Informations du NPC
{
    public string npcName;
}

[System.Serializable]
public class S_DialogueLine //& Informations d'une ligne de Dialogue
{
    public S_DialogueNPC npc;
    [TextArea(3, 3)] public string line;
}

[System.Serializable]
public class S_Dialogue //& Contient les lignes de dialogues
{
    public List<S_DialogueLine> dialogueLines = new List<S_DialogueLine>();
}

public class S_DialogueTrigger : MonoBehaviour //& Permet de lancer le dialogue et contient le dialogue
{
    [Tooltip("Dialogue en français")]
    public S_Dialogue dialogueFrench;

    [Tooltip("Dialogue en anglais")]
    public S_Dialogue dialogueEnglish;

    public void TriggerDialogueFrench() //& Lance le dialogue en français
    {
        S_DialogueManager.instance.StartDialogue(dialogueFrench);
    }

    public void TriggerDialogueEnglish() //& Lance le dialogue en anglais
    {
        S_DialogueManager.instance.StartDialogue(dialogueEnglish);
    }
}
