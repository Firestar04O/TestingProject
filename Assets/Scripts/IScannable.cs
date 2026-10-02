using UnityEngine;
public interface IScannable : IInteractable
{
    string GetScanPrompt();
    void OnScanStarted();
    void OnScanCancelled();
    void OnScanCompleted();
    bool IsSafe();
}