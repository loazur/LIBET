using UnityEngine;
using UnityEngine.UI;

public class S_ConfirmationPopupMenu : S_Menu
{
    [Header("Menu Navigation")]
    [SerializeField] private S_SaveSlotsMenu saveSlotsMenu;

    [Header("Boutons ConfirmationPopup Menu")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private S_SaveSlot saveSlot = null;

    public void OnConfirmClicked()
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_button_confirm, S_FMODEvents.instance.target.position);

        S_DataPersistanceManager.instance.DeleteProfileData(saveSlot.GetProfileId());
        saveSlotsMenu.ActivateMenu(); // Rafraichit le menu
        DeactivateMenu();
    }

    public void OnCancelClicked()
    {
        S_AudioManager.instance.PlayOneShot(S_FMODEvents.instance.ui_return, S_FMODEvents.instance.target.position);

        saveSlotsMenu.ActivateMenu(); // Rafraichit le menu
        DeactivateMenu();
    }

    public void ActivateMenu(S_SaveSlot saveSlot)
    {
        this.saveSlot = saveSlot;
        gameObject.SetActive(true);
    }

}
