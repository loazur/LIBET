using UnityEngine;

public class S_GameManager : MonoBehaviour
{
    public static S_GameManager instance { get; private set; }

    public InputEvents inputEvents;

    public S_QuestEvent questEvents;

    public PlayerEvents playerEvents;


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
        inputEvents = new InputEvents();
        questEvents = new S_QuestEvent();
        playerEvents = new PlayerEvents();
    }
}
