using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SO_QuestDefinition", menuName = "Scriptable Objects/SO_QuestDefinition")]
public class SO_QuestDefinition : ScriptableObject
{
    //string id (unique, stable)
    // string title, string description
    // List<QuestStage> stages (chaque stage a List<ObjectiveBase> objectives)
    // List<string> prerequisites (ids de quêtes à terminer)
    // bool isRepeatable, int repeatCooldown, bool isUnique (uniques = une seule fois dans la vie du profil)
    // List<RewardDefinition> rewards
    // List<string> tags (e.g. tutorial, main, side, daily)

    [Header("Informations de la quête")]
    public string questId;
    public string title;
    public string description;

    [Header("Étapes de la quête")]
    public List<SO_QuestStage> stages;
    public List<string> prerequisites;

    [Header("Propriétés de la quête")]
    public bool isRepeatable;
    public int repeatCooldown; // en secondes
    public bool isUnique;

    [Header("Récompenses de la quête")]
    public List<SO_RewardDefinition> rewards;
    public List<string> tags;
}
