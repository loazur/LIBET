using UnityEngine;
using System;

public class S_PlaceObjectZone : MonoBehaviour
{
    public event Action<GameObject> onObjectPlaced;

    private void OnTriggerEnter(Collider other)
    {
        onObjectPlaced?.Invoke(other.gameObject);
    }
}
