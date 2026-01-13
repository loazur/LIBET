using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class QuestSlotUI
{
    public Button button;
    public Text titleText;
    public Text descriptionText;
    public GameObject panel; // Pour cacher si pas de quête
    [HideInInspector] public S_Quest quest; // Référence à la quête liée
}

