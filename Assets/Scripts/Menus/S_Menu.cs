using UnityEngine;
using UnityEngine.UI;

public class S_Menu : MonoBehaviour
{
    [Header("First selected button")]
    [SerializeField] private Button firstSelected;

    protected virtual void OnEnable()
    {
        SetFirstSelected(firstSelected);
    }

    public void SetFirstSelected(Button firstSelectedObject)
    {
        firstSelectedObject.Select();
    }
}
