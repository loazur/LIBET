using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Génère des textures 3D pour le Local Volumetric Fog
/// </summary>
public class VolumetricFogTextureGenerator : MonoBehaviour
{
#if UNITY_EDITOR
    [Header("Paramètres de génération")]
    [SerializeField] private int resolution = 32;
    [SerializeField] private float noiseScale = 4f;
    [SerializeField] private string textureName = "VolumetricFog3DTexture";
    
    [Header("Type de texture")]
    [SerializeField] private TextureType textureType = TextureType.PerlinNoise;
    
    public enum TextureType
    {
        PerlinNoise,      // Bruit naturel
        Gradient,         // Dégradé vertical
        Spherical,        // Forme sphérique
        Cylindrical       // Forme cylindrique (bon pour rayons de lumière)
    }
    
    [ContextMenu("Générer Texture 3D")]
    public void GenerateTexture()
    {
        Texture3D texture = new Texture3D(resolution, resolution, resolution, TextureFormat.RGBA32, false);
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Bilinear;
        
        Color[] colors = new Color[resolution * resolution * resolution];
        
        for (int z = 0; z < resolution; z++)
        {
            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float density = CalculateDensity(x, y, z);
                    int index = x + y * resolution + z * resolution * resolution;
                    colors[index] = new Color(density, density, density, density);
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        
        // Sauvegarder l'asset
        string path = $"Assets/Settings/HDRP/{textureName}.asset";
        AssetDatabase.CreateAsset(texture, path);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"Texture 3D générée : {path}");
    }
    
    private float CalculateDensity(int x, int y, int z)
    {
        float nx = (float)x / resolution;
        float ny = (float)y / resolution;
        float nz = (float)z / resolution;
        
        switch (textureType)
        {
            case TextureType.PerlinNoise:
                return Mathf.PerlinNoise(nx * noiseScale, ny * noiseScale) * 
                       Mathf.PerlinNoise(ny * noiseScale, nz * noiseScale);
                       
            case TextureType.Gradient:
                // Dégradé du bas vers le haut
                return 1f - ny;
                
            case TextureType.Spherical:
                // Sphère au centre
                float cx = nx - 0.5f;
                float cy = ny - 0.5f;
                float cz = nz - 0.5f;
                float dist = Mathf.Sqrt(cx * cx + cy * cy + cz * cz);
                return Mathf.Clamp01(1f - dist * 2f);
                
            case TextureType.Cylindrical:
                // Cylindre vertical (parfait pour les rayons de lumière)
                float dx = nx - 0.5f;
                float dz = nz - 0.5f;
                float radialDist = Mathf.Sqrt(dx * dx + dz * dz);
                return Mathf.Clamp01(1f - radialDist * 2f);
                
            default:
                return 1f;
        }
    }
#endif
}
