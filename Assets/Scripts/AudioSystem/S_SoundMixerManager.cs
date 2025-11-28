using UnityEngine;
using UnityEngine.Audio;

public class S_SoundMixerManager : MonoBehaviour
{
    public static S_SoundMixerManager instance;
    [Header("Gestion de l'audio mixer")]
    [Tooltip("Il s'agit de l'AudioMixer contenant les mixeur audios")]
    public AudioMixer audioMixer;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    //!--------------------------------------------------------

    public void SetMasterVolume(float volume) //& Change le volume général
    {
        audioMixer.SetFloat("masterVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetSoundFXVolume(float volume) //& Change le volume des effets sonores
    {
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(volume) * 20f);
    }

    public void SetMusicVolume(float volume) //& Change le volume de la musique
    {
        audioMixer.SetFloat("musicVolume", Mathf.Log10(volume) * 20f);
    }

}
