using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_MainMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_SaveSlotsMenu saveSlotsMenu;
    [SerializeField] private S_SettingsMenu settingMenu;

    [Header("Boutons Main Menu")]
    [SerializeField] private Button playGameButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button leaveButton;

    public void OnPlayGameClicked()
    {
        saveSlotsMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnSettingsClicked()
    {
        settingMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnLeaveClicked()
    {
        Application.Quit();
        Debug.Log("Retour au bureau...");
    }

    
}
