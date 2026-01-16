using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Contenu")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private Button clearButton;

    private bool hasData = false;

    public void SetData(S_GameData data)
    {
        // No Data
        if (data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
            clearButton.gameObject.SetActive(false);

            hasData = false;
        }
        else // Data
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            clearButton.gameObject.SetActive(true);

            hasData = true;

            //~ Affichage information du slot
            TextMeshProUGUI hasDataText = hasDataContent.GetComponent<TextMeshProUGUI>();

            // Temps de jeu
            int hours = Mathf.FloorToInt(data.playTime / 3600f);
            int minutes = Mathf.FloorToInt(data.playTime % 3600f / 60f);
            int seconds = Mathf.FloorToInt(data.playTime % 60f);

            string formatedTime = $"{hours:00}:{minutes:00}:{seconds:00}";


            // Affichage dans la bonne langue
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                hasDataText.text = $"Jour actuel :  + {data.currentDay} | Temps de jeu : {formatedTime}";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                hasDataText.text =  $"Current day :  + {data.currentDay} | Play time : {formatedTime}";
            }
            
        }
    }

    public string GetProfileId()
    {
        return profileId;
    }

    public bool HasDataInSlot()
    {
        return hasData;
    }
    
}
