using UnityEngine;

[CreateAssetMenu(fileName = "SO_ObjectiveDefinition", menuName = "Scriptable Objects/SO_ObjectiveDefinition")]

/**
 * définit un type d’objectif possible.
 *
 * @global
 */
public class SO_ObjectiveDefinition : ScriptableObject
{
    [Header("Identification")]
    public string objectiveId;
    public string description;

    [Header("Type d'objectif")]
    public SO_ObjectiveType_GoTo objectiveType;  // référence vers le type générique (Interact, GoTo, etc.)

    [Header("Paramètres spécifiques")]
    public string targetId;      // ex : ID de l’objet ou du NPC à atteindre
    public Vector3 targetPosition;
    public float radius = 1f;

    [Header("Suivi de progression")]
    public int requiredAmount = 1;
}
