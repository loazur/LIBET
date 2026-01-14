using System;
using UnityEngine;


[CreateAssetMenu(fileName = "New Quest Info", menuName = "Quest System/Quest Info", order = 1)]
public class SO_QuestInfo : ScriptableObject
{
    // * =============================  ATTRIBUTS  ==========================
    [SerializeField] private string _id;
    public string QuestDescriptionFR; //& Description qui sera utiliser dans l'UIQuestMenu FR
    public string QuestDescriptionEN; //& Description qui sera utiliser dans l'UIQuestMenu EN
    public string id => _id;

    [Header("Genral Info")]
    // Le displayName est maintenant calculé à partir du stepName du premier step
    public string displayName
    {
        get
        {
            if (questStepsPrefabs != null && questStepsPrefabs.Length > 0 && questStepsPrefabs[0] != null)
            {
                S_QuestStep firstStep = questStepsPrefabs[0].GetComponent<S_QuestStep>();
                if (firstStep != null && !string.IsNullOrEmpty(firstStep.stepName))
                {
                    return firstStep.stepName;
                }
            }
            // Fallback sur le nom de l'objet si pas de step ou pas de stepName
            return this.name;
        }
    }

    [Header("Requirements")]
    public int levelRequirement;
    public SO_QuestInfo[] prerequisiteQuests;

    [Header("Steps")]
    public GameObject[] questStepsPrefabs;

    [Header("Rewards")]
    [Tooltip("Liste des récompenses à donner à la fin de la quête")]
    public SO_QuestReward[] questRewards;

    [Header("Experience Reward")]
    [Tooltip("Quantité d'expérience à donner au joueur à la fin de la quête")]
    public int experienceReward; //& Sera utiliser pour donner l'accès à certaines quêtes qui seront de plus haut lvl



    // * ====================================================================
    
    /**
     * Assure la mise à jour de l'ID lors de la modification du nom de l'objet dans l'éditeur
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Monday, November 10th, 2025.
     * @access	private
     * @return	void
     */
    private void OnValidate()
    {
        #if UNITY_EDITOR
        _id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
