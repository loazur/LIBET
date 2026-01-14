using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Composant ajouté dynamiquement aux Selectables enfants pour relayer les événements de sélection
/// au parent SaveSlotHover. Cela permet aux boutons de rebind de déclencher les animations de déco
/// même quand le SaveSlotHover est sur un parent.
/// Les décos ne se ferment que quand un AUTRE bouton est survolé (géré par MenuSelectionManager).
/// </summary>
public class ChildSelectableHandler : MonoBehaviour, ISelectHandler, IPointerEnterHandler
{
    private SaveSlotHover parentHover;

    public void Initialize(SaveSlotHover parent)
    {
        parentHover = parent;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (parentHover != null)
        {
            parentHover.OnChildSelected();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (parentHover != null)
        {
            parentHover.OnChildSelected();
        }
    }
}
