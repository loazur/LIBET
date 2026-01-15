using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonSelector : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    ISelectHandler,
    IDeselectHandler
{
    [Header("Decorations")]
    public GameObject leftDeco;
    public GameObject rightDeco;

    void Awake()
    {
        SetDecorations(false);
    }

    void SetDecorations(bool state)
    {
        if (leftDeco) leftDeco.SetActive(state);
        if (rightDeco) rightDeco.SetActive(state);
    }

    // Souris
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
        SetDecorations(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetDecorations(false);
    }

    // Clavier / Manette
    public void OnSelect(BaseEventData eventData)
    {
        SetDecorations(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetDecorations(false);
    }
}
