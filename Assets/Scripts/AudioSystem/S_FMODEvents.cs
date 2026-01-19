using FMODUnity;
using UnityEngine;

public class S_FMODEvents : MonoBehaviour
{
    //! S_FMODEvents contient tout les events de FMOD qui peuvent etre ainsi récupérer partout
    public static S_FMODEvents instance {get; private set;}

    [field: Header("L'objet qui entend le son (joueur, ou Camera dans le MainMenu)")]
    [field: SerializeField] public Transform target {get; private set;}

    [field: Header("Doors SFX")]
    [field: SerializeField] public EventReference doorOpening {get; private set;}
    [field: SerializeField] public EventReference doorClosing {get; private set;}
    [field: SerializeField] public EventReference doorLocked {get; private set;}
    [field: SerializeField] public EventReference doorUnlock {get; private set;}

    [field: Header("Musics")]
    [field: SerializeField] public EventReference music {get; private set;}
    [field: SerializeField] public EventReference piano {get; private set;}

    [field: Header("Player")]
    [field: SerializeField] public EventReference footsteps {get; private set;}

    [field: Header("Notes")]
    [field: SerializeField] public EventReference noteOpen {get; private set;}
    [field: SerializeField] public EventReference noteClose {get; private set;}
    [field: SerializeField] public EventReference noteTurnPage {get; private set;}

    [field: Header("Menus SFX")]
    [field: SerializeField] public EventReference ui_button_confirm {get; private set;}
    [field: SerializeField] public EventReference ui_change_selection {get; private set;}
    [field: SerializeField] public EventReference ui_option_click {get; private set;}
    [field: SerializeField] public EventReference ui_return {get; private set;}

    void Awake()
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
    }

}
