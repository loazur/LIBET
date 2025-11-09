//! ce truc est polymorphe pour les différents types d'objectifs

using UnityEngine;

[CreateAssetMenu(fileName = "SO_ObjectiveBase", menuName = "Scriptable Objects/SO_ObjectiveBase")]
/**
 * Structure de base pour les objectifs de quête
 *
 * @author	Unknown
 * @since	v0.0.1
 * @version	v1.0.0	Sunday, November 9th, 2025.
 * @global
 */
public class SO_ObjectiveBase : ScriptableObject
{


    public string objectiveId;
    public string displayText;
    public bool isHidden;
    public float timeout;

    /**
     * Méthode abstraite pour vérifier l'achèvement de l'objectif
     *
     * @var		public	virtua
     */
    public virtual bool CheckCompletion(object eventInfo)
    {
        //& Implémentation par défaut (peut être surchargée dans les sous-classes)
        return false;
    }

}
