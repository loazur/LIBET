using UnityEngine;

public class S_GameManager : MonoBehaviour
{
    internal static object instance;
    public S_QuestEvent questEvent;


    private void Awake()
    {
        questEvent = new S_QuestEvent();
    }
}
