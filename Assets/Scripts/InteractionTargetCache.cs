using UnityEngine;

public sealed class InteractionTargetCache
{
    public Collider Collider { get; private set; }
    public IInteractable Interactable { get; private set; }
    public IScannable Scannable { get; private set; }
    public int ResolutionCount { get; private set; }

    public bool Refresh(Collider detectedCollider)
    {
        if (Collider == detectedCollider)
            return false;

        Collider = detectedCollider;
        Interactable = null;
        Scannable = null;

        if (detectedCollider != null)
        {
            Interactable = detectedCollider.GetComponentInParent<IInteractable>();
            Scannable = detectedCollider.GetComponentInParent<IScannable>();
            ResolutionCount++;
        }

        return true;
    }
}
