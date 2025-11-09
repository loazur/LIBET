using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SO_QuestStage", menuName = "Scriptable Objects/SO_QuestStage")]
/**
 * Définit une étape d'une quête
 *
 * @author	Unknown
 * @since	v0.0.1
 * @version	v1.0.0	Sunday, November 9th, 2025.
 * @global
 */
public class SO_QuestStage : ScriptableObject
{
    [Header("Informations de l'étape")]
    public string stageId;
    public bool autoAdvance;

    [Header("Objectifs de l'étape")]
    public List<SO_ObjectiveBase> objectives;
}
