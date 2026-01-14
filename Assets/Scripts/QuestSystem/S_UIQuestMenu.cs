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
 *       |-- Button Quest Story (OnClick)
 *           |-- Text Quest Story Title
 *           |-- Text Quest Story Description
 *       |-- Button Quest Side 1 (OnClick)
 *           |-- Text Quest Side Title 1
 *           |-- Text Quest Side Description 1
 *       |-- Button Quest Side 2 (OnClick)
 *           |-- Text Quest Side Title 2
 *           |-- Text Quest Side Description 2
 *       |-- Button Quest Side 3 (OnClick)
 *           |-- Text Quest Side Title 3
 *           |-- Text Quest Side Description 3
 */

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class S_UIQuestMenu : MonoBehaviour
{
    public static S_UIQuestMenu instance { get; private set; }

    [Header("UI Quest Menu")]
    [SerializeField] private GameObject uiQuestMenu;

    [Header("Quête Histoire")]
    [SerializeField] private S_QuestPoint storyQuestPoint; // QuestPoint de la quête principale
    [SerializeField] private Button questStoryButton;
    [SerializeField] private TextMeshProUGUI questStoryTitleText;
    [SerializeField] private TextMeshProUGUI questStoryDescriptionText;
    [SerializeField] private GameObject questStoryPanel; // Panel pour cacher/montrer si pas de quête story

    [Header("Quêtes Secondaires")]
    [SerializeField] private QuestSlotUI[] questSideSlots = new QuestSlotUI[3];

    [Header("Visual Feedback")]
    [SerializeField] private Color selectedQuestColor = new Color(0.8f, 1f, 0.8f, 1f);
    [SerializeField] private Color normalQuestColor = Color.white;

    // Cache de la quête principale liée au QuestPoint
    private S_Quest storyQuest;
    private bool isSubscribed = false;

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

        // S'abonner aux changements d'état de quête
        SubscribeToQuestEvents();
    }

    void OnDestroy()
    {
        UnsubscribeFromQuestEvents();
    }

    private void SubscribeToQuestEvents()
    {
        if (isSubscribed || S_GameManager.instance == null) return;

        S_GameManager.instance.questEvents.onQuestStateChange += OnQuestStateChange;
        isSubscribed = true;
    }

    private void UnsubscribeFromQuestEvents()
    {
        if (!isSubscribed || S_GameManager.instance == null) return;

        S_GameManager.instance.questEvents.onQuestStateChange -= OnQuestStateChange;
        isSubscribed = false;
    }

    /**
     * Callback quand l'état d'une quête change
     * Met à jour l'UI si c'est la quête principale (storyQuestPoint)
     */
    private void OnQuestStateChange(S_Quest quest)
    {
        // Vérifier si c'est la quête principale assignée
        if (storyQuestPoint != null && quest.info.id == storyQuestPoint.QuestId)
        {
            storyQuest = quest;
            RefreshIfOpen();
        }
    }

    void Update()
    { 
        //& Touche pour ouvrir/fermer le menu des quêtes
        if (S_UserInput.instance != null && S_UserInput.instance.QuestMenuInput)
        {
            ToggleQuestMenu();
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

        if (!isOpen) //~ Ouvert
        {
            //& Réactiver le curseur de la souris si le menu est ouvert
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //& Bloquer la caméra du joueur
            if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
            {
                S_GameManager.instance.playerEvents.LockPlayerCamera(true);
            }
            
            // Le menu vient de s'ouvrir
            UpdateQuestMenuUI();
            
            // Notifier le GameManager que le menu est ouvert (pour pause, curseur, etc.)
            if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
            {
                S_GameManager.instance.playerEvents.MenuOpened();
            }
        }
        else //~ Fermé
        {
            //& Re-locker le curseur si le menu est fermé
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //& Débloquer la caméra du joueur
            if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
            {
                S_GameManager.instance.playerEvents.LockPlayerCamera(false);
            }

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

            // Re-locker le curseur si le menu est fermé
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Débloquer la caméra du joueur et notifier la fermeture
            if (S_GameManager.instance != null && S_GameManager.instance.playerEvents != null)
            {
                S_GameManager.instance.playerEvents.LockPlayerCamera(false);
                S_GameManager.instance.playerEvents.MenuClosed();
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

        //& Mettre à jour la quête d'histoire
        UpdateStoryQuestUI();

        //& Mettre à jour les quêtes secondaires
        UpdateSideQuestsUI();

        //& Mettre en surbrillance la quête sélectionnée
        UpdateSelectionHighlight();
    }

    /**
     * Met à jour l'affichage de la quête d'histoire
     * Utilise le storyQuestPoint assigné dans l'Inspector
     * Note: Les quêtes FINISHED sont masquées
     */
    private void UpdateStoryQuestUI()
    {
        // Afficher seulement si la quête est en cours ou peut être terminée (pas FINISHED)
        bool shouldDisplay = storyQuest != null && 
                            (storyQuest.state == E_QuestState.IN_PROGRESS || 
                             storyQuest.state == E_QuestState.CAN_FINISH);

        if (shouldDisplay)
        {
            if (questStoryPanel != null) questStoryPanel.SetActive(true);
            
            if (questStoryTitleText != null)
            {
                questStoryTitleText.text = storyQuest.GetCurrentStepDisplayName();
                
                // Ajouter un indicateur pour les quêtes prêtes à être terminées
                if (storyQuest.state == E_QuestState.CAN_FINISH)
                {
                    questStoryTitleText.text += " !";
                }
            }
            
            if (questStoryDescriptionText != null)
            {
                questStoryDescriptionText.text = GetQuestDescription(storyQuest);
            }
        }
        else
        {
            // Pas de quête d'histoire active ou quête terminée
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
     * Note: Les quêtes FINISHED sont filtrées et ne sont pas affichées
     */
    private void UpdateSideQuestsUI()
    {
        S_Quest[] allSideQuests = S_QuestManager.instance.GetSideQuests();
        
        // Filtrer les quêtes terminées - on n'affiche que les quêtes IN_PROGRESS ou CAN_FINISH
        System.Collections.Generic.List<S_Quest> activeSideQuests = new System.Collections.Generic.List<S_Quest>();
        foreach (S_Quest sideQuest in allSideQuests)
        {
            if (sideQuest != null && sideQuest.state != E_QuestState.FINISHED)
            {
                activeSideQuests.Add(sideQuest);
            }
        }

        for (int i = 0; i < questSideSlots.Length; i++)
        {
            QuestSlotUI slot = questSideSlots[i];
            
            if (i < activeSideQuests.Count && activeSideQuests[i] != null)
            {
                S_Quest quest = activeSideQuests[i];
                slot.quest = quest;
                
                if (slot.panel != null) slot.panel.SetActive(true);
                
                if (slot.titleText != null)
                {
                    slot.titleText.text = quest.GetCurrentStepDisplayName();
                    
                    // Ajouter un indicateur pour les quêtes prêtes à être terminées
                    if (quest.state == E_QuestState.CAN_FINISH)
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

    #region Button Callbacks (Public - pour OnClick Unity Inspector)

    //* Fonction à config sur le bouton dans l'Inspector Unity
    //*===========================================================================================
    /**
     * Appelé quand le joueur clique sur la quête d'histoire
     * À assigner dans l'Inspector: Button.OnClick -> S_UIQuestMenu.OnClickStoryQuest()
     */
    public void OnClickStoryQuest()
    {
        Debug.Log("<color=yellow>[UIQuestMenu]</color> OnClickStoryQuest appelé!");
        
        if (S_QuestManager.instance == null) return;
        
        if (storyQuest != null && (storyQuest.state == E_QuestState.IN_PROGRESS || storyQuest.state == E_QuestState.CAN_FINISH))
        {
            S_QuestManager.instance.SetSelectedQuestForDisplay(storyQuest);
            UpdateSelectionHighlight();
            Debug.Log($"<color=cyan>[UIQuestMenu]</color> Quête histoire sélectionnée: {storyQuest.info.displayName}");
        }
    }

    
    /**
     * Appelé quand le joueur clique sur la quête secondaire 1
     * À assigner dans l'Inspector: Button.OnClick -> S_UIQuestMenu.OnClickSideQuest1()
     */
    public void OnClickSideQuest1()
    {
        Debug.Log("<color=yellow>[UIQuestMenu]</color> OnClickSideQuest1 appelé!");
        SelectSideQuest(0);
    }

    /**
     * Appelé quand le joueur clique sur la quête secondaire 2
     * À assigner dans l'Inspector: Button.OnClick -> S_UIQuestMenu.OnClickSideQuest2()
     */
    public void OnClickSideQuest2()
    {
        Debug.Log("<color=yellow>[UIQuestMenu]</color> OnClickSideQuest2 appelé!");
        SelectSideQuest(1);
    }

    /**
     * Appelé quand le joueur clique sur la quête secondaire 3
     * À assigner dans l'Inspector: Button.OnClick -> S_UIQuestMenu.OnClickSideQuest3()
     */
    public void OnClickSideQuest3()
    {
        Debug.Log("<color=yellow>[UIQuestMenu]</color> OnClickSideQuest3 appelé!");
        SelectSideQuest(2);
    }
    //*===========================================================================================

    /**
     * Sélectionne une quête secondaire par son index
     */
    private void SelectSideQuest(int index)
    {
        Debug.Log($"<color=yellow>[UIQuestMenu]</color> SelectSideQuest appelé pour index: {index}");
        
        if (S_QuestManager.instance == null) return;
        
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

    #region Language

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

    #region Debug

    [ContextMenu("Side Quest 1 Click")]
    private void DebugClickSideQuest1()
    {
        OnClickSideQuest1();
    }

    [ContextMenu("Side Quest 2 Click")]
    private void DebugClickSideQuest2()
    {
        OnClickSideQuest2();
    }

    [ContextMenu("Side Quest 3 Click")]
    private void DebugClickSideQuest3()
    {
        OnClickSideQuest3();
    }

    [ContextMenu("Story Quest Click")]
    private void DebugClickStoryQuest()
    {
        OnClickStoryQuest();
    }

    #endregion

}