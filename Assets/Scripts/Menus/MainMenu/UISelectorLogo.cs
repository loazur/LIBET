using UnityEngine;

[RequireComponent(typeof(Animator))]
public class UISelectorLogo : MonoBehaviour
{
    private Animator animator;
    private bool isOpen = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Open()
    {
        if (isOpen) return;   // ⛔ empêche la boucle

        isOpen = true;
        gameObject.SetActive(true);
        animator.SetBool("IsActive", true);
    }

    public void Close()
    {
        if (!isOpen) return;

        isOpen = false;
        animator.SetBool("IsActive", false);
    }

    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
