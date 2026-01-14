using UnityEngine;
using UnityEngine.UI;
using TMPro;


[System.Serializable]
public class QuestSlotUI
{
    public Button button;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public GameObject panel; // Pour cacher si pas de quête
    [HideInInspector] public S_Quest quest; // Référence à la quête liée
}

