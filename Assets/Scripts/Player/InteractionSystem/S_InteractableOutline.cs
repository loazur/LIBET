using UnityEngine;

public class S_InteractableOutline : MonoBehaviour
{
    //~ Gère l'outline de CHAQUE intéraction
    private Outline outline;

    void Awake() //& Création de l'outline et propriétés
    {
        outline = GetComponent<Outline>();
        
        if (!outline)
            outline = gameObject.AddComponent<Outline>();

        outline.enabled = false;
        outline.OutlineMode = Outline.Mode.OutlineAll;
        outline.OutlineColor = Color.white;
        outline.OutlineWidth = 6f;
    }

    public void Enable()  => outline.enabled = true;
    public void Disable() => outline.enabled = false;
}
