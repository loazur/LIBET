using UnityEngine;

public enum PageType
{
    TEXT,
    TEXTURE
}

[CreateAssetMenu(fileName = "NewPage", menuName = "NotesSystem/NewPage")]
public class S_Page : ScriptableObject
{
    [SerializeField] private PageType _pageType = PageType.TEXT;
    public PageType pageType {get {return _pageType;}}

    [TextArea(8, 16)]
    [SerializeField] private string _text = string.Empty;
    public string text {get {return _text;}}

    [SerializeField] private Sprite _texture = null;
    public Sprite texture {get { return _texture;}}

    [SerializeField] private bool _useSubscript = true;
    public bool useSubscript {get {return _useSubscript;}}
}
