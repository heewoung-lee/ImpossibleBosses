

using Module.PlayerModule;
using UnityEngine;

namespace Module.CommonModule
{
    public interface IInteraction
    {
        public bool CanInteraction { get; }

        public string InteractionName { get; }
        public Color InteractionNameColor { get; }

        public void Interaction(ModulePlayerInteraction caller);

        public void OutInteraction();
    }
}