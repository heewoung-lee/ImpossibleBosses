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
        //튀어오르면서 로테이션을 돌린다.
        //바닥에 닿으면 VFX효과를 킨다.
        //아이템을 회전시킨다.
        //상호작용을 하면
        _rigidBody.AddForce(Vector3.up * 5f,ForceMode.Impulse);
        // 임의의 회전을 위한 랜덤 값 생성
        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),  // X축 회전
            Random.Range(-1f, 1f),  // Y축 회전
            Random.Range(-1f, 1f)   // Z축 회전
        );

        // 회전 힘 추가 (랜덤 값에 강도를 조절)
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