using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class SdkInitializationPipeline
{
    private readonly IReadOnlyList<ISdkInitializationStep> _steps;
    private readonly bool _verboseLog;

    public SdkInitializationPipeline(bool verboseLog, IReadOnlyList<ISdkInitializationStep> steps)
    {
        _verboseLog = verboseLog;
        _steps = steps;
    }

    public IEnumerator Run()
    {
        if (_steps == null || _steps.Count == 0)
        {
            yield break;
        }

        for (int i = 0; i < _steps.Count; i++)
        {
            ISdkInitializationStep step = _steps[i];
            if (step == null)
            {
                continue;
            }

            if (_verboseLog)
            {
                Debug.Log($"[SDK] Running step {i + 1}/{_steps.Count}: {step.StepName}");
            }

            yield return step.Execute();
        }
    }
}
