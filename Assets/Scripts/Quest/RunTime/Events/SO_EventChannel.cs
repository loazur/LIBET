using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "SO_EventChannel", menuName = "Scriptable Objects/SO_EventChannel")]
public class SO_EventChannel<T> : ScriptableObject
{
    private event UnityAction<T> OnEventRaised;

    public void Raise(T param)
    {
        OnEventRaised?.Invoke(param);
    }

    public void RegisterListener(UnityAction<T> listener)
    {
        OnEventRaised += listener;
    }

    public void UnregisterListener(UnityAction<T> listener)
    {
        OnEventRaised -= listener;
    }
}
