using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlotHover : MonoBehaviour, IPointerEnterHandler
{
    [Header("Decorations")]
    [SerializeField] private UISelectorLogo leftDecoration;
    [SerializeField] private UISelectorLogo rightDecoration;

    public void OnPointerEnter(PointerEventData eventData)
    {
        MenuSelectionManager.Instance.Select(leftDecoration, rightDecoration);
    }
}
