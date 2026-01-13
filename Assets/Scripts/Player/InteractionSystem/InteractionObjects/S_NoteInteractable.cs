using UnityEngine;

public class S_NoteInteractable : MonoBehaviour, SI_Interactable
{
    //~ Gestion de la note
    [Header("Gestion de la note")]
    [SerializeField] private S_Note note = null;

    [SerializeField] private bool autoDisplay = false;
    [SerializeField] private bool add = true;

    [SerializeField] private string interactText = "not_set"; // Texte à afficher

    void Start() //& Initialize la montre
    {
        UpdateInteractText(); // Setup
        
        S_GameUserData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    //! Méthodes provenant de l'interface SI_Interactable

    public void Interact(Transform playerTransform)
    {
        Debug.Log("Note ramassé!");

        if (autoDisplay)
        {
            S_NotesSystem.Display(note);
        }
        if (add)
        {
            S_NotesSystem.AddNote(note.label, note);
            Destroy(gameObject);
        }
    }

    public string getInteractText() => interactText; //& Texte affiché sur l'UI
    public Transform getTransform() => transform; //& Position de la montre
    
    //!---------------------------------------------

    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            interactText = "Ramasser";
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            interactText = "Pickup";
        }
    }
}
