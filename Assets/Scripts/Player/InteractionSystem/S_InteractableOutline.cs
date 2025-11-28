using UnityEngine;

public class S_InteractableOutline : MonoBehaviour
{
    //~ Gère l'outline de CHAQUE intéraction
    private Outline outline;

    void Awake() //& Création de l'outline et ses propriétés
    {
        outline = GetComponent<Outline>();
        
        if (!outline)
            outline = gameObject.AddComponent<Outline>();

        outline.enabled = false;

        // Informations de l'outline
        outline.OutlineMode = Outline.Mode.OutlineAll; // -> à travers les murs
        outline.OutlineColor = new Color(1f, 1f, 1f, 0.9f); // Blanc avec un peu de glow
        outline.OutlineWidth = 5f;
    }

    public void Enable()  => outline.enabled = true; //& Pour l'activé l'outline
    public void Disable() => outline.enabled = false; //& Pour désactivé l'outline
}
