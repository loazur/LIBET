using FMODUnity;
using UnityEngine;

public class S_MusicManager : MonoBehaviour
{
    public static S_MusicManager instance;
    private StudioEventEmitter musicEmitter;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        musicEmitter = GetComponent<StudioEventEmitter>();
    }


    //!-------------------------------------

    public void SetMusicVolume(float volume)
    {
        musicEmitter.EventInstance.setVolume(volume);
    }

    public float GetMusicVolume()
    {
        musicEmitter.EventInstance.getVolume(out float volume);
        return volume;
    }


}
