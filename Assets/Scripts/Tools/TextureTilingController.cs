using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TextureTilingController : MonoBehaviour
{
    public Vector2 tiling = Vector2.one;
    public Vector2 offset = Vector2.zero;

    [Tooltip("Nom de la propriété texture (_BaseMap URP, _MainTex Built-in)")]
    public string textureProperty = "_MainTex";

    Renderer rend;
    Material matInstance;

    void Awake()
    {
        rend = GetComponent<Renderer>();

        // Crée une instance locale du matériau
        matInstance = rend.material;

        Apply();
    }

    void OnValidate()
    {
        if (rend == null)
            rend = GetComponent<Renderer>();

        if (rend != null)
        {
            matInstance = rend.material;
            Apply();
        }
    }

    void Apply()
    {
        if (matInstance.HasProperty(textureProperty))
        {
            matInstance.SetTextureScale(textureProperty, tiling);
            matInstance.SetTextureOffset(textureProperty, offset);
        }
        else
        {
            Debug.LogWarning(
                $"Le matériau {matInstance.name} n'a pas la propriété {textureProperty}",
                this
            );
        }
    }
}
