using UnityEngine;
using UnityEngine.EventSystems;

public class S_Item3DViewer : MonoBehaviour, IDragHandler
{
    public static S_Item3DViewer instance;

    [SerializeField] private GameObject uiContainer;
    [SerializeField] private float rotationSensitivity = 0.5f; // Ajustez cette valeur pour contrôler la sensibilité

    private Transform itemInstance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        if (S_UserInput.instance.CancelInteractionAction.WasPressedThisFrame() && itemInstance != null)
        {
            TriggerEndExamine();
        }
    }

    public void TriggerExamine(Transform item)
    {
        if (S_MenuManager.instance != null)
        {
            if (!S_MenuManager.instance.RegisterMenuOpen(S_MenuManager.MenuType.MINIGAME))
            {
                Debug.LogWarning("[ArrowMinigame] Impossible de démarrer le menu ArrowMinigame, un menu est ouvert");
                return;
            }
        }

        if (itemInstance != null) Destroy(itemInstance.gameObject); // Mesh déjà existant
        
        itemInstance = Instantiate(item, new Vector3(1000, 1000, 1000), Quaternion.identity);

        uiContainer.SetActive(true);
    }

    public void TriggerEndExamine()
    {
        if (S_MenuManager.instance != null) 
        {
            S_MenuManager.instance.RegisterMenuClose(S_MenuManager.MenuType.MINIGAME);
        }

        if (itemInstance) Destroy(itemInstance.gameObject);

        uiContainer.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemInstance == null) return;

        // Rotation autour de l'axe Y (horizontal) et X (vertical)
        float rotationX = -eventData.delta.y * rotationSensitivity;
        float rotationY = eventData.delta.x * rotationSensitivity;

        // Appliquer la rotation de manière relative
        itemInstance.Rotate(Vector3.up, -rotationY, Space.World);
        itemInstance.Rotate(Vector3.right, -rotationX, Space.Self);
    }
}
