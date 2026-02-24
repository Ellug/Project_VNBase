using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public sealed class SettingsAudioController : MonoBehaviour
{
    private const string MasterVolumeParameter = "MasterVolume";
    private const string BgmVolumeParameter = "BgmVolume";
    private const string SfxVolumeParameter = "SfxVolume";
    private const float MinLinear = 0.0001f;

    [SerializeField] private AudioMixer _mixer;
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;

    private bool _suppressEvents;

    void OnEnable()
    {
        BindSlider(_masterSlider, OnMasterChanged);
        BindSlider(_bgmSlider, OnBgmChanged);
        BindSlider(_sfxSlider, OnSfxChanged);
        LoadAndApply();
    }

    void OnDisable()
    {
        UnbindSlider(_masterSlider, OnMasterChanged);
        UnbindSlider(_bgmSlider, OnBgmChanged);
        UnbindSlider(_sfxSlider, OnSfxChanged);
    }

    private void BindSlider(Slider slider, UnityEngine.Events.UnityAction<float> callback)
    {
        if (slider == null)
            return;

        slider.onValueChanged.RemoveListener(callback);
        slider.onValueChanged.AddListener(callback);
    }

    private void UnbindSlider(Slider slider, UnityEngine.Events.UnityAction<float> callback)
    {
        if (slider == null)
            return;

        slider.onValueChanged.RemoveListener(callback);
    }

    private void LoadAndApply()
    {
        _suppressEvents = true;

        float master = PlayerPrefs.GetFloat(SettingsPreferenceKeys.MasterVolume, 1f);
        float bgm = PlayerPrefs.GetFloat(SettingsPreferenceKeys.BgmVolume, 1f);
        float sfx = PlayerPrefs.GetFloat(SettingsPreferenceKeys.SfxVolume, 1f);

        if (_masterSlider != null)
            _masterSlider.value = master;

        if (_bgmSlider != null)
            _bgmSlider.value = bgm;

        if (_sfxSlider != null)
            _sfxSlider.value = sfx;

        ApplyVolume(MasterVolumeParameter, master);
        ApplyVolume(BgmVolumeParameter, bgm);
        ApplyVolume(SfxVolumeParameter, sfx);

        _suppressEvents = false;
    }

    private void OnMasterChanged(float value)
    {
        if (_suppressEvents)
            return;

        ApplyVolume(MasterVolumeParameter, value);
        PlayerPrefs.SetFloat(SettingsPreferenceKeys.MasterVolume, value);
        PlayerPrefs.Save();
    }

    private void OnBgmChanged(float value)
    {
        if (_suppressEvents)
            return;

        ApplyVolume(BgmVolumeParameter, value);
        PlayerPrefs.SetFloat(SettingsPreferenceKeys.BgmVolume, value);
        PlayerPrefs.Save();
    }

    private void OnSfxChanged(float value)
    {
        if (_suppressEvents)
            return;

        ApplyVolume(SfxVolumeParameter, value);
        PlayerPrefs.SetFloat(SettingsPreferenceKeys.SfxVolume, value);
        PlayerPrefs.Save();
    }

    private void ApplyVolume(string parameterName, float linearValue)
    {
        if (_mixer == null)
            return;

        float clamped = Mathf.Clamp(linearValue, 0f, 1f);
        float db = Mathf.Log10(Mathf.Max(clamped, MinLinear)) * 20f;
        _mixer.SetFloat(parameterName, db);
    }
}
