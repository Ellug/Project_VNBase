using UnityEngine;
using UnityEngine.UI;

public sealed class StartLoadingView : MonoBehaviour
{
    [SerializeField] private Image _fillImage;

    private void Awake()
    {
        SetProgress(0f);
    }

    public void SetProgress(float normalizedProgress)
    {
        if (_fillImage == null)
            return;

        _fillImage.fillAmount = Mathf.Clamp01(normalizedProgress);
    }
}
