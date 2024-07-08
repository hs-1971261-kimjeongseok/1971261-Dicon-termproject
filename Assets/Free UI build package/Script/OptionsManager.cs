using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle muteToggle;
    public Button closeButton;

    private float savedVolume;

    void Start()
    {
        volumeSlider.onValueChanged.AddListener(SetVolume);
        muteToggle.onValueChanged.AddListener(ToggleMute);
        closeButton.onClick.AddListener(CloseOptions);

        savedVolume = GlobalAudioManager.Instance.GetGlobalVolume();
        volumeSlider.value = 0.12f;
        muteToggle.isOn = savedVolume == 0;
    }

    public void SetVolume(float volume)
    {
        if (!muteToggle.isOn)
        {
            GlobalAudioManager.Instance.SetGlobalVolume(volume);
            savedVolume = volume;
        }
    }

    public void ToggleMute(bool isMuted)
    {
        if (isMuted)
        {
            GlobalAudioManager.Instance.SetGlobalVolume(0);
        }
        else
        {
            GlobalAudioManager.Instance.SetGlobalVolume(savedVolume);
        }
    }

    public void CloseOptions()
    {
        gameObject.SetActive(false); // This assumes the OptionsManager is managing the options panel visibility.
    }
}
