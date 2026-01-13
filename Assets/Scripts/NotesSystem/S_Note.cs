using UnityEngine;

[CreateAssetMenu(fileName = "NewNote", menuName = "NotesSystem/NewNote")]
public class S_Note : ScriptableObject
{
    [SerializeField] private string _label = string.Empty;
    public string label {get {return _label;}}

    [SerializeField] private S_Page[] _pages = new S_Page[0];
    public S_Page[] pages {get {return _pages;}}
}
