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
    [SerializeField] private TextMeshProUGUI hasDataText;
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
            // Affichage de l'UI
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            clearButton.gameObject.SetActive(true);

            hasData = true;

            //~ Affichage information du slot
            // Affichage dans la bonne langue
            if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
            {
                hasDataText.text = $"Jour actuel : {data.getCurrentDay()} | Temps de jeu : {data.getPlayTime()}";
            }
            else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
            {
                hasDataText.text =  $"Current day : {data.getCurrentDay()} | Play time : {data.getPlayTime()}";
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
