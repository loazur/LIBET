using UnityEngine;
using UnityEngine.UI;

public class S_ControllerMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_SettingsMenu settingsMenu;

    [Header("Boutons Controller Menu")]
    [SerializeField] private Button returnButton;
    [SerializeField] private Button leaveButton;

    //! Mise à jour sliders des keybinds automatiquement via le prefab lié

    protected override void OnEnable()
    {
        base.OnEnable(); // Utilise le OnEnable du parent

        S_HandlerPauseMenu.instance.setCurrentMenu(this);
        S_HandlerPauseMenu.instance.setMenuOpened(true);
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
