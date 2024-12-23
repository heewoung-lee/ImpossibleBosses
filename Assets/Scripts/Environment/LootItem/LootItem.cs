using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootItem : MonoBehaviour,IInteraction
{

    Transform _dropper;
    Rigidbody _rigidBody;
    GameObject _itemEffect;
    CapsuleCollider _collider;

    IItem _iteminfo;



    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }
    private void Start()
    {
        transform.position = _dropper.transform.position + Vector3.up*_dropper.GetComponent<Collider>().bounds.max.y;
        //Ƣ������鼭 �����̼��� ������.
        //�ٴڿ� ������ VFXȿ���� Ų��.
        //�������� ȸ����Ų��.
        //��ȣ�ۿ��� �ϸ�
        _rigidBody.AddForce(Vector3.up * 5f,ForceMode.Impulse);
        // ������ ȸ���� ���� ���� �� ����
        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),  // X�� ȸ��
            Random.Range(-1f, 1f),  // Y�� ȸ��
            Random.Range(-1f, 1f)   // Z�� ȸ��
        );

        // ȸ�� �� �߰� (���� ���� ������ ����)
        _rigidBody.AddTorque(randomTorque*0.3f, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _rigidBody.isKinematic = true;
            _rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            transform.position += Vector3.up*0.2f;
            transform.rotation = Quaternion.identity;
            StartCoroutine(RotationDropItem());
            CreateLootingItemEffect();
        }
    }


    private void CreateLootingItemEffect()
    {
        _itemEffect = ItemGradeEffect(_iteminfo);
        _itemEffect.transform.position = transform.position;
    }


    public void SetDropperAndItem(Transform dropper,IItem iteminfo)
    {
        _dropper = dropper;
        _iteminfo = iteminfo;
    }


    IEnumerator RotationDropItem()
    {
        while (true)
        {
            transform.Rotate(new Vector3(0, Time.deltaTime * 40f, 0));
            yield return null;
        }
    }

    private GameObject ItemGradeEffect(IItem iteminfo)
    {
        switch (iteminfo.Item_Grade)
        {
            case Item_Grade_Type.Normal:
                return Managers.VFX_Manager.GenerateParticle("Paticle/LootingItemEffect/Lootbeams_Runic_Common");
            case Item_Grade_Type.Magic:
                return Managers.VFX_Manager.GenerateParticle("Paticle/LootingItemEffect/Lootbeams_Runic_Uncommon");
            case Item_Grade_Type.Rare:
                return Managers.VFX_Manager.GenerateParticle("Paticle/LootingItemEffect/Lootbeams_Runic_Rare");
            case Item_Grade_Type.Unique:
                return Managers.VFX_Manager.GenerateParticle("Paticle/LootingItemEffect/Lootbeams_Runic_Epic");
            case Item_Grade_Type.Epic:
                return Managers.VFX_Manager.GenerateParticle("Paticle/LootingItemEffect/Lootbeams_Runic_Legendary");
        }
        return Managers.VFX_Manager.GenerateParticle("Paticle/LootingItemEffect/Lootbeams_Runic_Common");
    }

    public void Interaction()
    {
        
    }

    public void OutInteraction()
    {
    }
}