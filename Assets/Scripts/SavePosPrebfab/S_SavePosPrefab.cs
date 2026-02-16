



using System;
using System.Collections.Generic;
using UnityEngine;

public class S_SavePosPrefab : MonoBehaviour
{

    public static S_SavePosPrefab instance { get; private set; }

    #region  Attributes
    [Serializable]
    public struct SpawnEntry
    {
        public Transform spawnPoint;
        public GameObject prefab;
    }

    [SerializeField]
    private List<SpawnEntry> spawnEntries = new List<SpawnEntry>();

    private readonly Dictionary<Transform, GameObject> spawnMap = new Dictionary<Transform, GameObject>();

    #endregion

    // 
    private void Awake()
    {
        BuildSpawnMap();
    }

    public IReadOnlyDictionary<Transform, GameObject> GetSpawnMap()
    {
        return spawnMap;
    }

    /**
     * Fonction pour instancier tous les prefabs aux positions correspondantes
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, February 16th, 2026.
     * @access	public
     * @return	void
     */
    public void SpawnAll()
    {
        foreach (var pair in spawnMap)
        {
            if (pair.Key == null || pair.Value == null)
            {
                continue;
            }

            Instantiate(pair.Value, pair.Key.position, pair.Key.rotation);
        }
    }

    /**
     * Fonction pour construire la map de spawn à partir de la liste d'entrées
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, February 16th, 2026.
     * @access	private
     * @return	void
     */
    private void BuildSpawnMap()
    {
        spawnMap.Clear();

        foreach (var entry in spawnEntries)
        {
            if (entry.spawnPoint == null || entry.prefab == null)
            {
                continue;
            }

            if (!spawnMap.ContainsKey(entry.spawnPoint))
            {
                spawnMap.Add(entry.spawnPoint, entry.prefab);
            }
        }
    }
}