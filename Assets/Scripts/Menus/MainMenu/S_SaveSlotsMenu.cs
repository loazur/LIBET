using System.Collections.Generic;
using UnityEngine;

public class S_SaveSlotsMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private S_MainMenu mainMenu;

    private S_SaveSlot[] saveSlots;

    void Awake()
    {
        saveSlots = GetComponentsInChildren<S_SaveSlot>();
    }

    public void ActivateMenu()
    {
        // Active le menu
        gameObject.SetActive(true);

        // Charge tout les profiles qui existent
        Dictionary<string, S_GameData> profilesGameData = S_DataPersistanceManager.instance.GetAllProfilesGameData();

        // Boucle dans tout les SaveSlot pour changer le contenu
        foreach (S_SaveSlot saveSlot in saveSlots)
        {
            S_GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);
        }
    }

    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        DeactivateMenu();
    }
}
