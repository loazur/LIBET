


using UnityEngine;

/**
 * Récompense de quête qui déclenche un événement Alzheimer
 * 
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Wednesday, January 8th, 2026.
 */
[CreateAssetMenu(fileName = "New Alzheimer Event Reward", menuName = "Quest System/Rewards/Alzheimer Event Reward", order = 2)]
public class RewardAlzheimerEvent : QuestReward
{
    /**
     * Déclenche un événement Alzheimer aléatoire
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 8th, 2026.
     * @access	public
     * @return	void
     */
    public override void GiveReward()
    {
        if (S_AlzheimerEventsManager.instance != null)
        {
            S_AlzheimerEventsManager.instance.TryTriggerRandomEvent();
            Debug.Log("<color=yellow>[RewardAlzheimerEvent]</color> Événement Alzheimer déclenché");
        }
        else
        {
            Debug.LogWarning("[RewardAlzheimerEvent] S_AlzheimerEventsManager.instance est null !");
        }
    }
}