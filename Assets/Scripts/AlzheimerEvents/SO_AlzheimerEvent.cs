using UnityEngine;

[CreateAssetMenu(menuName = "Alzheimer/Event")]
public class SO_AlzheimerEvent : ScriptableObject
{
    //~ Informations d'un event
    [Header("Information de l'event")]
    [Tooltip("Nom de l'event unique")]
    public string eventName;

    [Tooltip("Durée de l'event en secondes (0 = inactif)"), Min(0)]
    public float eventDuration = 0;

    [Tooltip("Intensité de l'event par rapport aux autres"), Range(0, 10)]
    public float eventIntensity;

    [Tooltip("Comment l'event s'active")]
    public ActivationType eventActivationType; 

    [Tooltip("Poids que l'event s'active"), Min(0)]
    public float eventBaseWeight;

    [Tooltip("Si l'event s'active une seule fois ou non")]
    public bool eventIsOneShot;

    public enum ActivationType
    {
        Randomly, //-> l'event ce lance aléatoirement
        OnWakeUp, //-> l'event ce lance après que Libet se lève
        Conditional //-> l'event ce lance après que une condition est atteinte
    }

    [Header("Prefab avec lequel l'event est lié")]
    [Tooltip("Contient la logique de l'event")]
    public GameObject eventPrefab;

    [HideInInspector] public bool eventHasTriggered = false; //-> si l'event s'est déja lancé

    public void Trigger() //& Fonction qui active l'event
    {
        GameObject instance = Instantiate(eventPrefab); //TODO gérer le parent pour que sa soit mieux ranger lors de l'instantiation

        if (eventDuration != 0) // Destruction du prefab après une certaine durée
            Destroy(instance, eventDuration);
    }
}
