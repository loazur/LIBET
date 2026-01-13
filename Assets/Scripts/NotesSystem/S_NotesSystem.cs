using System;
using System.Collections.Generic;
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

public class S_NotesSystem : MonoBehaviour
{
    #region Data and Action

    [SerializeField] private UIElements UI = new UIElements();

    [SerializeField] private Color color1 = Color.gray;
    [SerializeField] private Color color2 = Color.gray;    

    private static Dictionary<string, S_Note> notes = new Dictionary<string, S_Note>();
    private List<S_NoteData> noteDatas = new List<S_NoteData>();
    private static Action<S_Note> A_display = delegate {};

    //~ Références
    [SerializeField] private S_PlayerController playerController;
    [SerializeField] private S_FirstPersonCamera playerCamera;

    private S_PlayerCrouch playerCrouch;
    private S_PlayerInteract playerInteract;
    private S_PlayerFootsteps playerFootsteps;

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
        
    }

    void OnDisable()
    {
        
    }

    void Awake()
    {
        playerCrouch = playerController.GetComponent<S_PlayerCrouch>();
        playerInteract = playerController.GetComponent<S_PlayerInteract>();
        playerFootsteps = playerController.GetComponent<S_PlayerFootsteps>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    #endregion


    public void Open()
    {
        DisableMouvements(); // Désactive les scripts

        UpdateList();
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

    private void ClearList()
    {
        foreach (var note in noteDatas)
        {
            Destroy(note.gameObject);
        }
        noteDatas.Clear();
    }

    //?-----------------------------------------------------------

    private void EnableMovements()
    {
        playerController.setMovementsEnabled(true);
        playerCamera.setCursorEnabled(false);
        playerCamera.setRotationEnabled(true);
        playerInteract.setInteractionEnabled(true);
        playerCrouch.setAbleToCrouch(true);
        playerFootsteps.SetSoundsEnabled(true);
    }

    private void DisableMouvements()
    {
        playerController.setMovementsEnabled(false);
        playerCamera.setCursorEnabled(true);
        playerCamera.setRotationEnabled(false);
        playerInteract.setInteractionEnabled(false);
        playerCrouch.setAbleToCrouch(false);
        playerFootsteps.SetSoundsEnabled(false);
    }
}
