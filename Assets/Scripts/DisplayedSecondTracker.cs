using UnityEngine;

public sealed class DisplayedSecondTracker
{
    private int lastDisplayedSecond = int.MinValue;

    public bool TryGetChangedSecond(float currentTime, out int displayedSecond)
    {
        displayedSecond = Mathf.CeilToInt(Mathf.Max(0f, currentTime));

        if (displayedSecond == lastDisplayedSecond)
            return false;

        lastDisplayedSecond = displayedSecond;
        return true;
    }

    public void Reset()
    {
        lastDisplayedSecond = int.MinValue;
    }
}
