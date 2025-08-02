using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenuUI : MonoBehaviour
{
    public static OptionsMenuUI Instance { get; private set; }

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundFXVolumeSlider;

    [SerializeField] private Button closeButton;

    private void Awake()
    {
        Instance = this;

        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void SetMasterVolume()
    {
        float masterVolume = masterVolumeSlider.value;
        audioMixer.SetFloat("masterVolume", Mathf.Log10(masterVolume) * 20);
    }
    public void SetMusicVolume()
    {
        float musicVolume = musicVolumeSlider.value;
        audioMixer.SetFloat("musicVolume", Mathf.Log10(musicVolume) * 20);
    }
    public void SetSoundFXVolume()
    {
        float soundFXVolume = soundFXVolumeSlider.value;
        audioMixer.SetFloat("soundFXVolume", Mathf.Log10(soundFXVolume) * 20);
    }
    

}
