using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

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
        if (firstSelectedObject == null) return;
        
        // Sélectionner le bouton
        firstSelectedObject.Select();
        
        // Activer les décorations du bouton sélectionné
        StartCoroutine(ActivateDecorationsDelayed(firstSelectedObject.gameObject));
    }

    private IEnumerator ActivateDecorationsDelayed(GameObject selectedObject)
    {
        // Attendre une frame pour que tout soit initialisé
        yield return null;
        
        // Chercher le SaveSlotHover sur l'objet sélectionné ou ses parents
        var hoverHandler = selectedObject.GetComponent<SaveSlotHover>();
        if (hoverHandler == null)
        {
            hoverHandler = selectedObject.GetComponentInParent<SaveSlotHover>();
        }
        
        if (hoverHandler != null)
        {
            // Déclencher manuellement l'événement OnSelect
            hoverHandler.OnSelect(new BaseEventData(EventSystem.current));
        }
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
