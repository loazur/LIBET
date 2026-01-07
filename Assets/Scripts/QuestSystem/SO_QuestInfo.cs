using UnityEngine;


[CreateAssetMenu(fileName = "New Quest Info", menuName = "Quest System/Quest Info", order = 1)]
public class SO_QuestInfo : ScriptableObject
{
    // * =============================  ATTRIBUTS  ==========================
    [SerializeField] private string _id;
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
    // ! À MODIFIER SELON LES BESOINS DU JEU
    public int experienceReward; // ! Ne sert à rien pour le moment



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
