using UnityEngine;

public class S_GameManager : MonoBehaviour
{
    internal static object instance;
    public S_QuestEvent questEvent;
    internal object inputEvent;

    private void Awake()
    {
        questEvent = new S_QuestEvent();
    }
}
