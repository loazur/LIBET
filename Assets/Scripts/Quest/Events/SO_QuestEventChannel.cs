using UnityEngine;

[CreateAssetMenu(fileName = "SO_QuestEventChannel", menuName = "Scriptable Objects/SO_QuestEventChannel")]
/**
 * canal d'événements pour les événements de quête
 *
 * @author	Unknown
 * @since	v0.0.1
 * @version	v1.0.0	Sunday, November 9th, 2025.
 * @global
 */
public class SO_QuestEventChannel : ScriptableObject
{
    public UnityEngine.Events.UnityEvent<Event> EventChannel;

    // type d'événement générique
    public enum Event
    {
        OnInteract,
        OnPlace,
        OnSit
        //! Ajouter d'autres types d'événements si nécessaire
    }
}
