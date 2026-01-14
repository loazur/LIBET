using UnityEngine;

public class MenuSelectionManager : MonoBehaviour
{
    public static MenuSelectionManager Instance;

    private UISelectorLogo currentLeft;
    private UISelectorLogo currentRight;

    void Awake()
    {
        Instance = this;
    }

    void OnEnable()
    {
        // S'assurer qu'il n'y a qu'une seule instance active
        if (Instance != null && Instance != this)
        {
            Instance = this;
        }
    }

    public void Select(UISelectorLogo left, UISelectorLogo right)
    {
        // Toujours fermer les anciennes décos même si ce sont les mêmes (force refresh)
        if (currentLeft != null && currentLeft != left) 
        {
            currentLeft.ForceClose();
        }
        if (currentRight != null && currentRight != right) 
        {
            currentRight.ForceClose();
        }

        currentLeft = left;
        currentRight = right;

        if (currentLeft != null) currentLeft.Open();
        if (currentRight != null) currentRight.Open();
    }

    // Fermer toutes les décorations immédiatement
    public void CloseAll()
    {
        if (currentLeft != null) currentLeft.ForceClose();
        if (currentRight != null) currentRight.ForceClose();
        currentLeft = null;
        currentRight = null;
    }
}
