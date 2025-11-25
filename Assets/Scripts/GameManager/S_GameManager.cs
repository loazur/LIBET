using UnityEngine;

public class S_GameManager : MonoBehaviour
{
    public static S_GameManager instance { get; private set; }

    public InputEvents inputEvents;

    public S_QuestEvent questEvents;

    public PlayerEvents playerEvents;


    private void Awake()
    {
        Debug.Log("S_GameManager Awake called.");
        if (instance != null)
        {
            Debug.LogError("Found more than one Game Events Manager in the scene.");
        }
        instance = this;

        // initialize all events
        inputEvents = new InputEvents();
        questEvents = new S_QuestEvent();
        playerEvents = new PlayerEvents();
    }
}
