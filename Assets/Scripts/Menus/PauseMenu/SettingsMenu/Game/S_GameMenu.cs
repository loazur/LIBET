using UnityEngine;
using UnityEngine.UI;

public class S_GameMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_SettingsMenu settingsMenu;

    [Header("Boutons Game Menu")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button leaveButton;

    //! Mise à jour sliders / Boutons resets sont géré directement via S_GameUserData

    protected override void OnEnable()
    {
        base.OnEnable(); // Utilise le OnEnable du parent

        S_HandlerPauseMenu.instance.setCurrentMenu(this);
        S_HandlerPauseMenu.instance.setMenuOpened(true);
    }

    public void OnSaveClicked()
    {
        S_GameUserData.instance.SaveData();
    }

    public void OnReturnClicked()
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_return, S_FMODEvents.instance.target.position);

        settingsMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnLeaveButton()
    {
        S_HandlerPauseMenu.instance.CompletelyCloseMenu();
    }


}
