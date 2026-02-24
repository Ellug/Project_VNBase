using UnityEngine;
using UnityEngine.InputSystem;

public sealed class GlobalSettingsUIManager : Singleton<GlobalSettingsUIManager>
{
    [SerializeField] private SettingsPanelController _settingsPanelPrefab;
    [SerializeField] private bool _prewarmPanelOnAwake = true;

    private SettingsPanelController _settingsPanelInstance;

    protected override void OnSingletonAwake()
    {
        if (_prewarmPanelOnAwake)
            CreatePanelInstanceIfNeeded();
    }

    void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        TogglePanel();
    }

    public void TogglePanel()
    {
        CreatePanelInstanceIfNeeded();

        if (_settingsPanelInstance == null)
            return;

        if (_settingsPanelInstance.IsVisible)
            _settingsPanelInstance.Hide();
        else
            _settingsPanelInstance.Show();
    }

    public void OpenPanel()
    {
        CreatePanelInstanceIfNeeded();
        _settingsPanelInstance?.Show();
    }

    public void ClosePanel()
    {
        _settingsPanelInstance?.Hide();
    }

    private void CreatePanelInstanceIfNeeded()
    {
        if (_settingsPanelInstance != null)
            return;

        if (_settingsPanelPrefab == null)
        {
            Debug.LogError("[Settings] GlobalSettingsUIManager has no SettingsPanel prefab assigned.");
            return;
        }

        _settingsPanelInstance = Instantiate(_settingsPanelPrefab, transform);
        _settingsPanelInstance.HideImmediate();
    }
}
