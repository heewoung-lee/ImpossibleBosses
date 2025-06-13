using Player;
using Stats.BaseStats;
using UnityEngine;

namespace Controller
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(BaseStats))]
    public abstract class MoveableController : BaseController
    {
        protected Vector3 _destPos;
        private PlayerController _player;
        private void Update()
        {
            CurrentStateType.UpdateState();
        }
    }
}
