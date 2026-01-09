using UnityEngine;
using UnityEngine.EventSystems;

public class SaveSlotHover : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private UISelectorLogo left;
    [SerializeField] private UISelectorLogo right;
    [SerializeField] private MenuBackgroundShaderController shaderController;

    public void OnPointerEnter(PointerEventData eventData)
    {
        left.Open();
        right.Open();
        shaderController.OnHoverEnter();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        left.Close();
        right.Close();
        shaderController.OnHoverExit();
    }
}
