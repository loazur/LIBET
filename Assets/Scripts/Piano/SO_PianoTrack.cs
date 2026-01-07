using UnityEngine;

[CreateAssetMenu(menuName = "Audio/Piano Track")]
public class SO_PianoTrack : ScriptableObject
{
    [Header("Track Info")]
    public string trackName;
    public string author;
    public S_PianoTrack track;
}
