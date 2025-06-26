using System.Collections;
using GameManagers;
using GameManagers.Interface.Resources_Interface;
using GameManagers.Interface.ResourcesManager;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI.WorldSpace
{
    public class UIDamageText : UIBase
    {
        [Inject] IDestroyObject _destroyer;
        TMP_Text _damageText;
        public TMP_Text DamageText { get => _damageText; set => _damageText = value; }
        Color _originalColor;
        Vector3 _originalTransform;
        enum DamegeText
        {
            DamageText
        }

        public void OnEnable()
        {
            if(_damageText == null)
            {
                Bind<TMP_Text>(typeof(DamegeText));
                _damageText = GetText((int)DamegeText.DamageText);
                _originalColor = _damageText.color;
                _originalTransform = transform.position;
            }
            StartCoroutine(DisplayDamage());
        }

        public void OnDisable()
        {
            _damageText.color = _originalColor;
            transform.position = _originalTransform;
        }

        IEnumerator DisplayDamage()
        {
            Color color = _damageText.color;
            while (true)
            {
                transform.position += Vector3.up *0.01f;
                color.a -= 0.01f;
                _damageText.color = color;
                if(color.a <= 0)
                {
                    _destroyer.DestroyObject(gameObject);
                    break;
                }
                yield return null;
            }
        }

        public void SetTextAndPosition(Transform parantTransform,int damage)
        {
            _damageText.text = damage.ToString();
            Vector3 damageTextPos = default;

            if (parantTransform.TryGetComponent(out Collider collider))
            {
                damageTextPos = parantTransform.position + Vector3.up * collider.bounds.max.y;
            }
            else
            {
                damageTextPos = parantTransform.position + Vector3.up * 1.5f;
            }
            transform.position = damageTextPos;
        }

        public void LateUpdate()
        {
            transform.rotation = Camera.main.transform.rotation;
        }

        protected override void StartInit()
        {
        }

        protected override void AwakeInit()
        {
        }
    }
}
