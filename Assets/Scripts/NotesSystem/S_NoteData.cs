using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_NoteData : MonoBehaviour
{
    [SerializeField] private Image bgImage = null;
    [SerializeField] private TextMeshProUGUI label = null;

    private S_Note note = null;
    private RectTransform _rect = null;
    public RectTransform rect
    {
        get
        {
            if (_rect == null)
            {
                _rect = GetComponent<RectTransform>();
                if (_rect == null)
                {
                    _rect = gameObject.AddComponent<RectTransform>();
                }
            }
            return _rect;
        }
    }

    public void UpdateInfo(S_Note note, Color color)
    {
        this.note = note;

        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            label.text = note.labelFrench;
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            label.text = note.labelEnglish;
        }

        bgImage.color = color;
    }
    public void Display()
    {
        S_NotesSystem.Display(note);
    }
}
