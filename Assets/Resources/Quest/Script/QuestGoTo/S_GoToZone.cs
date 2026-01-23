using UnityEngine;
using System;
using System.Collections.Generic;

public class S_GoToZone : MonoBehaviour
{
    [Header("Zone Identification")]
    [Tooltip("ID unique pour identifier cette zone (ex: 'zone_forest_01')")]
    [SerializeField] private string zoneId = "";

    public string ZoneId => zoneId;

    public event Action<GameObject> onEntityEntered;

    // Registre statique de toutes les zones
    private static Dictionary<string, S_GoToZone> zoneRegistry = new();

    public static S_GoToZone GetZoneById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        zoneRegistry.TryGetValue(id, out var zone);
        return zone;
    }

    private void Awake()
    {
        if (string.IsNullOrEmpty(zoneId))
        {
            Debug.LogWarning($"[S_GoToZone] '{gameObject.name}' has no Zone ID assigned!");
            return;
        }

        if (zoneRegistry.ContainsKey(zoneId))
        {
            Debug.LogWarning($"[S_GoToZone] Duplicate Zone ID '{zoneId}' on '{gameObject.name}'");
            return;
        }

        zoneRegistry[zoneId] = this;
    }

    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(zoneId) && zoneRegistry.ContainsKey(zoneId))
            zoneRegistry.Remove(zoneId);
    }

    private void OnTriggerEnter(Collider other)
    {
        onEntityEntered?.Invoke(other.gameObject);
    }
}
