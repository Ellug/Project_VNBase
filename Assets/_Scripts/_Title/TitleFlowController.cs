using System.Collections;
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
    [SerializeField] private bool _verboseLog;

    private bool _isInputArmed;
    private bool _menuOpened;

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
    }

    private void OpenMainMenu()
    {
        _menuOpened = true;

        if (_pressAnyKeyRoot != null)
            _pressAnyKeyRoot.SetActive(false);

        if (_mainMenuRoot != null)
            _mainMenuRoot.SetActive(true);

        if (_verboseLog)
            Debug.Log("[Title] Main menu opened.");
    }

    private static bool IsAnyInputPressed()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            return true;

        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame ||
                Mouse.current.middleButton.wasPressedThisFrame)
            {
                return true;
            }
        }

        if (Gamepad.current != null)
        {
            foreach (InputControl control in Gamepad.current.allControls)
            {
                if (control is ButtonControl button && button.wasPressedThisFrame)
                    return true;
            }
        }

        return false;
    }
}
