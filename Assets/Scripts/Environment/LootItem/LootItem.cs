using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class LootItem : NetworkBehaviour,IInteraction
{
    private const float ADDFORCE_OFFSET = 5f;
    private const float TORQUE_FORCE_OFFSET = 30f;
    private const float DROPITEM_VERTICAL_OFFSET = 0.2f;
    private const float DROPITEM_ROTATION_OFFSET = 40f;
    private UI_Player_Inventory _ui_player_Inventory;

    Transform _dropper;
    Rigidbody _rigidBody;
    GameObject _itemEffect;
    CapsuleCollider _collider;
    IItem _iteminfo;

    public bool CanInteraction => _canInteraction;
    public string InteractionName => _iteminfo.ItemName;
    public Color InteractionNameColor => Utill.GetItemGradeColor(_iteminfo.Item_Grade);

    private bool _canInteraction = false;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
    }
    private void Start()
    {
       
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        _ui_player_Inventory = Managers.UI_Manager.GetImportant_Popup_UI<UI_Player_Inventory>();
        _canInteraction = false;
        transform.position = _dropper.transform.position + Vector3.up * _dropper.GetComponent<Collider>().bounds.max.y;
        //Ƣ������鼭 �����̼��� ������.
        //�ٴڿ� ������ VFXȿ���� Ų��.
        //�������� ȸ����Ų��.
        //��ȣ�ۿ��� �ϸ�
        _rigidBody.AddForce(Vector3.up * ADDFORCE_OFFSET, ForceMode.Impulse);
        // ������ ȸ���� ���� ���� �� ����
        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),  // X�� ȸ��
            Random.Range(-1f, 1f),  // Y�� ȸ��
            Random.Range(-1f, 1f)   // Z�� ȸ��
        );
        // ȸ�� �� �߰� (���� ���� ������ ����)
        _rigidBody.AddTorque(randomTorque * TORQUE_FORCE_OFFSET, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            _rigidBody.isKinematic = true;
            transform.position += Vector3.up * DROPITEM_VERTICAL_OFFSET;
            transform.rotation = Quaternion.identity;//�������� ���� ������ �ȹٷ� �����.
            StartCoroutine(RotationDropItem());
            CreateLootingItemEffect();
            _canInteraction = true;
        }
    }


    private void CreateLootingItemEffect()
    {
        _itemEffect = ItemGradeEffect(_iteminfo);
        _itemEffect.transform.position = transform.position;
        _itemEffect.transform.SetParent(transform);
    }


    public void SetDropperAndItem(Transform dropper,IItem iteminfo)
    {
        _dropper = dropper;
        _iteminfo = iteminfo;
    }

    public void SetIteminfo(IItem iteminfo)
    {
        _iteminfo = iteminfo;
    }

    IEnumerator RotationDropItem()
    {
        while (true)
        {
            transform.Rotate(new Vector3(0, Time.deltaTime * DROPITEM_ROTATION_OFFSET, 0));
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

    public void Interaction(Module_Player_Interaction caller)
    {
        PlayerPickup(caller);
    }

    public void PlayerPickup(Module_Player_Interaction player)
    {
        PlayerController base_controller = player.PlayerController;
        base_controller.CurrentStateType = base_controller.PickupState;//�Ⱦ� �ִϸ��̼� ����

        if (base_controller.CurrentStateType != base_controller.PickupState)
            return;

        UI_ItemComponent_Inventory inventory_item = (_iteminfo as IInventoryItemMaker).MakeItemComponentInventory();
        inventory_item.transform.SetParent(Managers.LootItemManager.GetItemComponentPosition(_ui_player_Inventory));
        Managers.ResourceManager.DestroyObject(gameObject);
        player.DisEnable_Icon_UI();//��ȣ�ۿ� ������ ����
    }

    public void OutInteraction()
    {

    }
}