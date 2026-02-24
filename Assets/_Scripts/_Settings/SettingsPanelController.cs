using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public sealed class SettingsPanelController : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Button _closeButton;
    [SerializeField] private float _fadeDuration = 0.2f;

    private Tween _fadeTween;

    public bool IsVisible { get; private set; }

    void Awake()
    {
        if (_canvasGroup == null)
            _canvasGroup = GetComponent<CanvasGroup>();

        if (_closeButton != null)
        {
            _closeButton.onClick.RemoveListener(Hide);
            _closeButton.onClick.AddListener(Hide);
        }

        HideImmediate();
    }

    void OnDestroy()
    {
        if (_closeButton != null)
            _closeButton.onClick.RemoveListener(Hide);

        KillFadeTween();
    }

    public void Show()
    {
        IsVisible = true;
        gameObject.SetActive(true);

        if (_canvasGroup == null)
            return;

        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;

        KillFadeTween();
        _fadeTween = _canvasGroup
            .DOFade(1f, _fadeDuration)
            .SetEase(Ease.OutQuad);
    }

    public void Hide()
    {
        if (!IsVisible)
            return;

        IsVisible = false;

        if (_canvasGroup == null)
        {
            gameObject.SetActive(false);
            return;
        }

        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;

        KillFadeTween();
        _fadeTween = _canvasGroup
            .DOFade(0f, _fadeDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                gameObject.SetActive(false);
                _fadeTween = null;
            });
    }

    public void HideImmediate()
    {
        IsVisible = false;
        KillFadeTween();

        if (_canvasGroup != null)
        {
            _canvasGroup.alpha = 0f;
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        gameObject.SetActive(false);
    }

    private void KillFadeTween()
    {
        if (_fadeTween != null && _fadeTween.IsActive())
            _fadeTween.Kill();
    }
}
