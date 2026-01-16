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

    private string remainingDays; 
    private string timeplayed;

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

            TextMeshProUGUI hasDataText = hasDataContent.GetComponent<TextMeshProUGUI>();

            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                hasDataText.text = "Jour actuel : " + data.currentDay;
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                hasDataText.text = "Current day : " + data.currentDay;
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
