using UnityEngine;
using System;
using System.Collections.Generic;

public class S_PlaceObjectZone : MonoBehaviour
{
    [Header("Zone Identification")]
    [Tooltip("ID unique pour identifier cette zone (ex: 'place_zone_balls_01')")]
    [SerializeField] private string zoneId = "";

    public string ZoneId => zoneId;

    public event Action<GameObject> onObjectPlaced;

    // Registre statique de toutes les zones
    private static Dictionary<string, S_PlaceObjectZone> zoneRegistry = new();

    public static S_PlaceObjectZone GetZoneById(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        zoneRegistry.TryGetValue(id, out var zone);
        return zone;
    }

    private void Awake()
    {
        if (string.IsNullOrEmpty(zoneId))
        {
            Debug.LogWarning($"[S_PlaceObjectZone] '{gameObject.name}' has no Zone ID assigned!");
            return;
        }

        if (zoneRegistry.ContainsKey(zoneId))
        {
            Debug.LogWarning($"[S_PlaceObjectZone] Duplicate Zone ID '{zoneId}' on '{gameObject.name}'");
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
        onObjectPlaced?.Invoke(other.gameObject);
    }
}
