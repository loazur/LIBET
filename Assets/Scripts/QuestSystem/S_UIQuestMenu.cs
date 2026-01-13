/**
 * S_UIQuestMenu.cs
 * Fonctionnalités:
 * - Ouvrir un menu d'affichage des quêtes avec la touche "I"
 * - Choisir la quête à afficher dans l'UI des Objectifs qui se trouve dans le QuestManager
 * - Support multi-langues (FR/EN)
 * 
 * Organigrame de l'UI des quêtes:
 *   GameObject UIQuestMenu
 *       |-- Panel Background
 *       |-- Button Quest Story
 *           |-- Text Quest Story Title
 *           |-- Text Quest Story Description
 *       |-- Button Quest Side 1
 *           |-- Text Quest Side Title 1
 *           |-- Text Quest Side Description 1
 *       |-- Button Quest Side 2
 *           |-- Text Quest Side Title 2
 *           |-- Text Quest Side Description 2
 *       |-- Button Quest Side 3
 *           |-- Text Quest Side Title 3
 *           |-- Text Quest Side Description 3
 */

using UnityEngine;
using UnityEngine.UI;

public class S_UIQuestMenu : MonoBehaviour
{
    public static S_UIQuestMenu instance { get; private set; }

    [Header("UI Quest Menu")]
    [SerializeField] private GameObject uiQuestMenu;

    [Header("Quête Histoire")]
    [SerializeField] private Button questStoryButton;
    [SerializeField] private Text questStoryTitleText;
    [SerializeField] private Text questStoryDescriptionText;
    [SerializeField] private GameObject questStoryPanel; // Panel pour cacher/montrer si pas de quête story

    [Header("Quêtes Secondaires")]
    [SerializeField] private QuestSlotUI[] questSideSlots = new QuestSlotUI[3];

    [Header("Visual Feedback")]
    [SerializeField] private Color selectedQuestColor = new Color(0.8f, 1f, 0.8f, 1f);
    [SerializeField] private Color normalQuestColor = Color.white;

    // Quête actuellement en surbrillance dans le menu
    private S_Quest currentlySelectedQuest;

    

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Fermer le menu au démarrage
        if (uiQuestMenu != null)
        {
            uiQuestMenu.SetActive(false); //& Assurer que le menu est fermé au début
        }

