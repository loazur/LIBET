using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UISelectorLogo : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;

    void Awake()
    {
        animator = GetComponent<Animator>();

        Debug.Log($"[UISelectorLogo] Awake on {name} | Animator = {animator}");
    }

    void OnEnable()
    {
        Debug.Log($"[UISelectorLogo] OnEnable | isOpen={isOpen}");
    }

    public void Open()
    {
        Debug.Log($"[UISelectorLogo] Open() called | isOpen={isOpen}");

        if (isOpen)
        {
            Debug.Log("[UISelectorLogo] Open aborted (already open)");
            return;
        }

        isOpen = true;
        animator.Play("Open", 0, 0f);
        animator.SetBool("IsActive", true);

        Debug.Log("[UISelectorLogo] Open animation triggered");
    }

    public void Close()
    {
        Debug.Log($"[UISelectorLogo] Close() called | isOpen={isOpen}");

        if (!isOpen)
        {
            Debug.Log("[UISelectorLogo] Close aborted (already closed)");
            return;
        }

        isOpen = false;
        animator.SetBool("IsActive", false);

        Debug.Log("[UISelectorLogo] Close animation triggered");
    }

    // Animation Event à la FIN du clip Close
    public void DisableSelf()
    {
        Debug.Log("[UISelectorLogo] DisableSelf() called");
        gameObject.SetActive(false);
    }
}
