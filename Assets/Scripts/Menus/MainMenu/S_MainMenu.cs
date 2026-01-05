using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_MainMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_SaveSlotsMenu saveSlotsMenu;

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
        
    }

    public void OnLeaveClicked()
    {
        
    }

    
}
