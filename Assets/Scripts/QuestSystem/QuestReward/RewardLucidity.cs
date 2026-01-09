using UnityEngine;

/**
 * Récompense de quête qui augmente la jauge de lucidité
 * 
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Wednesday, January 8th, 2026.
 */
[CreateAssetMenu(fileName = "New Lucidity Reward", menuName = "Quest System/Rewards/Lucidity Reward", order = 1)]
public class RewardLucidity : QuestReward
{
    [Header("Lucidity Settings")]
    [Tooltip("Valeur d'augmentation de la jauge de lucidité (0-100)")]
    [SerializeField, Range(-100, 100)] 
    private float lucidityAmount = 10f;

    /**
     * Applique la récompense en augmentant la jauge de lucidité
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 8th, 2026.
     * @access	public
     * @return	void
     */
    public override void GiveReward()
    {
        Debug.Log($"<color=green>[RewardLucidity]</color> GiveReward() appelé - Tentative d'augmentation de {lucidityAmount}%");
        
        if (S_AlzheimerEventsManager.instance != null)
        {
            float lucidityBefore = S_AlzheimerEventsManager.instance.Lucidity;
            S_AlzheimerEventsManager.instance.RecoverLucidity(lucidityAmount);
            float lucidityAfter = S_AlzheimerEventsManager.instance.Lucidity;
            
            Debug.Log($"<color=green>[RewardLucidity]</color> Lucidité: {lucidityBefore:F1}% → {lucidityAfter:F1}% (Δ {lucidityAfter - lucidityBefore:F1}%)");
        }
        else
        {
            Debug.LogError("<color=red>[RewardLucidity]</color> S_AlzheimerEventsManager.instance est null ! Assurez-vous qu'il est présent dans la scène.");
        }
    }

    /**
     * Accesseur pour la valeur de lucidité (pour l'UI)
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 8th, 2026.
     * @access	public
     * @return	float
     */
    public float LucidityAmount => lucidityAmount;
}
