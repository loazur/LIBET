using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlotHover : MonoBehaviour, 
    IPointerEnterHandler,
    ISelectHandler,
    IDeselectHandler
{
    [Header("Decorations")]
    [SerializeField] private UISelectorLogo leftDecoration;
    [SerializeField] private UISelectorLogo rightDecoration;

    // Souris
    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuSelectionManager.Instance.Select(leftDecoration, rightDecoration);
    }

    // Manette / Clavier
    public void OnSelect(BaseEventData eventData)
    {
        MenuSelectionManager.Instance.Select(leftDecoration, rightDecoration);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        MenuSelectionManager.Instance.Select(null, null);
    }
}
