using System.Collections;
using UnityEngine;

// Start scene entry point.
// Runs SDK checks, initializes Addressables, then moves to the Title scene.
public sealed class StartManager : MonoBehaviour
{
    [SerializeField] private string _titleSceneAddress = SceneAddressKeys.Title;
    [SerializeField] private bool _verboseLog = true;
    [SerializeField] private StartLoadingView _loadingView;
    [SerializeField, Range(0.6f, 0.99f)] private float _loadingCapBeforeReady = 0.9f;
    [SerializeField] private float _loadingCapDuration = 1.6f;
    [SerializeField] private float _loadingCompleteDuration = 0.2f;
    [SerializeField] private float _loadingCompleteDelay = 0.05f;

    private AddressablesStartupLoader _addressablesLoader;
    private SdkInitializationPipeline _sdkPipeline;
    private bool _startupReady;

    private void Awake()
    {
        _addressablesLoader = new AddressablesStartupLoader(_verboseLog);
        _sdkPipeline = new SdkInitializationPipeline(
            _verboseLog,
            new ISdkInitializationStep[]
            {
                new NoOpSdkInitializationStep("Placeholder SDK Validation")
            });

        if (_loadingView == null)
        {
            StartLoadingView[] views = FindObjectsByType<StartLoadingView>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (views.Length > 0)
                _loadingView = views[0];
        }
    }

    private void Start()
    {
        StartCoroutine(BootstrapRoutine());
    }

    private IEnumerator BootstrapRoutine()
    {
        _startupReady = false;
        _loadingView?.SetProgress(0f);

        StartCoroutine(RunStartupRoutine());

        float progress = 0f;
        float capSpeed = _loadingCapBeforeReady / Mathf.Max(0.01f, _loadingCapDuration);
        while (!_startupReady)
        {
            progress = Mathf.MoveTowards(progress, _loadingCapBeforeReady, capSpeed * Time.deltaTime);
            _loadingView?.SetProgress(progress);
            yield return null;
        }

        float completeSpeed = (1f - progress) / Mathf.Max(0.01f, _loadingCompleteDuration);
        while (progress < 1f)
        {
            progress = Mathf.MoveTowards(progress, 1f, completeSpeed * Time.deltaTime);
            _loadingView?.SetProgress(progress);
            yield return null;
        }

        if (_loadingCompleteDelay > 0f)
            yield return new WaitForSeconds(_loadingCompleteDelay);

        yield return _addressablesLoader.LoadScene(_titleSceneAddress);
    }

    private IEnumerator RunStartupRoutine()
    {
        yield return _sdkPipeline.Run();
        yield return _addressablesLoader.InitializeAddressables();
        _startupReady = true;
    }
}
