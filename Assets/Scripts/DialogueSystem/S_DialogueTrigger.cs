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
    [Tooltip("Dialogue Unique")]
    public bool oneShot = false; 

    [Tooltip("Dialogue en français")]
    public List<S_Dialogue> dialoguesFrench = new List<S_Dialogue>();

    [Tooltip("Dialogue en anglais")]
    public List<S_Dialogue> dialoguesEnglish = new List<S_Dialogue>();

    private int howManyTimeTalkedTo = 0; // Combien de fois le joueur a parler avec

    public void TriggerDialogueFrench() //& Lance le dialogue en français
    {
        if (oneShot && howManyTimeTalkedTo >= dialoguesFrench.Count) return; // Fin du dialogue

        if (howManyTimeTalkedTo < dialoguesEnglish.Count) // Vérification en fonction du nombre de dialogue
            S_DialogueManager.instance.StartDialogue(dialoguesFrench[howManyTimeTalkedTo]);

        if (howManyTimeTalkedTo < dialoguesFrench.Count - 1) // Si il reste toujours des choses à dire après lui avoir parler
            ++howManyTimeTalkedTo;
        else if (oneShot) // Si c'est le dernier dialogue et oneShot est actif
            ++howManyTimeTalkedTo;
    }

    public void TriggerDialogueEnglish() //& Lance le dialogue en anglais
    {
        if (oneShot && howManyTimeTalkedTo >= dialoguesEnglish.Count) return; // Fin du dialogue

        if (howManyTimeTalkedTo < dialoguesEnglish.Count) // Vérification en fonction du nombre de dialogue
            S_DialogueManager.instance.StartDialogue(dialoguesEnglish[howManyTimeTalkedTo]);

        if (howManyTimeTalkedTo < dialoguesEnglish.Count - 1) // Si il reste toujours des choses à dire après lui avoir parler
            ++howManyTimeTalkedTo;
        else if (oneShot) // Si c'est le dernier dialogue et oneShot est actif
            ++howManyTimeTalkedTo;
    }
}
