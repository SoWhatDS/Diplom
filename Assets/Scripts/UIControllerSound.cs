using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerSound : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;
    public Sprite _muteMusic, _MuteSFX;
    public Sprite _MusicOn, _SFXOn;
    public Button _MusicBtn, _SFXBtn;

    public void ToggleMusic()
    {
        AudioManager.Instance.ToogleMusic();
    }

    public void ToggleSFX()
    {
        AudioManager.Instance.ToogleSFX();
    }

    public void MusicVolume()
    {
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    }

    public void SFXVolume()
    {
        AudioManager.Instance.SFXVolume(_sfxSlider.value);
    }

    public void ChangeImageOnMusicBtn()
    {
        if (AudioManager.Instance.musicSource.mute)
        {
            _MusicBtn.image.sprite = _muteMusic;
        }
        else
        {
            _MusicBtn.image.sprite = _MusicOn;
        }
    }

    public void ChangeImageOnSFXBtn()
    {
        if (AudioManager.Instance.sfxSource.mute)
        {
            _SFXBtn.image.sprite = _MuteSFX;
        }
        else
        {
            _SFXBtn.image.sprite = _SFXOn;
        }
    }

   
}
