using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public sealed class SettingsGraphicsController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown _displayModeDropdown;
    [SerializeField] private TMP_Dropdown _resolutionDropdown;

    private readonly List<FullScreenMode> _displayModes = new()
    {
        FullScreenMode.FullScreenWindow,
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.Windowed
    };

    private readonly List<ResolutionOption> _resolutionOptions = new();
    private bool _suppressEvents;

    void OnEnable()
    {
        if (_displayModeDropdown != null)
        {
            _displayModeDropdown.onValueChanged.RemoveListener(OnDisplayModeChanged);
            _displayModeDropdown.onValueChanged.AddListener(OnDisplayModeChanged);
        }

        if (_resolutionDropdown != null)
        {
            _resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
            _resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }

        RefreshOptionsAndApplySavedValues();
    }

    void OnDisable()
    {
        if (_displayModeDropdown != null)
            _displayModeDropdown.onValueChanged.RemoveListener(OnDisplayModeChanged);

        if (_resolutionDropdown != null)
            _resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
    }

    private void RefreshOptionsAndApplySavedValues()
    {
        _suppressEvents = true;
        BuildDisplayModeOptions();
        BuildResolutionOptions();
        ApplySavedGraphicsSettings();
        _suppressEvents = false;
    }

    private void BuildDisplayModeOptions()
    {
        if (_displayModeDropdown == null)
            return;

        _displayModeDropdown.ClearOptions();
        _displayModeDropdown.AddOptions(new List<string>
        {
            "Borderless Fullscreen",
            "Exclusive Fullscreen",
            "Windowed"
        });
    }

    private void BuildResolutionOptions()
    {
        if (_resolutionDropdown == null)
            return;

        _resolutionOptions.Clear();
        _resolutionDropdown.ClearOptions();

        // Screen.resolutions is tied to the display the game window/fullscreen is currently using.
        Resolution[] rawResolutions = Screen.resolutions;
        var dedup = new Dictionary<Vector2Int, ResolutionOption>();

        foreach (Resolution resolution in rawResolutions)
        {
            var key = new Vector2Int(resolution.width, resolution.height);
            if (!dedup.ContainsKey(key))
            {
                dedup.Add(key, new ResolutionOption(resolution.width, resolution.height));
            }
        }

        foreach (ResolutionOption option in dedup.Values)
            _resolutionOptions.Add(option);

        _resolutionOptions.Sort((a, b) =>
        {
            int widthCompare = a.Width.CompareTo(b.Width);
            if (widthCompare != 0)
                return widthCompare;

            return a.Height.CompareTo(b.Height);
        });

        var optionLabels = new List<string>(_resolutionOptions.Count);
        foreach (ResolutionOption option in _resolutionOptions)
            optionLabels.Add($"{option.Width} x {option.Height}");

        if (optionLabels.Count == 0)
        {
            _resolutionOptions.Add(new ResolutionOption(Screen.width, Screen.height));
            optionLabels.Add($"{Screen.width} x {Screen.height}");
        }

        _resolutionDropdown.AddOptions(optionLabels);
    }

    private void ApplySavedGraphicsSettings()
    {
        FullScreenMode savedMode = ReadSavedDisplayMode();
        ResolutionOption savedResolution = ReadSavedResolution();

        int modeIndex = Mathf.Clamp(_displayModes.IndexOf(savedMode), 0, _displayModes.Count - 1);
        if (_displayModeDropdown != null)
            _displayModeDropdown.value = modeIndex;

        int resolutionIndex = GetBestResolutionIndex(savedResolution.Width, savedResolution.Height);
        if (_resolutionDropdown != null)
            _resolutionDropdown.value = resolutionIndex;

        ApplyCurrentSelection();
    }

    private FullScreenMode ReadSavedDisplayMode()
    {
        int savedValue = PlayerPrefs.GetInt(SettingsPreferenceKeys.DisplayMode, (int)Screen.fullScreenMode);
        foreach (FullScreenMode mode in _displayModes)
        {
            if ((int)mode == savedValue)
                return mode;
        }

        return Screen.fullScreenMode;
    }

    private ResolutionOption ReadSavedResolution()
    {
        int width = PlayerPrefs.GetInt(SettingsPreferenceKeys.ResolutionWidth, Screen.width);
        int height = PlayerPrefs.GetInt(SettingsPreferenceKeys.ResolutionHeight, Screen.height);
        return new ResolutionOption(width, height);
    }

    private int GetBestResolutionIndex(int width, int height)
    {
        for (int i = 0; i < _resolutionOptions.Count; i++)
        {
            if (_resolutionOptions[i].Width == width && _resolutionOptions[i].Height == height)
                return i;
        }

        for (int i = 0; i < _resolutionOptions.Count; i++)
        {
            if (_resolutionOptions[i].Width == Screen.width && _resolutionOptions[i].Height == Screen.height)
                return i;
        }

        return Mathf.Clamp(_resolutionOptions.Count - 1, 0, _resolutionOptions.Count - 1);
    }

    private void OnDisplayModeChanged(int _)
    {
        if (_suppressEvents)
            return;

        ApplyCurrentSelection();
    }

    private void OnResolutionChanged(int _)
    {
        if (_suppressEvents)
            return;

        ApplyCurrentSelection();
    }

    private void ApplyCurrentSelection()
    {
        if (_displayModeDropdown == null || _resolutionDropdown == null || _resolutionOptions.Count == 0)
            return;

        FullScreenMode mode = _displayModes[Mathf.Clamp(_displayModeDropdown.value, 0, _displayModes.Count - 1)];
        ResolutionOption resolution = _resolutionOptions[Mathf.Clamp(_resolutionDropdown.value, 0, _resolutionOptions.Count - 1)];

        Screen.SetResolution(resolution.Width, resolution.Height, mode);

        PlayerPrefs.SetInt(SettingsPreferenceKeys.DisplayMode, (int)mode);
        PlayerPrefs.SetInt(SettingsPreferenceKeys.ResolutionWidth, resolution.Width);
        PlayerPrefs.SetInt(SettingsPreferenceKeys.ResolutionHeight, resolution.Height);
        PlayerPrefs.Save();
    }

    [Serializable]
    private struct ResolutionOption
    {
        public int Width;
        public int Height;

        public ResolutionOption(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
