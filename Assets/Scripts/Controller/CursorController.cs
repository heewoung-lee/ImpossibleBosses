using UnityEngine;

namespace Controller
{
    /// <summary>
    /// 6.10일 해당 컨트롤러 삭제
    /// 이유: 너무 로직이 구식이라 나중에 추가할땐 인터페이스별로 구현된 타입을 노출시킬것
    /// </summary>
    public class CursorController : MonoBehaviour
    {
        
        [SerializeField] Texture2D _BaseCursor;
        [SerializeField] Texture2D _AttackCursor;
        [SerializeField] Texture2D _TalkCursor;
        Define.CurrentMouseType _currentMouseType = Define.CurrentMouseType.None;



        int _mask = 1 << (int)Define.Layer.Npc | 1 << (int)Define.Layer.Monster | 1 << (int)Define.Layer.Npc;

        Camera _camera;
        void Start()
        {
            _BaseCursor = Managers.ResourceManager.Load<Texture2D>("Prefabs/Textures/Cursors/Base");
            _AttackCursor = Managers.ResourceManager.Load<Texture2D>("Prefabs/Textures/Cursors/Attack");
            _TalkCursor = Managers.ResourceManager.Load<Texture2D>("Prefabs/Textures/Cursors/Talk");
            _camera = Camera.main;
        }


        void Update()
        {
            CursorEvent();
        }


        private void CursorEvent()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, _mask))
            {
                switch (hit.collider.gameObject.layer)
                {
                    case (int)Define.Layer.Monster:
                        if (_currentMouseType != Define.CurrentMouseType.Attack)
                        {
                            Cursor.SetCursor(_AttackCursor, new Vector2(_AttackCursor.width / 4, 0), CursorMode.Auto);
                            _currentMouseType = Define.CurrentMouseType.Attack;
                        }
                        break;
                    case (int)Define.Layer.Npc:
                        if (_currentMouseType != Define.CurrentMouseType.Talk)
                        {
                            Cursor.SetCursor(_TalkCursor, new Vector2(_TalkCursor.width / 4, 0), CursorMode.Auto);
                            _currentMouseType = Define.CurrentMouseType.Talk;
                        }
                        break;
                }
            }
            else
            {
                if(_currentMouseType != Define.CurrentMouseType.Base)
                {
                    Cursor.SetCursor(_BaseCursor, new Vector2(_BaseCursor.width / 3, _BaseCursor.height / 3), CursorMode.Auto);
                    _currentMouseType = Define.CurrentMouseType.Base;
                }
            }

        }
    }
}
