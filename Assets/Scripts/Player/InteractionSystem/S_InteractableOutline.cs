using UnityEngine;

public class S_InteractableOutline : MonoBehaviour
{
    //~ Gère l'outline de CHAQUE intéraction
    private Outline outline;

    void Awake() //& Création de l'outline et ses propriétés
    {

        // Vérifie que l'objet a un mesh exploitable
        if (!HasSafeMesh())
        {
            Debug.LogWarning($"[Outline] {name} : Mesh invalide → Outline annulé");
            enabled = false;
            return;
        }

        outline = gameObject.AddComponent<Outline>();

        // Informations de l'outline
        outline.enabled = false;
        outline.OutlineMode = Outline.Mode.OutlineAll; // -> à travers les murs
        outline.OutlineColor = new Color(1f, 1f, 1f, 0.9f); // Blanc avec un peu de glow
        outline.OutlineWidth = 5f;
    }

    public void Enable()//& Pour l'activé l'outline
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void Disable()//& Pour désactivé l'outline
    {
        if (outline != null)
            outline.enabled = false;
    }

    bool HasSafeMesh()
    {
        // MeshFilter
        var mf = GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
            return true;

        // SkinnedMesh
        var sk = GetComponent<SkinnedMeshRenderer>();
        if (sk != null && sk.sharedMesh != null)
            return true;

        // Enfants
        foreach (var child in GetComponentsInChildren<MeshFilter>())
            if (child.sharedMesh != null)
                return true;

        foreach (var child in GetComponentsInChildren<SkinnedMeshRenderer>())
            if (child.sharedMesh != null)
                return true;

        return false;
    }


}
