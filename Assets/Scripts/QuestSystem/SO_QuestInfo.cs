using UnityEngine;


[CreateAssetMenu(fileName = "New Quest Info", menuName = "Quest System/Quest Info", order = 1)]
public class SO_QuestInfo : ScriptableObject
{
    // * =============================  ATTRIBUTS  ==========================
    [SerializeField] public string id { get; private set; }

    [Header("Genral Info")]
    public string displayName;

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
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
