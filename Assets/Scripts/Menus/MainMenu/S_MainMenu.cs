using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class S_MainMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private S_SaveSlotsMenu saveSlotsMenu;

    [Header("Boutons Main Menu")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;

    void Start()
    {
        if (!S_DataPersistanceManager.instance.HasGameData())
        {
            continueButton.interactable = false;
        }

    }

    //!----------------------------------------

    public void OnNewGameClicked()
    {
        saveSlotsMenu.ActivateMenu();
        DeactivateMenu();
        /*
        // Création d'une nouvelle partie
        S_DataPersistanceManager.instance.DeleteSaveData();

        // Charge la scène du jeu
        SceneManager.LoadSceneAsync("TestMap");
        */
    }

    public void OnContinueGameClicked()
    {
        // Charge la scène du jeu
        SceneManager.LoadSceneAsync("TestMap");
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
