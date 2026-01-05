using UnityEngine;
using UnityEngine.UI;

public class S_Menu : MonoBehaviour
{
    [Header("First selected button")]
    [SerializeField] private Selectable firstSelected;

    protected virtual void OnEnable()
    {
        SetFirstSelected(firstSelected);
    }

    public void SetFirstSelected(Selectable firstSelectedObject)
    {
        firstSelectedObject.Select();
    }

    public void ActivateMenu()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }
}
