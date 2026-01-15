using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveSlotHover : MonoBehaviour, 
    IPointerEnterHandler,
    ISelectHandler
{
    [Header("Decorations")]
    [SerializeField] private UISelectorLogo leftDecoration;
    [SerializeField] private UISelectorLogo rightDecoration;

    [Header("Navigation (optionnel)")]
    [Tooltip("Si ce composant est sur un parent, référencez ici le Selectable enfant qui reçoit la navigation manette")]
    [SerializeField] private Selectable childSelectable;

    void OnEnable()
    {
        // S'abonner aux événements du Selectable enfant si spécifié
        if (childSelectable != null)
        {
            var childHandler = childSelectable.gameObject.GetComponent<ChildSelectableHandler>();
            if (childHandler == null)
            {
                childHandler = childSelectable.gameObject.AddComponent<ChildSelectableHandler>();
            }
            childHandler.Initialize(this);
        }
    }

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

    // Appelé par le ChildSelectableHandler
    public void OnChildSelected()
    {
        MenuSelectionManager.Instance.Select(leftDecoration, rightDecoration);
    }
}
