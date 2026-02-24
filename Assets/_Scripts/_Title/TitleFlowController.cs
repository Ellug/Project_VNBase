using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

// Controls title flow:
// 1) Show "Press Any Key"
// 2) Open menu after first input.
public sealed class TitleFlowController : MonoBehaviour
{
    [SerializeField] private GameObject _pressAnyKeyRoot;
    [SerializeField] private GameObject _mainMenuRoot;
    [SerializeField] private RectTransform _mainMenuRect;
    [SerializeField] private CanvasGroup _mainMenuCanvasGroup;
    [SerializeField] private float _menuAppearOffsetY = 30f;
    [SerializeField] private float _menuAppearMoveDuration = 0.4f;
    [SerializeField] private float _menuAppearFadeDuration = 0.35f;
    [SerializeField] private bool _verboseLog;

    private bool _isInputArmed;
    private bool _menuOpened;
    private Vector2 _mainMenuBaseAnchoredPosition;
    private Tween _mainMenuAppearTween;

    private void Awake()
    {
        BindMenuAnimationTargets();
    }

    private void Start()
    {
        SetWaitingForInputState();
        StartCoroutine(ArmInputNextFrame());
    }

    private IEnumerator ArmInputNextFrame()
    {
        yield return null;
        _isInputArmed = true;
    }

    void Update()
    {
        if (!_isInputArmed || _menuOpened)
            return;

        if (!IsAnyInputPressed())
            return;

        OpenMainMenu();
    }

    private void SetWaitingForInputState()
    {
        _menuOpened = false;

        if (_pressAnyKeyRoot != null)
            _pressAnyKeyRoot.SetActive(true);

        if (_mainMenuRoot != null)
            _mainMenuRoot.SetActive(false);

        if (_mainMenuAppearTween != null && _mainMenuAppearTween.IsActive())
            _mainMenuAppearTween.Kill();

        if (_mainMenuCanvasGroup != null)
        {
            _mainMenuCanvasGroup.alpha = 0f;
            _mainMenuCanvasGroup.interactable = false;
            _mainMenuCanvasGroup.blocksRaycasts = false;
        }

        if (_mainMenuRect != null)
            _mainMenuRect.anchoredPosition = _mainMenuBaseAnchoredPosition + Vector2.down * _menuAppearOffsetY;
    }

    private void OpenMainMenu()
    {
        _menuOpened = true;

        if (_pressAnyKeyRoot != null)
            _pressAnyKeyRoot.SetActive(false);

        if (_mainMenuRoot != null)
            _mainMenuRoot.SetActive(true);

        if (_mainMenuRect != null && _mainMenuCanvasGroup != null)
        {
            if (_mainMenuAppearTween != null && _mainMenuAppearTween.IsActive())
                _mainMenuAppearTween.Kill();

            _mainMenuRect.anchoredPosition = _mainMenuBaseAnchoredPosition + Vector2.down * _menuAppearOffsetY;
            _mainMenuCanvasGroup.alpha = 0f;
            _mainMenuCanvasGroup.interactable = false;
            _mainMenuCanvasGroup.blocksRaycasts = false;

            _mainMenuAppearTween = DOTween.Sequence()
                .Join(_mainMenuRect.DOAnchorPos(_mainMenuBaseAnchoredPosition, _menuAppearMoveDuration).SetEase(Ease.OutCubic))
                .Join(_mainMenuCanvasGroup.DOFade(1f, _menuAppearFadeDuration).SetEase(Ease.OutQuad))
                .OnComplete(() =>
                {
                    _mainMenuCanvasGroup.interactable = true;
                    _mainMenuCanvasGroup.blocksRaycasts = true;
                    _mainMenuAppearTween = null;
                });
        }

        if (_verboseLog)
            Debug.Log("[Title] Main menu opened.");
    }

    private void BindMenuAnimationTargets()
    {
        if (_mainMenuRoot == null)
            return;

        if (_mainMenuRect == null)
            _mainMenuRect = _mainMenuRoot.GetComponent<RectTransform>();

        if (_mainMenuCanvasGroup == null)
            _mainMenuCanvasGroup = _mainMenuRoot.GetComponent<CanvasGroup>();

        if (_mainMenuRect != null)
            _mainMenuBaseAnchoredPosition = _mainMenuRect.anchoredPosition;
    }

    private void OnDestroy()
    {
        if (_mainMenuAppearTween != null && _mainMenuAppearTween.IsActive())
            _mainMenuAppearTween.Kill();
    }

    private static bool IsAnyInputPressed()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            return true;

        if (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame)
            return true;

        return false;
    }
}
