using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.UI;

class S_Piano : MonoBehaviour, SI_Interactable
{
    [Header("Piano Keys Settings")]
    [SerializeField] private float timePressed = 0.2f;
    [SerializeField] private int numberOfKeysSimulated = 5;
    [SerializeField] private float playInterval = 0.5f;

    [Header("Music")]
    [SerializeField] private SO_PianoTrack[] pianoTracks;

    [Header("UI")]
    [SerializeField] private GameObject musicUI;
    [SerializeField] private Text trackNameText;
    [SerializeField] private Text trackAuthorText;

    [Header("Interaction Range")]
    [SerializeField] private float stopDistance = 3f;

    private Transform currentPlayer;
    private SO_PianoTrack currentTrack;
    private StudioEventEmitter pianoEmitter;
    private Coroutine playRoutine;

    private float keyPressDepth = 0.022f;
    private List<GameObject> pianoKeys = new();
    private string interactText = "Jouer du piano";


    //*========================================================


    void Start()
    {
        //DisableUIMusic(); //& S'assurer que l'UI est désactivée au départ
        UpdateInteractText(); //& Met à jour le texte d'interaction en fonction de la langue

        pianoEmitter = S_AudioManager.instance.InitializeEventEmitter(S_FMODEvents.instance.piano, gameObject);
        pianoEmitter.Stop();

        foreach (Transform child in transform)
            if (child.name.StartsWith("touche"))
                pianoKeys.Add(child.gameObject);

        DisableUIMusic();
    }

    void Update()
    {
        if (!pianoEmitter.IsPlaying() || currentPlayer == null)
            return;

        float distance = Vector3.Distance(transform.position, currentPlayer.position);

        if (distance > stopDistance)
            StopPlaying();
    }



    // ===================== INTERACTION =====================
    #region Gestion Interaction

    /**
     * Toggle le jeu du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	public
     * @param	transform	playerTransform	
     * @return	void
     */
    public void Interact(Transform playerTransform)
    {
        if (pianoEmitter.IsPlaying())
        {
            StopPlaying();
        }
        else
        {
            currentPlayer = playerTransform;
            StartPlaying();
        }
    }


    /**
     * Fonctions de gestion du jeu du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @param	transform	player	
     * @return	void
     */
    private void StartPlaying()
    {
        pianoEmitter.Play();
        currentTrack = GetRandomPianoTrack();
        SetPianoTrack(currentTrack.track);

        playRoutine = StartCoroutine(PlayLoop());
        EnableUIMusic();
        UpdateMusicUI();
    }

    /**
     * Arrête le jeu du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @return	void
     */
    private void StopPlaying()
    {
        if (!pianoEmitter.IsPlaying()) return;

        pianoEmitter.Stop();

        if (playRoutine != null)
            StopCoroutine(playRoutine);

        DisableUIMusic();
    }
    #endregion Gestion Interaction

    

    /**
     * Coroutine de simulation du jeu du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @return	void
     */
    private IEnumerator PlayLoop()
    {
        while (pianoEmitter.IsPlaying())
        {
            MoveAllTouches();
            yield return new WaitForSeconds(playInterval);
        }
    }

    
    #region TOUCHES

    /**
     * Simule l'appui sur plusieurs touches du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @return	void
     */
    private void MoveAllTouches()
    {
        List<GameObject> selectedKeys = SelectRandomKeys();

        foreach (GameObject key in selectedKeys)
            MoveOneTouch(key);

        StartCoroutine(ResetKeysAfterDelay(selectedKeys, timePressed));
    }



    /**
     * Réinitialise les touches après un délai
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @param	mixed	keys 	
     * @param	float	delay	
     * @return	mixed
     */
    private IEnumerator ResetKeysAfterDelay(List<GameObject> keys, float delay)
    {
        yield return new WaitForSeconds(delay);

        foreach (GameObject key in keys)
            ResetOneTouch(key);
    }

    /**
     * Bouger une touche du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @param	gameobject	key	
     * @return	void
     */
    private void MoveOneTouch(GameObject key)
    {
        key.transform.localPosition -= new Vector3(0, keyPressDepth, 0);
    }

    /**
     * Reset une touche du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @param	gameobject	key	
     * @return	void
     */
    private void ResetOneTouch(GameObject key)
    {
        key.transform.localPosition += new Vector3(0, keyPressDepth, 0);
    }

    /**
     * Sectionner des touches aléatoires
     *
     * @var		mixed	SelectRandomKeys()
     */
    private List<GameObject> SelectRandomKeys()
    {
        List<GameObject> selected = new();
        HashSet<int> used = new();

        while (selected.Count < numberOfKeysSimulated && selected.Count < pianoKeys.Count)
        {
            int i = Random.Range(0, pianoKeys.Count);
            if (used.Add(i))
                selected.Add(pianoKeys[i]);
        }

        return selected;
    }

    #endregion TOUCHES
    #region MUSIC

    /**
     * Obtient une piste de piano aléatoire
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 7th, 2026.
     * @access	private
     * @return	mixed
     */
    private SO_PianoTrack GetRandomPianoTrack() //& Récupére une track aléatoire en fonction de la liste
    {
        int index = Random.Range(0, pianoTracks.Length);
        return pianoTracks[index];
    }

    /**
     * Définit la track du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 7th, 2026.
     * @access	public
     * @param	s_pianotrack	track	
     * @return	void
     */
    public void SetPianoTrack(S_PianoTrack track) //& Change la track
    {
        pianoEmitter.EventInstance.setParameterByName("track", (float)track);
    }
    

    #endregion MUSIC

    #region Accesseurs

    /**
     * Obtient la position du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 7th, 2026.
     * @access	public
     * @return	void
     */
    public Transform getTransform() => transform;

    #endregion Accesseurs
    
    #region UI

    /**
     * Mettre à jour le texte d'interaction en fonction de la langue
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 7th, 2026.
     * @access	private
     * @return	void
     */
    private void UpdateInteractText() 
    {
        if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.French)
        {
            interactText = "Jouer du piano";
        }
        else if (S_GameUserData.instance.currentLanguage == S_GameUserData.Languages.English)
        {
            interactText = "Play the piano";
        }
    }

    /**
     * Gestion du texte d'interaction
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 7th, 2026.
     * @access	public
     * @return	mixed
     */
    public string getInteractText()
    {
        return pianoEmitter.IsPlaying() ? "Arrêter de jouer" : interactText;
    }

    /**
     * met à jour l'UI de la musique
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 7th, 2026.
     * @access	private
     * @return	void
     */
    private void UpdateMusicUI()
    {
        if (musicUI != null)
            musicUI.SetActive(true);

        if (trackNameText != null)
            trackNameText.text = currentTrack.trackName;

        if (trackAuthorText != null)
            trackAuthorText.text = currentTrack.author;
    }

    /**
     * Désactive l'UI de la musique
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 7th, 2026.
     * @access	public
     * @return	void
     */
    public void DisableUIMusic()
    {
        if (musicUI != null)
            musicUI.SetActive(false);
    }

    /**
     * Active l'UI de la musique
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 7th, 2026.
     * @access	public
     * @return	void
     */
    public void EnableUIMusic()
    {
        if (musicUI != null)
            musicUI.SetActive(true);
    }
    #endregion UI
}
