using UnityEngine;

[CreateAssetMenu(fileName = "SO_RewardDefinition", menuName = "Scriptable Objects/SO_RewardDefinition")]
/**
 * Définit une récompense de quête et stock les récompenses (peuvent être ajouter en modifiant le script)
 *
 * @author	Unknown
 * @since	v0.0.1
 * @version	v1.0.0	Sunday, November 9th, 2025.
 * @global
 */
public class SO_RewardDefinition : ScriptableObject
{
    public enum RewerdType //! Sert à rien pour le moment mais s'il faut un système de récompense, il faudra modifier ça
    {
        Item,
        XP,
        Currency,
        Custom
    }

    public RewerdType type;
    public int amount;
    public string referenceId; //! Attention au référencement des items ou autres sinon ça marche pas



}
