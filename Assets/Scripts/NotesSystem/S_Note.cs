using UnityEngine;

[CreateAssetMenu(fileName = "NewNote", menuName = "NotesSystem/NewNote")]
public class S_Note : ScriptableObject
{
    [SerializeField] private string _labelFrench = string.Empty;
    public string labelFrench {get {return _labelFrench;}}
    [SerializeField] private string _labelEnglish = string.Empty;
    public string labelEnglish {get {return _labelEnglish;}}

    [SerializeField] private S_Page[] _pagesFrench = new S_Page[0];
    public S_Page[] pagesFrench {get {return _pagesFrench;}}

    [SerializeField] private S_Page[] _pagesEnglish = new S_Page[0];
    public S_Page[] pagesEnglish {get {return _pagesEnglish;}}
}
