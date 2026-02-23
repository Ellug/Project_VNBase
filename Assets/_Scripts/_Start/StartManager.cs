using System.Collections;
using UnityEngine;

// Start scene entry point.
// Runs SDK checks, initializes Addressables, then moves to the Title scene.
public sealed class StartManager : MonoBehaviour
{
    [SerializeField] private string _titleSceneAddress = SceneAddressKeys.Title;
    [SerializeField] private bool _verboseLog = true;

    private AddressablesStartupLoader _addressablesLoader;
    private SdkInitializationPipeline _sdkPipeline;

    private void Awake()
    {
        _addressablesLoader = new AddressablesStartupLoader(_verboseLog);
        _sdkPipeline = new SdkInitializationPipeline(
            _verboseLog,
            new ISdkInitializationStep[]
            {
                new NoOpSdkInitializationStep("Placeholder SDK Validation")
            });
    }

    private void Start()
    {
        StartCoroutine(BootstrapRoutine());
    }

    private IEnumerator BootstrapRoutine()
    {
        yield return _sdkPipeline.Run();
        yield return _addressablesLoader.InitializeAddressables();
        yield return _addressablesLoader.LoadScene(_titleSceneAddress);
    }
}
