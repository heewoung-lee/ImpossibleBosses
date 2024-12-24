

using UnityEngine;

public interface IInteraction
{
    public bool CanInteraction { get; }

    public string InteractionName { get; }
    public Color InteractionNameColor { get; }

    public void Interaction();

    public void OutInteraction();
}