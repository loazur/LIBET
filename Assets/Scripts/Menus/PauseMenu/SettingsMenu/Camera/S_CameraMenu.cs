using UnityEngine;
using UnityEngine.UI;

public class S_CameraMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_SettingsMenu settingsMenu;

    [Header("Boutons Camera Menu")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button returnButton;
    //TODO bouton en haut a droite pour quitter tout 

    //! Mise à jour sliders / Boutons resets sont géré directement via S_CameraUserData

    public void OnSaveClicked()
    {
        S_CameraUserData.instance.SaveData();
    }

    public void OnReturnClicked()
    {
        settingsMenu.ActivateMenu();
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
