using UnityEngine;
using FMODUnity;

[CreateAssetMenu(menuName = "Audio/Piano Track")]
public class SO_PianoTrack : ScriptableObject
{
    public EventReference musicEvent;

    [Header("UI Info")]
    public string trackName;
    public string author;
}
