using UnityEngine;

public class S_SoundFXManager : MonoBehaviour
{
    public static S_SoundFXManager instance;
    [SerializeField] private AudioSource soundFXObject;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume) //& Fait jouer un sound effect
    {
        // Fait apparaitre le gameobject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength); // Detruit l'audioSource après la fin du son
    }
    
    public void PlayRandomSoundFXClip(AudioClip[] audioClips, Transform spawnTransform, float volume) //& Fait jouer un sound effect aléatoire
    {
        int randomNumber = Random.Range(0, audioClips.Length); // AudioClip aléatoire dans la liste

        // Fait apparaitre le gameobject
        AudioSource audioSource = Instantiate(soundFXObject, spawnTransform.position, Quaternion.identity);

        audioSource.clip = audioClips[randomNumber];
        audioSource.volume = volume;
        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength); // Detruit l'audioSource après la fin du son
    }
}
