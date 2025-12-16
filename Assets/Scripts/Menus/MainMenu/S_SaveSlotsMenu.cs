using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_SaveSlotsMenu : S_Menu
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
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out S_GameData profileData);
            saveSlot.SetData(profileData);
        }
    }

    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
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
        S_DataPersistanceManager.instance.DeleteProfileData(saveSlot.GetProfileId());
        ActivateMenu(); // Rafraichit le menu
    }

    public void OnBackClicked()
    {
        mainMenu.ActivateMenu();
        DeactivateMenu();
    }

}
