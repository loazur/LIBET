using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_PauseMenu : S_Menu
{

    [Header("Menu Navigation")]
    [SerializeField] private S_SettingsMenu settingsMenu;

    [Header("Boutons Pause Menu")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    [Header("Fond du menu pause")]
    [SerializeField] private Image pauseMenuBackground;

    protected override void OnEnable()
    {
        base.OnEnable(); // Utilise le OnEnable du parent

        if (pauseMenuBackground != null)
            pauseMenuBackground.gameObject.SetActive(true);

        //S_HandlerPauseMenu.instance.DisableAll();
        S_HandlerPauseMenu.instance.setMenuOpened(true);
        S_HandlerPauseMenu.instance.setCurrentMenu(this);
    }

    public void OnContinueClicked()
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_button_confirm, S_FMODEvents.instance.target.position);

        if (pauseMenuBackground != null)
            pauseMenuBackground.gameObject.SetActive(false);
        S_HandlerPauseMenu.instance.CompletelyCloseMenu();
    }

    public void OnSettingsClicked()
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_option_click, S_FMODEvents.instance.target.position);

        if (pauseMenuBackground != null)
            pauseMenuBackground.gameObject.SetActive(true);
        settingsMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnQuitClicked()
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_return, S_FMODEvents.instance.target.position);

        if (pauseMenuBackground != null)
            pauseMenuBackground.gameObject.SetActive(false);

        // Remet l'écoulement du temps
        Time.timeScale = 1f;

        // Remet la musique du MainMenu
        S_AudioManager.instance.SetMusicArea(E_MusicArea.AREA1);

        // Désactivé le menu avant changement de scène
        DeactivateMenu();

        // Changement de scene
        S_SceneLoader.instance.LoadScene("MainMenu");
    }

}
