using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_button_confirm, S_FMODEvents.instance.target.position);

        // Met à jour le profile id du save slot 
        S_DataPersistanceManager.instance.ChangeSelectedProfileId(saveSlot.GetProfileId());

        // Création d'une nouvelle partie

        if (!saveSlot.HasDataInSlot()) //~ Si nouvelle partie
            S_DataPersistanceManager.instance.DeleteProfileData(saveSlot.GetProfileId());

        // Changement de musique
        S_AudioManager.instance.SetMusicArea(E_MusicArea.AREA2);

        // Charge la scène du jeu
        S_SceneLoader.instance.LoadScene("Game");
    }

    public void OnClearClicked(S_SaveSlot saveSlot)
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_change_selection, S_FMODEvents.instance.target.position);

        confirmationPopupMenu.ActivateMenu(saveSlot);
        DeactivateMenu();
    }

    public void OnBackClicked()
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_return, S_FMODEvents.instance.target.position);

        mainMenu.ActivateMenu();
        DeactivateMenu();
    }

}
