using UnityEngine;
using UnityEngine.UI;

public class S_AudioMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_SettingsMenu settingsMenu;

    [Header("Boutons Audio Menu")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button returnButton;
    //TODO bouton en haut a droite pour quitter tout 

    //! Mise à jour sliders / Boutons resets sont géré directement via S_AudioUserData

    public void OnSaveClicked()
    {
        S_AudioUserData.instance.SaveData();
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
