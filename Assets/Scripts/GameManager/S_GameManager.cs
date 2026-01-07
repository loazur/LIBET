using UnityEngine;

public class S_GameManager : MonoBehaviour
{
    public static S_GameManager instance { get; private set; }

    public S_InputEvents inputEvents;

    public S_QuestEvent questEvents;

    public S_PlayerEvents playerEvents;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // initialize all events
        inputEvents = new S_InputEvents();
        questEvents = new S_QuestEvent();
        playerEvents = new S_PlayerEvents();
    }
}