        // Setup des listeners de boutons
        SetupButtonListeners();
    }

    void Update()
    { 
        //& Touche pour ouvrir/fermer le menu des quêtes
        if (S_UserInput.instance != null && S_UserInput.instance.QuestMenuInput)
        {
            ToggleQuestMenu();

            // Activer/désactiver le curseur de la souris et verrouiller/déverrouiller la caméra
            if (uiQuestMenu.activeSelf)
            {
                // Réactiver le curseur de la souris si le menu est ouvert
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                // Bloquer la caméra du joueur
                if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
                {
                    S_GameManager.instance.playerEvents.LockPlayerCamera(true);
                }
            }
            else
            {
                // Re-locker le curseur si le menu est fermé
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                // Débloquer la caméra du joueur
                if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
                {
                    S_GameManager.instance.playerEvents.LockPlayerCamera(false);
                }
            }
        } 
    }

    #region Menu Toggle 

    /**
     * Ouvre ou ferme le menu des quêtes
     */
    public void ToggleQuestMenu()
    {
        if (uiQuestMenu == null) return;

        bool isOpen = uiQuestMenu.activeSelf;
        uiQuestMenu.SetActive(!isOpen);

        if (!isOpen)
        {
            // Le menu vient de s'ouvrir
            UpdateQuestMenuUI();
            
            // Notifier le GameManager que le menu est ouvert (pour pause, curseur, etc.)
            if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
            {
                S_GameManager.instance.playerEvents.MenuOpened();
            }
        }
        else
        {
            // Le menu vient de se fermer
            if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
            {
                S_GameManager.instance.playerEvents.MenuClosed();
            }
        }
    }

    /**
     * Force la fermeture du menu
     */
    public void CloseQuestMenu()
    {
        if (uiQuestMenu != null && uiQuestMenu.activeSelf)
        {
            uiQuestMenu.SetActive(false);
            
            if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
            {
                S_GameManager.instance.playerEvents.MenuClosed();
            }
        }
    }

    /**
     * Force l'ouverture du menu
     */
    public void OpenQuestMenu()
    {
        if (uiQuestMenu != null && !uiQuestMenu.activeSelf)
        {
            uiQuestMenu.SetActive(true);
            UpdateQuestMenuUI();
            
            if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
            {
                S_GameManager.instance.playerEvents.MenuOpened();
            }
        }
    }

    #endregion

    #region Button Setup

    /**
     * Configure les listeners des boutons de quête
     */
    private void SetupButtonListeners()
    {
        // Bouton quête histoire
        if (questStoryButton != null)
        {
            questStoryButton.onClick.AddListener(OnStoryQuestClicked);
        }

        // Boutons quêtes secondaires
        for (int i = 0; i < questSideSlots.Length; i++)
        {
            int index = i; // Capture pour closure
            if (questSideSlots[i].button != null)
            {
                questSideSlots[i].button.onClick.AddListener(() => OnSideQuestClicked(index));
            }
        }
    }

    #endregion

    #region UI Update

    /**
     * Met à jour l'affichage complet du menu des quêtes
     */
    public void UpdateQuestMenuUI()
    {
        if (S_QuestManager.instance == null)
        {
            Debug.LogWarning("[S_UIQuestMenu] S_QuestManager.instance est null!");
            return;
        }

        // Mettre à jour la quête d'histoire
        UpdateStoryQuestUI();

        // Mettre à jour les quêtes secondaires
        UpdateSideQuestsUI();

        // Mettre en surbrillance la quête sélectionnée
        UpdateSelectionHighlight();
    }

    /**
     * Met à jour l'affichage de la quête d'histoire
     */
    private void UpdateStoryQuestUI()
    {
        S_Quest storyQuest = S_QuestManager.instance.GetStoryQuest();

        if (storyQuest != null && storyQuest.state == E_QuestState.IN_PROGRESS)
        {
            if (questStoryPanel != null) questStoryPanel.SetActive(true);
            
            if (questStoryTitleText != null)
            {
                questStoryTitleText.text = storyQuest.GetCurrentStepDisplayName();
            }
            
            if (questStoryDescriptionText != null)
            {
                questStoryDescriptionText.text = GetQuestDescription(storyQuest);
            }
        }
        else
        {
            // Pas de quête d'histoire active
            if (questStoryPanel != null) questStoryPanel.SetActive(false);
            
            if (questStoryTitleText != null)
            {
                questStoryTitleText.text = GetLocalizedText("Aucune quête principale", "No main quest");
            }
            
            if (questStoryDescriptionText != null)
            {
                questStoryDescriptionText.text = "";
            }
        }
    }

    /**
     * Met à jour l'affichage des quêtes secondaires
     */
    private void UpdateSideQuestsUI()
    {
        S_Quest[] sideQuests = S_QuestManager.instance.GetSideQuests();

        for (int i = 0; i < questSideSlots.Length; i++)
        {
            QuestSlotUI slot = questSideSlots[i];
            
            if (i < sideQuests.Length && sideQuests[i] != null)
            {
                S_Quest quest = sideQuests[i];
                slot.quest = quest;
                
                if (slot.panel != null) slot.panel.SetActive(true);
                
                if (slot.titleText != null)
                {
                    slot.titleText.text = quest.GetCurrentStepDisplayName();
                    
                    // Ajouter un indicateur d'état
                    if (quest.state == E_QuestState.FINISHED)
                    {
                        slot.titleText.text += " ✓";
                    }
                    else if (quest.state == E_QuestState.CAN_FINISH)
                    {
                        slot.titleText.text += " !";
                    }
                }
                
                if (slot.descriptionText != null)
                {
                    slot.descriptionText.text = GetQuestDescription(quest);
                }
            }
            else
            {
                // Pas de quête pour ce slot
                slot.quest = null;
                
                if (slot.panel != null) slot.panel.SetActive(false);
                
                if (slot.titleText != null)
                {
                    slot.titleText.text = GetLocalizedText("Aucune quête", "No quest");
                }
                
                if (slot.descriptionText != null)
                {
                    slot.descriptionText.text = "";
                }
            }
        }
    }

    /**
     * Met à jour la surbrillance de la quête sélectionnée
     */
    private void UpdateSelectionHighlight()
    {
        S_Quest selected = S_QuestManager.instance.GetSelectedQuestForDisplay();

        // Quête histoire
        if (questStoryButton != null)
        {
            S_Quest storyQuest = S_QuestManager.instance.GetStoryQuest();
            ColorBlock colors = questStoryButton.colors;
            colors.normalColor = (storyQuest != null && storyQuest == selected) ? selectedQuestColor : normalQuestColor;
            questStoryButton.colors = colors;
        }

        // Quêtes secondaires
        for (int i = 0; i < questSideSlots.Length; i++)
        {
            if (questSideSlots[i].button != null)
            {
                ColorBlock colors = questSideSlots[i].button.colors;
                colors.normalColor = (questSideSlots[i].quest != null && questSideSlots[i].quest == selected) 
                    ? selectedQuestColor 
                    : normalQuestColor;
                questSideSlots[i].button.colors = colors;
            }
        }
    }

    #endregion

    #region Button Callbacks

    /**
     * Appelé quand le joueur clique sur la quête d'histoire
     */
    private void OnStoryQuestClicked()
    {
        S_Quest storyQuest = S_QuestManager.instance.GetStoryQuest();
        
        if (storyQuest != null && storyQuest.state == E_QuestState.IN_PROGRESS)
        {
            S_QuestManager.instance.SetSelectedQuestForDisplay(storyQuest);
            UpdateSelectionHighlight();
            Debug.Log($"<color=cyan>[UIQuestMenu]</color> Quête histoire sélectionnée: {storyQuest.info.displayName}");
        }
    }

    /**
     * Appelé quand le joueur clique sur une quête secondaire
     */
    private void OnSideQuestClicked(int index)
    {
        if (index >= 0 && index < questSideSlots.Length)
        {
            S_Quest quest = questSideSlots[index].quest;
            
            if (quest != null && quest.state == E_QuestState.IN_PROGRESS)
            {
                S_QuestManager.instance.SetSelectedQuestForDisplay(quest);
                UpdateSelectionHighlight();
                Debug.Log($"<color=cyan>[UIQuestMenu]</color> Quête secondaire {index + 1} sélectionnée: {quest.info.displayName}");
            }
        }
    }

    #endregion

    #region Localization Helpers

    /**
     * Retourne le texte localisé selon la langue actuelle
     */
    private string GetLocalizedText(string french, string english)
    {
        if (S_GameUserData.instance == null)
        {
            return french; // Défaut français
        }

        return S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French 
            ? french 
            : english;
    }

    /**
     * Récupère la description de la quête dans la bonne langue
     */
    private string GetQuestDescription(S_Quest quest)
    {
        if (quest == null || quest.info == null)
        {
            return "";
        }

        if (S_GameUserData.instance == null)
        {
            return quest.info.QuestDescriptionFR; //& Défaut français
        }

        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            return !string.IsNullOrEmpty(quest.info.QuestDescriptionFR) 
                ? quest.info.QuestDescriptionFR 
                : quest.info.QuestDescriptionEN;
        }
        else
        {
            return !string.IsNullOrEmpty(quest.info.QuestDescriptionEN) 
                ? quest.info.QuestDescriptionEN 
                : quest.info.QuestDescriptionFR;
        }
    }

    #endregion

    #region Public Methods

    /**
     * Vérifie si le menu est ouvert
     */
    public bool IsMenuOpen()
    {
        return uiQuestMenu != null && uiQuestMenu.activeSelf;
    }

    /**
     * Rafraîchit l'UI si le menu est ouvert
     */
    public void RefreshIfOpen()
    {
        if (IsMenuOpen())
        {
            UpdateQuestMenuUI();
        }
    }

    #endregion
}