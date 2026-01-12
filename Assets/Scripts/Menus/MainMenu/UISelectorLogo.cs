using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UISelectorLogo : MonoBehaviour
{
    private Animator animator;
    private bool isOpen;

    void Awake()
    {
        animator = GetComponent<Animator>();
        gameObject.SetActive(false);
    }

    public void Open()
    {
        if (isOpen) return;

        Debug.Log($"[UISelectorLogo] Open → {name}");
        isOpen = true;
        gameObject.SetActive(true);
        animator.SetBool("IsActive", true);
    }

    public void Close()
    {
        if (!isOpen) return;

        Debug.Log($"[UISelectorLogo] Close → {name}");
        isOpen = false;
        animator.SetBool("IsActive", false);
    }

    // Appelé par Animation Event à la fin de Close
    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
