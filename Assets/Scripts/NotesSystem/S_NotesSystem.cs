using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct UIElements
{
    [SerializeField] private TextMeshProUGUI _textObj;
    public TextMeshProUGUI textObj {get {return _textObj;}}

    [SerializeField] private TextMeshProUGUI _subscript;
    public TextMeshProUGUI subscript {get {return _subscript;}}

    [SerializeField] private CanvasGroup _subscriptGroup;
    public CanvasGroup subscriptGroup {get {return _subscriptGroup;}}

    [SerializeField] private Image _page;
    public Image page {get {return _page;}}

    //LINES?

    [SerializeField] private CanvasGroup _noteCanvasGroup;
    public CanvasGroup noteCanvasGroup {get {return _noteCanvasGroup;}}

    [SerializeField] private CanvasGroup _listCanvasGroup;
    public CanvasGroup listConvasGroup {get {return _listCanvasGroup;}}

    [SerializeField] private CanvasGroup _readButton;
    public CanvasGroup readButton {get {return _readButton;}}

    [SerializeField] private CanvasGroup _nextButton;
    public CanvasGroup nextButton {get {return _nextButton;}}

    [SerializeField] private CanvasGroup _previousButton;
    public CanvasGroup previousButton {get {return _previousButton;}}

    [SerializeField] private S_NoteData _noteDataPrefab;
    public S_NoteData noteDataPrefab {get {return _noteDataPrefab;}}

    [SerializeField] private RectTransform _listRect;
    public RectTransform listRect {get {return _listRect;}}
}

public class S_NotesSystem : MonoBehaviour, SI_DataPersistance
{
    #region Data and Action

    [SerializeField] private UIElements UI = new UIElements();

    [SerializeField] private Color color1 = Color.gray;
    [SerializeField] private Color color2 = Color.gray;    

    private static Dictionary<string, S_Note> notes = new Dictionary<string, S_Note>();
    private List<S_NoteData> noteDatas = new List<S_NoteData>();
    private static Action<S_Note> A_display = delegate {};

    #endregion
    
    #region Audio
    //Array of audioSource
    //Fmod open SFX
    //Fmod close SFX
    //Turn pages SFXs
    #endregion

    #region Properties and Private

    private S_Note activeNote = null;
    private S_Page activePage
    {
        get
        {
            return activeNote.pages[currentPage];
        }
    }
    private int currentPage = 0;
    private bool readSubscript = false;
    private Sprite defaultPageTexture = null;
    private bool usingNotesSystem = false;

    #endregion

    #region Unity's Default methods

    void OnEnable()
    {
        A_display += DisplayNote;
    }

    void OnDisable()
    {
        A_display -= DisplayNote;
    }

    void Start()
    {
        Close(false);
        defaultPageTexture = UI.page.sprite;
    }

    void Update()
    {
        if (S_UserInput.instance.NotesMenuInput)
        {
            usingNotesSystem = !usingNotesSystem;

            if (usingNotesSystem)
            {
                Open();
            }
            else
            {
                Close(activeNote != null);
            }
        }
    }

    #endregion

     //!---------------- SI_DataPersistance ----------------

    //~ Sauvegarde notes obtenus

    public void LoadData(S_GameData gameData)
    {
        // Récupérer les notes stockés
        notes.Clear();

        foreach (KeyValuePair<string, S_Note> eachNote in gameData.notesObtained)
        {
            notes.Add(eachNote.Key, eachNote.Value);
        }
    }

    public void SaveData(S_GameData gameData)
    {
        // Sauvegarder les notes actuels
        gameData.notesObtained.Clear();

        foreach (KeyValuePair<string, S_Note> eachNote in notes)
        {
            gameData.notesObtained.Add(eachNote.Key, eachNote.Value);
        }
    }

    public int GetLoadPriority() => 0; // ✅ Priorité normale


    public void Open()
    {

        if (S_MenuManager.instance != null)
        {
            if (!S_MenuManager.instance.RegisterMenuOpen(S_MenuManager.MenuType.NOTES))
            {
                Debug.LogWarning("[NotesMenu] Impossible de démarrer le menu notes, un menu est ouvert");
                return;
            }
        }

        UpdateList();
        UpdateCanvasGroup(true, UI.listConvasGroup);
    }
    public void Close(bool playSFX)
    {
        CloseNote(playSFX);
        UpdateCanvasGroup(false, UI.listConvasGroup);
    }

