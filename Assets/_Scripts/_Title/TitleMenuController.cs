using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

// Main menu actions for the title screen.
public sealed class TitleMenuController : MonoBehaviour
{
    [SerializeField] private string _newGameSceneAddress = SceneAddressKeys.Game;

    private bool _isLoadingScene;

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
        Debug.Log("[TitleMenu] Settings selected. Implementation pending.");
    }

    public void OnConfigSelected()
    {
        Debug.Log("[TitleMenu] Config selected. Implementation pending.");
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

        AsyncOperationHandle<UnityEngine.ResourceManagement.ResourceProviders.SceneInstance> loadHandle =
            Addressables.LoadSceneAsync(_newGameSceneAddress, LoadSceneMode.Single, true);
        yield return loadHandle;

        if (loadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[TitleMenu] Failed to load New Game scene: {_newGameSceneAddress}");
        }

        _isLoadingScene = false;
    }
}
