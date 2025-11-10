using UnityEngine;
using System.Collections.Generic;



/**
 * Scripte qui contient toutes les définitions de quêtes
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Monday, November 10th, 2025.
 * @global
 */
public class S_QuestDatabase : MonoBehaviour
{
    [SerializeField] private List<SO_QuestDefinition> questDefinitions;

    public SO_QuestDefinition GetQuestById(string questId)
    {
        return questDefinitions.Find(q => q.questId == questId);
    }
}
