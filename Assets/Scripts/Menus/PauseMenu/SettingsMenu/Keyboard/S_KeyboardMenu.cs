using UnityEngine;
using UnityEngine.UI;

public class S_KeyboardMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_SettingsMenu settingsMenu;

    [Header("Boutons Keyboard Menu")]
    [SerializeField] private Button returnButton;
    //TODO bouton en haut a droite pour quitter tout 

    //! Mise à jour sliders des keybinds automatiquement via le prefab lié

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
