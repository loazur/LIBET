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

    public void Select(UISelectorLogo left, UISelectorLogo right)
    {
        if (currentLeft == left && currentRight == right)
            return;

        if (currentLeft != null) currentLeft.Close();
        if (currentRight != null) currentRight.Close();

        currentLeft = left;
        currentRight = right;

        if (currentLeft != null) currentLeft.Open();
        if (currentRight != null) currentRight.Open();
    }
}
