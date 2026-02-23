using System.Collections;

public interface ISdkInitializationStep
{
    string StepName { get; }
    IEnumerator Execute();
}
