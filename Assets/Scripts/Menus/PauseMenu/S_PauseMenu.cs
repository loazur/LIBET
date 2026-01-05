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
    //TODO bouton en haut a droite pour quitter tout 

    public void OnContinueClicked()
    {
        DeactivateMenu();
    }

    public void OnSettingsClicked()
    {
        settingsMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnQuitClicked()
    {
        // Sauvegarde quand on quitte
        S_DataPersistanceManager.instance.SaveGame();
        
        // Changement de scene
        SceneManager.LoadSceneAsync("MainMenu");
    }

    public void ActivateMenu()
    {
        gameObject.SetActive(true);
    }

    public void DeactivateMenu()
    {
        gameObject.SetActive(false);
    }

}
