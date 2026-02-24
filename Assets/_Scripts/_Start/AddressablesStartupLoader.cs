using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public sealed class AddressablesStartupLoader
{
    private readonly bool _verboseLog;

    public AddressablesStartupLoader(bool verboseLog)
    {
        _verboseLog = verboseLog;
    }

    public IEnumerator InitializeAddressables()
    {
        if (_verboseLog)
        {
            Debug.Log("[Addressables] Initializing...");
        }

        AsyncOperationHandle<IResourceLocator> initHandle = Addressables.InitializeAsync(false);
        yield return initHandle;

        if (initHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("[Addressables] InitializeAsync failed.");
        }

        Addressables.Release(initHandle);
    }

    public IEnumerator LoadScene(string sceneAddress)
    {
        if (_verboseLog)
        {
            Debug.Log($"[Addressables] Loading scene: {sceneAddress}");
        }

        AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> sceneLoadHandle =
            Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Single, true);
        yield return sceneLoadHandle;

        if (sceneLoadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[Addressables] Failed to load scene address: {sceneAddress}");
        }
    }
}
