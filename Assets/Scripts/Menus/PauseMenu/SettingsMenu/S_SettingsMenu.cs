using UnityEngine;
using UnityEngine.UI;

public class S_SettingsMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_PauseMenu pauseMenu;
    [SerializeField] private S_AudioMenu audioMenu;

    [Header("Boutons Settings Menu")]
    [SerializeField] private Button gameButton;
    [SerializeField] private Button audioButton;
    [SerializeField] private Button videoButton;
    [SerializeField] private Button cameraButton;
    [SerializeField] private Button keyboardButton;
    [SerializeField] private Button controllerButton;
    [SerializeField] private Button returnButton;
    //TODO bouton en haut a droite pour quitter tout 

    public void OnGameClicked()
    {
        
    }

    public void OnAudioClicked()
    {
        audioMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnVideoClicked()
    {
        
    }

    public void OnCameraClicked()
    {
        
    }

    public void OnKeyboardClicked()
    {
        
    }

    public void OnControllerClicked()
    {
        
    }

    public void OnReturnClicked()
    {
        pauseMenu.ActivateMenu();
        DeactivateMenu();
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
