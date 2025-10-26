using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class S_DialogueNPC
{
    public string npcName;
}

[System.Serializable]
public class S_DialogueLine
{
    public S_DialogueNPC npc;
    [TextArea(3, 10)] public string line;
}

[System.Serializable]
public class S_Dialogue
{
    public List<S_DialogueLine> dialogueLines = new List<S_DialogueLine>();
}

public class S_DialogueTrigger : MonoBehaviour
{
    public S_Dialogue dialogue;

    public void TriggerDialogue()
    {
        S_DialogueManager.Instance.StartDialogue(dialogue);
    }
}
