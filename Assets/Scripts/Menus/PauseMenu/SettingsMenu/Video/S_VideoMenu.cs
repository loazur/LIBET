using UnityEngine;
using UnityEngine.UI;

public class S_VideoMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_SettingsMenu settingsMenu;

    [Header("Boutons Video Menu")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button leaveButton;

    //! Mise à jour sliders / Boutons resets sont géré directement via S_VideoUserData

    protected override void OnEnable()
    {
        base.OnEnable(); // Utilise le OnEnable du parent

        S_HandlerPauseMenu.instance.setCurrentMenu(this);
        S_HandlerPauseMenu.instance.setMenuOpened(true);
    }

    public void OnSaveClicked()
    {
        S_VideoUserData.instance.SaveData();
    }

    public void OnReturnClicked()
    {
        settingsMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnLeaveButton()
    {
        S_HandlerPauseMenu.instance.CompletelyCloseMenu();
    }

}
