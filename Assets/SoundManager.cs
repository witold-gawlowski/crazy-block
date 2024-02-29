using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip clickClip;
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private AudioClip successSound;
    [SerializeField] private AudioClip failSound;
    [SerializeField] private AudioClip switchSound;


    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    public static SoundManager Instance { get; private set; }
    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance != null && Instance == this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            Instance = this;
        }
    }

    public void PlayMusic(bool value)
    {
        musicSource.clip = musicClip;
        if (value)
        {
            musicSource.Play();
        }
        else
        {
            musicSource.Stop();
        }
    }

    public void PlayClick()
    {
        sfxSource.clip = clickClip;
        sfxSource.Play();
    }

    public void PlaySuccess()
    {
        sfxSource.clip = successSound;
        sfxSource.Play();
    }
    public void PlayFail()
    {
        sfxSource.clip = failSound;
        sfxSource.Play();
    }

    public void PlaySwithc()
    {
        sfxSource.clip = switchSound;
        sfxSource.Play(); 
    }
}
