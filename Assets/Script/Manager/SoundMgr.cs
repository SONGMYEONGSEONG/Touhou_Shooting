using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SoundMgr : MonoBehaviour
{
    /// <summary>
    /// ΩÃ±€≈Ê ±∏¡∂  
    static SoundMgr instance = null;
    public static SoundMgr Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<SoundMgr>();
                instance.Initialize();

                DontDestroyOnLoad(instance.gameObject);
            }

            return instance;
        }
    }
    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(gameObject);
        }

        if (!BGMPlayer) { BGMPlayer = GetComponent<AudioSource>(); }
        if (!SFXPlayer) { SFXPlayer = GetComponent<AudioSource>(); }
    }
    ///

    [SerializeField] AudioSource BGMPlayer = null;
    [SerializeField] AudioSource SFXPlayer = null;
    [SerializeField] AudioClip[] audioClips = null;

    [SerializeField][Range(0.0f, 1.0f)] float BGMvolume = 1.0f;
    [SerializeField][Range(0.0f, 1.0f)] float SFXvolume = 1.0f;

    public float BGMVolume { get { return BGMPlayer.volume; }  set { BGMPlayer.volume = value; } }
    public float SFXVolume { get { return SFXPlayer.volume; } set { SFXPlayer.volume = value; } }

    public void Initialize()
    {
        BGMPlayer.volume = BGMvolume;
        SFXPlayer.volume = SFXvolume;
        return;
    }

    AudioClip FindClip(string name)
    {
        foreach (AudioClip clip in audioClips)
        {
            if (clip.name.ToLower().Equals(name.ToLower()))
            {
                return clip;
            }
        }
        return null;
    }

    public void PlayBGM(string clipName)
    {
        BGMPlayer.clip = FindClip(clipName);
        BGMPlayer.Play();
    }

    public void StopBGM() { BGMPlayer.Stop(); }
    public void PlaySFX(string clipName)
    {
        AudioClip clip = FindClip(clipName);
        if (clip) SFXPlayer.PlayOneShot(clip);
    }

}
