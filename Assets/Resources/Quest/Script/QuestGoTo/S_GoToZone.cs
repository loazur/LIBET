using UnityEngine;
using System;

public class S_GoToZone : MonoBehaviour
{
    public event Action<GameObject> onEntityEntered;

    private void OnTriggerEnter(Collider other)
    {
        onEntityEntered?.Invoke(other.gameObject);
    }
}
