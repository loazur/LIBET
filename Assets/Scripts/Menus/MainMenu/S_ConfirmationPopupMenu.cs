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
        S_DataPersistanceManager.instance.DeleteProfileData(saveSlot.GetProfileId());
        saveSlotsMenu.ActivateMenu(); // Rafraichit le menu
        DeactivateMenu();
    }

    public void OnCancelClicked()
    {
        saveSlotsMenu.ActivateMenu(); // Rafraichit le menu
        DeactivateMenu();
    }

    public void ActivateMenu(S_SaveSlot saveSlot)
    {
        this.saveSlot = saveSlot;
        gameObject.SetActive(true);
    }

    public void DeactivateMenu()
    {
        saveSlot = null;
        gameObject.SetActive(false);
    }
}
