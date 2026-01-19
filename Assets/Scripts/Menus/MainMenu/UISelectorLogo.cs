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

        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_change_selection, S_FMODEvents.instance.target.position);

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

    // Fermeture forcée et immédiate (pour éviter les chevauchements)
    public void ForceClose()
    {
        Debug.Log($"[UISelectorLogo] ForceClose → {name}");
        isOpen = false;
        if (animator != null)
        {
            animator.SetBool("IsActive", false);
        }
        // Désactiver immédiatement pour éviter les problèmes de chevauchement
        gameObject.SetActive(false);
    }

    // Appelé par Animation Event à la fin de Close
    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
