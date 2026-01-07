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

    [field: Header("Musics")]
    [field: SerializeField] public EventReference music {get; private set;}
    [field: SerializeField] public EventReference piano {get; private set;}


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

}
