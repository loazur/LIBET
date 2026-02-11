using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class S_Item3DViewer : MonoBehaviour, IDragHandler, IPointerClickHandler
{
    public static S_Item3DViewer instance;

    [SerializeField] private GameObject uiContainer;
    [SerializeField] private RawImage rawImage; // La RawImage qui affiche la RenderTexture
    [SerializeField] private float rotationSensitivity = 0.5f;
    [SerializeField] private Camera item3DCamera;
    [SerializeField] private LayerMask interactableLayer;

    private Transform itemInstance;
    private bool isDragging = false;

    // Événement déclenché lors d'un clic sur le modèle 3D
    public event Action<RaycastHit> OnItem3DClicked;

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
                Debug.LogWarning("[Item3DViewer] Impossible de démarrer, un menu est ouvert");
                return;
            }
        }

        if (itemInstance != null) Destroy(itemInstance.gameObject);
        
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

        // Nettoyer les listeners
        OnItem3DClicked = null;

        uiContainer.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (itemInstance == null) return;

        isDragging = true;

        float rotationX = -eventData.delta.y * rotationSensitivity;
        float rotationY = eventData.delta.x * rotationSensitivity;

        itemInstance.Rotate(Vector3.up, -rotationY, Space.World);
        itemInstance.Rotate(Vector3.right, -rotationX, Space.Self);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;
            return;
        }

        if (itemInstance == null || item3DCamera == null || rawImage == null) return;

        RectTransform rectTransform = rawImage.rectTransform;
        Vector2 localPoint;
        
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out localPoint))
        {
            Rect rect = rectTransform.rect;
            float normalizedX = (localPoint.x - rect.x) / rect.width;
            float normalizedY = (localPoint.y - rect.y) / rect.height;

            Vector2 viewportPoint = new Vector2(normalizedX, normalizedY);

            Ray ray = item3DCamera.ViewportPointToRay(viewportPoint);
            
            RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
            
            foreach (RaycastHit hit in hits)
            {
                if (((1 << hit.collider.gameObject.layer) & interactableLayer) != 0)
                {
                    OnItem3DClicked?.Invoke(hit);
                    return;
                }
            }
        }
    }
}