    private void DisplayNote(S_Note note)
    {
        if (note == null) return;

        if (S_MenuManager.instance != null)
        {
            if (!S_MenuManager.instance.RegisterMenuOpen(S_MenuManager.MenuType.NOTES))
            {
                if (S_MenuManager.instance.GetCurrentOpenMenu() != S_MenuManager.MenuType.NOTES)
                {
                    Debug.LogWarning("[NotesMenu] Impossible de démarrer le menu notes, un menu est ouvert");
                    return;
                }
            }
        }

        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.noteOpen, S_FMODEvents.instance.target.position);

        UpdateCanvasGroup(true, UI.noteCanvasGroup);
        activeNote = note;

        DisplayPage(0);
    }
    private void DisplayPage(int page)
    {
        UI.readButton.interactable = activeNote.pages[page].pageType == PageType.TEXTURE;

        if (activeNote.pages[page].pageType != PageType.TEXT)
        {
            readSubscript = false;
        }
        else
        {
            if (readSubscript)
            {
                UpdateSubscript();
            }
        }



        switch (activeNote.pages[page].pageType)
        {
            case PageType.TEXT:
                UI.page.sprite = defaultPageTexture;
                UI.textObj.text = activeNote.pages[page].text;
                break;

            case PageType.TEXTURE:
                UI.page.sprite = activeNote.pages[page].texture;
                UI.textObj.text = string.Empty;
                break;
        }

        UpdateUI();
    }

    public static void Display(S_Note note)
    {
        A_display(note);
    }
    public static void Display(string key)
    {
        var note = GetNote(key);
        A_display(note);
    }

    public void CloseNote(bool playSFX)
    {
        if (playSFX)
        {
             S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.noteClose, S_FMODEvents.instance.target.position);
        }

        UpdateCanvasGroup(false, UI.noteCanvasGroup);
        OnNoteClose();
    }



    private void UpdateUI()
    {
        UI.previousButton.interactable = !(currentPage == 0);
        UI.nextButton.interactable = !(currentPage == activeNote.pages.Length - 1);

         // Le bouton Read est visible seulement pour les pages TEXT avec subscript
        var useSubscript = activePage.pageType == PageType.TEXT && activePage.useSubscript;
        UI.readButton.alpha = useSubscript ? (readSubscript ? 0.5f : 1f) : 0f;
        UI.readButton.interactable = useSubscript; 

        UpdateCanvasGroup(readSubscript, UI.subscriptGroup);
    }
    private void UpdateList()
    {
        ClearList();

        var index = 0;
        var height = 0.0f;
        foreach (var note in notes)
        {
            var color = index % 2 == 0 ? color1 : color2;

            var newNotePrefab = Instantiate(UI.noteDataPrefab, UI.listRect);
            noteDatas.Add(newNotePrefab);

            newNotePrefab.UpdateInfo(note.Value, color);
            
            newNotePrefab.rect.anchoredPosition = new Vector2(0, height);
            height -= newNotePrefab.rect.sizeDelta.y;

            UI.listRect.sizeDelta = new Vector2(UI.listRect.sizeDelta.x, height * -1);

            index++;
        }
    }
    private void UpdateSubscript()
    {
        UI.subscript.text = readSubscript ? activePage.text : string.Empty;
    }

    public void Next()
    {
         S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.noteTurnPage, S_FMODEvents.instance.target.position);

        currentPage++;
        DisplayPage(currentPage);
    }
    public void Previous()
    {
         S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.noteTurnPage, S_FMODEvents.instance.target.position);

        currentPage--;
        DisplayPage(currentPage);
    }
    public void Read()
    {
        readSubscript = !readSubscript;

        UpdateSubscript();
        UpdateUI();
    }

    private void ClearList()
    {
        foreach (var note in noteDatas)
        {
            Destroy(note.gameObject);
        }
        noteDatas.Clear();
    }
    private void OnNoteClose()
    {
        if (!usingNotesSystem)
        {
            if (S_MenuManager.instance != null)
            {
                S_MenuManager.instance.RegisterMenuClose(S_MenuManager.MenuType.NOTES);
            }
        }

        activeNote = null;
        currentPage = 0;
        readSubscript = false;
    }
    private void UpdateCanvasGroup(bool state, CanvasGroup canvasGroup)
    {
        if (state)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
        else
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
                
    }
  
    public static void AddNote(string key, S_Note note)
    {
        if (notes.ContainsKey(key) == false)
        {
            notes.Add(key, note);
        }
    }
    public static S_Note GetNote(string key)
    {
        if (notes.ContainsKey(key))
        {
            return notes[key];
        }

        return null;
    }

    //?-----------------------------------------------------------
}
