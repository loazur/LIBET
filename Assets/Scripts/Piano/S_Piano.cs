using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

class S_Piano : MonoBehaviour, SI_Interactable
{
    [Header("Piano Keys Settings")]
    [SerializeField] private float timePressed = 0.2f;
    [SerializeField] private int numberOfKeysSimulated = 5;
    [SerializeField] private float playInterval = 0.5f;

    //* Gestion Audio
    [Header("Music")]
    [SerializeField] private SO_PianoTrack[] pianoTracks;
    private SO_PianoTrack currentTrack;
    private StudioEventEmitter pianoEmitter;
    private EventInstance pianoEventInstance;

    //TODO Remettre l'UI
    /*
    [Header("UI")]

    [Tooltip("UI affichée lors de la lecture de la musique")]
    [SerializeField]private GameObject musicUI;

    [Tooltip("Texte du nom de la piste")]
    [SerializeField]private Text trackNameText;

    [Tooltip("Texte de l'auteur de la piste")]
    [SerializeField]private Text trackAuthorText;
    */
    //*========================================================

    //! eviter de modifier cette valeur
    private float keyPressDepth = 0.022f; //! Important: doit correspondre au déplacement visuel des touches
    private List<GameObject> pianoKeys = new();
    private string interactText = "Jouer du piano";


    void Start()
    {
        //DisableUIMusic(); //& S'assurer que l'UI est désactivée au départ
        UpdateInteractText(); //& Met à jour le texte d'interaction en fonction de la langue

        pianoEmitter = S_AudioManager.instance.InitializeEventEmitter(S_FMODEvents.instance.piano, gameObject);
        pianoEmitter.Stop();

        foreach (Transform child in transform)
            if (child.name.StartsWith("touche"))
                pianoKeys.Add(child.gameObject);
    }

    private SO_PianoTrack GetRandomPianoTrack() //& Récupére une track aléatoire en fonction de la liste
    {
        int index = Random.Range(0, pianoTracks.Length);
        return pianoTracks[index];
    }

    public void SetPianoTrack(S_PianoTrack track) //& Change la track
    {
        pianoEmitter.EventInstance.setParameterByName("track", (float)track);
    }


    // ===================== INTERACTION =====================

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
            StopPlaying();
        else
            StartPlaying();
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
        SetPianoTrack(GetRandomPianoTrack().track);

        //playRoutine = StartCoroutine(PlayLoop());
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

        /*
        if (playRoutine != null)
            StopCoroutine(playRoutine);

        StopPianoMusic();
        musicStarted = false;
        */
    }

    // ===================== PLAY LOOP =====================

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
        
        //while (isPlaying)
        {
            MoveAllTouches();
            yield return new WaitForSeconds(playInterval);
        }
    }

    // ===================== TOUCHES =====================

    /**
     * Simule l'appui sur plusieurs touches du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Tuesday, January 6th, 2026.
     * @access	private
     * @return	void
     */
    private void MoveAllTouches() //TODO Remettre avec le nouveau système
    {
        /*
        if (!musicStarted)
        {
            StartPianoMusic();
            musicStarted = true;
        }
        */

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

    #region MUSIC


    /**
     * obtenir une piste aléatoire
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 7th, 2026.
     * @access	private
     * @return	mixed
     */
    

    #endregion MUSIC


    /*
    // ===================== UI =====================
    #region UI

   
    private void UpdateMusicUI()
    {
        if (musicUI != null)
            musicUI.SetActive(true);

        if (trackNameText != null)
            trackNameText.text = currentTrack.trackName;

        if (trackAuthorText != null)
            trackAuthorText.text = currentTrack.author;
    }

   
    public void DisableUIMusic()
    {
        if (musicUI != null)
            musicUI.SetActive(false);
    }

   
    public void EnableUIMusic()
    {
        if (musicUI != null)
            musicUI.SetActive(true);
    }
    */

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
     * Obtient la position du piano
     *
     * @author	Lucas
     * @since	v0.0.1
     * @version	v1.0.0	Wednesday, January 7th, 2026.
     * @access	public
     * @return	void
     */
    public Transform getTransform() => transform;

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
    #region UI
    #endregion UI
}
