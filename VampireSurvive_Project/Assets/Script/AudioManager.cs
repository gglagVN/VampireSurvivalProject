using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicAudioSource;
    public AudioSource vfxAudioSource;

    public AudioClip musicClip;

    public Slider musicSlider;   // slider chỉnh âm nhạc
    public Slider vfxSlider;     // slider chỉnh hiệu ứng

    void Start()
    {
        // Phát nhạc
        musicAudioSource.clip = musicClip;
        musicAudioSource.loop = true;
        musicAudioSource.Play();

        // Load volume đã lưu
        float musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

        musicAudioSource.volume = musicVol;
        vfxAudioSource.volume = sfxVol;

        musicSlider.value = musicVol;
        vfxSlider.value = sfxVol;

        // Gán sự kiện thay đổi
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        vfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float vol)
    {
        musicAudioSource.volume = vol;
        PlayerPrefs.SetFloat("MusicVolume", vol);
    }

    public void SetSFXVolume(float vol)
    {
        vfxAudioSource.volume = vol;
        PlayerPrefs.SetFloat("SFXVolume", vol);
    }
}
