using UnityEngine;

namespace Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BaseStats))]
    public abstract class MoveableController : BaseController
    {
        protected Vector3 _destPos;
        PlayerController _player;
        private void Update()
        {
            CurrentStateType.UpdateState();
        }
    }
}
