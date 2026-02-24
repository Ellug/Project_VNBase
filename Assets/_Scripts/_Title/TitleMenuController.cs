using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

// Main menu actions for the title screen.
public sealed class TitleMenuController : MonoBehaviour
{
    [SerializeField] private string _newGameSceneName = "VN";
    [SerializeField] private CanvasGroup _titleCanvasGroup;
    [SerializeField] private bool _playFadeInOnStart = true;
    [SerializeField] private float _sceneFadeInDuration = 0.35f;
    [SerializeField] private float _sceneFadeOutDuration = 0.3f;

    private bool _isLoadingScene;
    private Tween _sceneFadeTween;

    void Awake()
    {
        if (_titleCanvasGroup == null)
            return;

        _titleCanvasGroup.alpha = _playFadeInOnStart ? 0f : 1f;
        _titleCanvasGroup.interactable = !_playFadeInOnStart;
        _titleCanvasGroup.blocksRaycasts = !_playFadeInOnStart;
    }

    void Start()
    {
        if (_titleCanvasGroup == null || !_playFadeInOnStart)
            return;

        if (_sceneFadeTween != null && _sceneFadeTween.IsActive())
            _sceneFadeTween.Kill();

        _sceneFadeTween = _titleCanvasGroup
            .DOFade(1f, _sceneFadeInDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _titleCanvasGroup.interactable = true;
                _titleCanvasGroup.blocksRaycasts = true;
                _sceneFadeTween = null;
            });
    }

    public void OnNewGameSelected()
    {
        if (_isLoadingScene)
            return;

        StartCoroutine(LoadNewGameRoutine());
    }

    public void OnLoadGameSelected()
    {
        Debug.Log("[TitleMenu] Load Game selected. Implementation pending.");
    }

    public void OnSettingsSelected()
    {
        if (GlobalSettingsUIManager.Instance == null)
        {
            Debug.LogError("[Settings] GlobalSettingsUIManager instance is missing in scene.");
            return;
        }

        GlobalSettingsUIManager.Instance.OpenPanel();
    }

    public void OnGallerySelected()
    {
        Debug.Log("[TitleMenu] Galley selected. Implementation pending.");
    }

    public void OnExitSelected()
    {
        Debug.Log("[TitleMenu] Exit selected.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private IEnumerator LoadNewGameRoutine()
    {
        _isLoadingScene = true;

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_newGameSceneName, LoadSceneMode.Single);
        loadOperation.allowSceneActivation = false;

        _titleCanvasGroup.interactable = false;
        _titleCanvasGroup.blocksRaycasts = true;

        if (_sceneFadeTween != null && _sceneFadeTween.IsActive())
            _sceneFadeTween.Kill();

        _sceneFadeTween = _titleCanvasGroup
            .DOFade(0f, _sceneFadeOutDuration)
            .SetEase(Ease.OutQuad);

        while (loadOperation.progress < 0.9f || (_sceneFadeTween != null && _sceneFadeTween.IsActive() && _sceneFadeTween.IsPlaying()))
            yield return null;

        // Activates only after scene data is fully loaded for smoother transition.
        loadOperation.allowSceneActivation = true;

        while (!loadOperation.isDone)
            yield return null;

        _isLoadingScene = false;
    }

    void OnDestroy()
    {
        if (_sceneFadeTween != null && _sceneFadeTween.IsActive())
            _sceneFadeTween.Kill();
    }
}
