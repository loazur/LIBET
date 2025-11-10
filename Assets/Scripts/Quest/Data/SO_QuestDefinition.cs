using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SO_QuestDefinition", menuName = "Scriptable Objects/SO_QuestDefinition")]

/**
 * Définit la structure de base d'une quête
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Monday, November 10th, 2025.
 * @global
 */
public class SO_QuestDefinition : ScriptableObject
{


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
