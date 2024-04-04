using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
   [SerializeField] private AudioMixer myMixer;
   [SerializeField] private Slider musicSilider;
   [SerializeField] private Slider SFXSilider;

   private void Start()
   {
    if(PlayerPrefs.HasKey("musicVolume"))
    {
        LoadVolume();
    }
    else{
        SetMusicVolume();
        SetSFXVolume();
    }
    
   }
   
   public void SetMusicVolume()
   {
    float volume = musicSilider.value;
    myMixer.SetFloat("music", Mathf.Log10(volume)*20);
    PlayerPrefs.SetFloat("musicVolume", volume);
   }

   public void SetSFXVolume()
   {
    float volume = SFXSilider.value;
    myMixer.SetFloat("SFX", Mathf.Log10(volume)*20);
    PlayerPrefs.SetFloat("SFXVolume", volume);
   }

   private void LoadVolume()
   {
    musicSilider.value = PlayerPrefs.GetFloat("musicVolume");
    SFXSilider.value = PlayerPrefs.GetFloat("SFXVolume");

    SetMusicVolume();
    SetSFXVolume();
   }
    
}
