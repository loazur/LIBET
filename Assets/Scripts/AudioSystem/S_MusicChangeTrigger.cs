using UnityEngine;

public class S_MusicChangeTrigger : MonoBehaviour
{
    [Header("Area")]
    [SerializeField] private E_MusicArea area;

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag.Equals("Player"))
        {
            S_AudioManager.instance.SetMusicArea(area);
        }
    }
}
