//! Singleton chargé de fournir les définitions de quêtes (S_QuestDefinition ScriptableObjects). 
//! Doit permettre GetDefinition(string id) et charger les assets (Resources ou une liste exposée dans l’inspector).

using System.Collections.Generic;
using UnityEngine;

/**
 * Base simple pour charger / retrouver les QuestDefinition (ScriptableObjects).
 * Méthode de chargement : soit via une liste exposée en inspector, soit via Resources.LoadAll.
 *
 * @author	Lucas
 * @since	v0.0.1
 * @version	v1.0.0	Monday, November 10th, 2025.
 * @global
 */
public class S_QuestDatabase : MonoBehaviour
{
    public static S_QuestDatabase Instance { get; private set; }

    [Header("Option 1: fill manually in inspector")]
    public List<S_QuestDefinition> questDefinitions = new List<S_QuestDefinition>();

    [Header("Option 2: or load from Resources/Quests (if non-empty)")]
    public bool useResourcesLoad = true;
    private Dictionary<string, S_QuestDefinition> _index = new Dictionary<string, S_QuestDefinition>();

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        BuildIndex();
    }

    /**
     * Construit l'index des définitions de quêtes
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @access	private
     * @return	void
     */
    private void BuildIndex()
    {
        _index.Clear();

        if (useResourcesLoad)
        {
            var arr = Resources.LoadAll<S_QuestDefinition>("Quests");
            foreach (var d in arr)
            {
                if (d == null) continue;
                if (string.IsNullOrEmpty(d.questId))
                {
                    Debug.LogWarning("[QuestDatabase] QuestDefinition sans questId: " + d.name);
                    continue;
                }
                _index[d.questId] = d;
            }
        }

        // Also index inspector-filled definitions (overrides Resources if same id)
        foreach (var d in questDefinitions)
        {
            if (d == null) continue;
            if (string.IsNullOrEmpty(d.questId))
            {
                Debug.LogWarning("[QuestDatabase] (Inspector) QuestDefinition sans questId: " + d.name);
                continue;
            }
            _index[d.questId] = d;
        }
    }

    /**
     * Retourne la définition de quête correspondant à l'id ou null.
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @access	public
     * @param	string	questId	
     * @return	mixed
     */
    public S_QuestDefinition GetDefinition(string questId)
    {
        if (string.IsNullOrEmpty(questId)) return null;
        _index.TryGetValue(questId, out var def);
        return def;
    }
}
