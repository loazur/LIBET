using TMPro;
using UnityEngine;

public class S_SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Contenu")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI percentageCompleteText; //TODO - Trouver quelque chose a afficher sur le slot: Location? Nombre de clés? Temps de jeu?

    public void SetData(S_GameData data)
    {
        // No Data
        if (data == null)
        {
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }
        else // Data
        {
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);

            //TODO - Changer le texte affiché ici

        }
    }

    public string GetProfileId()
    {
        return profileId;
    }

    
}
