using System.Runtime.CompilerServices;
using UnityEngine;


public abstract class BaseController : MonoBehaviour
{
    public abstract Define.WorldObject WorldobjectType { get; protected set; }
    protected Define.State _state = Define.State.Idle;

    private const float DEFALUT_Transition_Idle = 0.3f;
    private const float DEFALUT_Transition_Move = 0.15f;
    private const float DEFALUT_Transition_Attack = 0.3f;
    private const float DEFALUT_Transition_Die = 0.3f;


    private Animator _anim;
    private float _transition_Idle = DEFALUT_Transition_Idle;
    private float _transition_Move = DEFALUT_Transition_Move;
    private float _transition_Attack = DEFALUT_Transition_Attack;
    private float _transition_Die = DEFALUT_Transition_Die;
    private int _animLayer = 0;

    public Animator Anim { get => _anim; protected set => _anim = value; }

    protected abstract int Hash_Idle { get; }
    protected abstract int Hash_Move { get; }
    protected abstract int Hash_Attack { get; }
    protected abstract int Hash_Die { get; }


    public int GetAttackHash => Hash_Attack;
    public int GetDieHash => Hash_Die;
    public int GetMoveHash => Hash_Move;
    public int GetIdleHash => Hash_Idle;


    public float Transition_Idle { get => _transition_Idle; protected set => _transition_Idle = value; }
    public float Transition_Move { get => _transition_Move; protected set => _transition_Move = value; }
    public float Transition_Attack { get => _transition_Attack; protected set => _transition_Attack = value; }
    public float Transition_Die { get => _transition_Die; protected set => _transition_Die = value; }

    public int AnimLayer { get => _animLayer; protected set => _animLayer = value; }


    public Define.State State
    {
        get => _state;
        protected set
        {
            _state = value;

            switch (_state)
            {
                case Define.State.Idle:
                    if (Hash_Idle == 0)
                        return;
                    _anim.CrossFade(Hash_Idle, Transition_Idle, AnimLayer, 0f);
                    break;
                case Define.State.Move:
                    if (Hash_Move == 0)
                        return;
                    _anim.CrossFade(Hash_Move, Transition_Move, AnimLayer, 0f);
                    break;
                case Define.State.Attack:
                    if (Hash_Attack == 0)
                        return;
                    _anim.CrossFade(Hash_Attack, Transition_Attack, AnimLayer, 0f);
                    break;
                case Define.State.Die:
                    if (Hash_Die == 0)
                        return;
                    _anim.CrossFade(Hash_Die, Transition_Die, AnimLayer, 0f);
                    break;
            }
        }
    }
    private void Awake()
    {
        _anim = GetComponent<Animator>();
        AwakeInit();
    }

    private void Start()
    {
        StartInit();
    }

    protected abstract void AwakeInit();
    protected abstract void StartInit();

    public void SetDefalutTransition_Value() 
    {
        _transition_Idle = DEFALUT_Transition_Idle;
        _transition_Move = DEFALUT_Transition_Move;
        _transition_Attack = DEFALUT_Transition_Attack;
        _transition_Die = DEFALUT_Transition_Die;
    }
}
