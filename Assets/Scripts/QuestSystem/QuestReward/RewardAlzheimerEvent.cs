


using UnityEngine;

class RewardAlzheimerEvent : ScriptableObject
{
    public override void GiveReward()
    {
        // Trigger the Alzheimer's event in the game
        AlzheimerEventManager.Instance.TriggerAlzheimerEvent();
    }
}