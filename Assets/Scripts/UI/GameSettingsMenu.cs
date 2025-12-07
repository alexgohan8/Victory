using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Simulation.Enums.Localization;

public class GameSettingsMenu : MonoBehaviour
{
    [SerializeField] private Slider sliderBgm;
    [SerializeField] private Slider sliderSfx;
    [SerializeField] private Toggle toggleLocalizationStyle;

    private void Start()
    {
        sliderBgm.value = SettingsManager.Instance.CurrentSettings.BgmVolume;
        sliderSfx.value = SettingsManager.Instance.CurrentSettings.SfxVolume;
        toggleLocalizationStyle.isOn = SettingsManager.Instance.CurrentSettings.CurrentLocalizationStyle == LocalizationStyle.Romanized;
    }

    public void OnSliderBgmValueChanged()
    {
        float volume = sliderBgm.value;
        SettingsManager.Instance.SetBgmVolume(volume);
    }

    public void OnSliderSfxValueChanged()
    {
        float volume = sliderSfx.value;
        SettingsManager.Instance.SetSfxVolume(volume);
    }

    public void OnLocalizationToggleChanged()
    {
        LocalizationStyle style = toggleLocalizationStyle.isOn
            ? LocalizationStyle.Romanized
            : LocalizationStyle.Localized;

        SettingsManager.Instance.SetLocalizationStyle(style);
    }
}
