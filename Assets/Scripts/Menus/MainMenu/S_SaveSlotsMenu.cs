using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_SaveSlotsMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_MainMenu mainMenu;
    [SerializeField] private S_ConfirmationPopupMenu confirmationPopupMenu;

    private S_SaveSlot[] saveSlots;

    void Awake()
    {
        saveSlots = GetComponentsInChildren<S_SaveSlot>();
    }

    public new void ActivateMenu()
    {
        // Active le menu
        gameObject.SetActive(true);

        // Charge tout les profiles qui existent
        Dictionary<string, S_GameData> profilesGameData = S_DataPersistanceManager.instance.GetAllProfilesGameData();

        // Boucle dans tout les SaveSlot pour changer le contenu
        foreach (S_SaveSlot saveSlot in saveSlots)
        {
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out S_GameData profileData);
            saveSlot.SetData(profileData);
        }
    }

    public void OnSaveSlotClicked(S_SaveSlot saveSlot)
    {
        // Met à jour le profile id du save slot 
        S_DataPersistanceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        // Création d'une nouvelle partie

        if (!saveSlot.HasDataInSlot()) //~ Si nouvelle partie
            S_DataPersistanceManager.instance.DeleteProfileData(saveSlot.GetProfileId());

        // Charge la scène du jeu
        SceneManager.LoadSceneAsync("TestMap"); 
    }

    public void OnClearClicked(S_SaveSlot saveSlot)
    {
        confirmationPopupMenu.ActivateMenu(saveSlot);
        DeactivateMenu();
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        DeactivateMenu();
    }

}
