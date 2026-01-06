using FMODUnity;
using UnityEngine;

public class S_FMODEvents : MonoBehaviour
{
    //! S_FMODEvents contient tout les events de FMOD qui peuvent etre ainsi récupérer partout
    public static S_FMODEvents instance {get; private set;}

    [field: Header("Doors SFX")]
    [field: SerializeField] public EventReference doorOpening {get; private set;}
    [field: SerializeField] public EventReference doorClosing {get; private set;}
    [field: SerializeField] public EventReference doorLocked {get; private set;}
    [field: SerializeField] public EventReference doorUnlock {get; private set;}

    [field: Header("Music")]
    [field: SerializeField] public EventReference musicTest {get; private set;}

    [field: Header("Music Piano")]
    [field: SerializeField] public EventReference MscPiano1 {get; private set;}
    [field: SerializeField] public EventReference MscPiano2 {get; private set;}
    [field: SerializeField] public EventReference MscPiano3 {get; private set;}
    [field: SerializeField] public EventReference MscPiano4 {get; private set;}
    [field: SerializeField] public EventReference MscPiano5 {get; private set;}

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public EventReference GetRandomPiano()
{
    EventReference[] pianoTracks =
    {
        MscPiano1,
        MscPiano2,
        MscPiano3,
        MscPiano4,
        MscPiano5
    };

    int index = Random.Range(0, pianoTracks.Length);
    return pianoTracks[index];
}

}
