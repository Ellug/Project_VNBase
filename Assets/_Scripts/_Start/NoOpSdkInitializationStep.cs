using System.Collections;
using UnityEngine;

/// <summary>
/// Placeholder SDK check step for future integrations (Steam, platform SDKs, etc).
/// </summary>
public sealed class NoOpSdkInitializationStep : ISdkInitializationStep
{
    public string StepName { get; }

    public NoOpSdkInitializationStep(string stepName)
    {
        StepName = stepName;
    }

    public IEnumerator Execute()
    {
        Debug.Log($"[SDK] {StepName}: no implementation yet.");
        yield break;
    }
}
