using UnityEngine;
using UnityEngine.EventSystems;
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

    public new void ActivateMenu()
    {
        gameObject.SetActive(true);
        // Sélectionne le bouton "Jouer" par défaut et déclenche les décorations
        if (playGameButton != null)
        {
            SetFirstSelected(playGameButton);
        }
    }

    public void OnPlayGameClicked()
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_button_confirm, S_FMODEvents.instance.target.position);

        saveSlotsMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnSettingsClicked()
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_option_click, S_FMODEvents.instance.target.position);

        settingMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnLeaveClicked()
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_return, S_FMODEvents.instance.target.position);

        Application.Quit();
        Debug.Log("Retour au bureau...");
    }

    
}
