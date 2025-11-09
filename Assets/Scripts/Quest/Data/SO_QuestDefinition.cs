/**
 * @brief décrit une quête complète
 *
 * @file SO_QuestDefinition.cs
 *
 */


using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_QuestDefinition", menuName = "Scriptable Objects/SO_QuestDefinition")]
public class SO_QuestDefinition : ScriptableObject
{
    public string questId;
    public string title;
    [TextArea] public string description;

    [Header("Liste des objectifs")]
    public SO_ObjectiveDefinition[] objectives;

    [Header("Récompenses")]
    public int goldReward;
    public string itemReward;
}
