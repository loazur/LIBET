using UnityEngine;

public class S_TakeKey : MonoBehaviour, SI_Interactable
{

    public bool hasKey = false; //& savoir si le joueur a la clé
    private string interactText = "not_set";


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateInteractText(); //& changement de langue

        S_GameSettingsData.instance.OnLanguageChanged += UpdateInteractText; // Gère changement langue
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //! Méthodes provenant de l'interface SI_Interactable
    //! =====================================================

    public void Interact(Transform playerTransform)
    {
        //& Récupère la clé:
        //& destruire l'objet clé dans la scène
        //& changer l'état de hasKey à true
        hasKey = true;
        Destroy(gameObject);
    }

    public string getInteractText()
    {
        return interactText;
    }

    public Transform getTransform()
    {
        return gameObject.transform;
    }

    //! =====================================================


    private void UpdateInteractText() //& Gestion du texte en fonction de la langue
    {
        if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.French)
        {
            interactText = "Prendre la clé";
        }
        else if (S_GameSettingsData.instance.currentLanguage == S_GameSettingsData.Languages.English)
        {
            interactText = "Take Key";
        }
    }
}
