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
    private NetworkObject _networkObject;

    [SerializeField] private Vector3 _dropPosition;
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private GameObject _itemEffect;
    [SerializeField] private CapsuleCollider _collider;
    [SerializeField] private IItem _iteminfo;


    public bool CanInteraction => _canInteraction;
    public string InteractionName => _iteminfo.ItemName;
    public Color InteractionNameColor => Utill.GetItemGradeColor(_iteminfo.Item_Grade);

    private bool _canInteraction = false;


    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<CapsuleCollider>();
        _networkObject = GetComponent<NetworkObject>();
    }

    public void SpawnBahaviour()
    {
        _ui_player_Inventory = Managers.UI_Manager.GetImportant_Popup_UI<UI_Player_Inventory>();
        _canInteraction = false;

        if (TryGetComponent(out LootItemBehaviour behaviour) == true)
        {
            behaviour.SpawnBahaviour(_rigidBody);
            return;
        }
        //튀어오르면서 로테이션을 돌린다.
        //바닥에 닿으면 VFX효과를 킨다.
        //아이템을 회전시킨다.
        //상호작용을 하면
        transform.position = _dropPosition + Vector3.up * 1.2f;
        _rigidBody.AddForce(Vector3.up * ADDFORCE_OFFSET, ForceMode.Impulse);
        // 임의의 회전을 위한 랜덤 값 생성
        Vector3 randomTorque = new Vector3(
            Random.Range(-1f, 1f),  // X축 회전
            Random.Range(-1f, 1f),  // Y축 회전
            Random.Range(-1f, 1f)   // Z축 회전
        );
        // 회전 힘 추가 (랜덤 값에 강도를 조절)
        _rigidBody.AddTorque(randomTorque * TORQUE_FORCE_OFFSET, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;                         // 충돌 판정은 서버만
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") == false || _rigidBody.isKinematic) return;
        LandedLogicRpc();                                 // 서버 로컬 처리
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void LandedLogicRpc()
    {
        _rigidBody.isKinematic = true;
        transform.position += Vector3.up * DROPITEM_VERTICAL_OFFSET;
        transform.rotation = Quaternion.identity;
        StartCoroutine(RotationDropItem());
        CreateLootingItemEffect();
        _canInteraction = true;
    }


    public void CreateLootingItemEffect()
    {
        ItemGradeEffect(_iteminfo);
    }

    public void SetPosition(Vector3 dropPosition)
    {
        _dropPosition = dropPosition;
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

private void ItemGradeEffect(IItem itemInfo)
{
    string path = itemInfo.Item_Grade switch
    {
        Item_Grade_Type.Normal  => "Prefabs/Paticle/LootingItemEffect/Lootbeams_Runic_Common",
        Item_Grade_Type.Magic   => "Prefabs/Paticle/LootingItemEffect/Lootbeams_Runic_Uncommon",
        Item_Grade_Type.Rare    => "Prefabs/Paticle/LootingItemEffect/Lootbeams_Runic_Rare",
        Item_Grade_Type.Unique  => "Prefabs/Paticle/LootingItemEffect/Lootbeams_Runic_Epic",
        Item_Grade_Type.Epic    => "Prefabs/Paticle/LootingItemEffect/Lootbeams_Runic_Legendary",
        _ => null
    };

    if (string.IsNullOrEmpty(path))
        return;

    Managers.VFX_Manager.GenerateParticle(path, addParticleActionEvent: (itemEffectParticle) =>
    {
        itemEffectParticle.transform.position = transform.position;
        itemEffectParticle.transform.SetParent(transform);
    });
}


    public void Interaction(Module_Player_Interaction caller)
    {
        PlayerPickup(caller);
    }
    public void PlayerPickup(Module_Player_Interaction player)
    {
        PlayerController base_controller = player.PlayerController;
        base_controller.CurrentStateType = base_controller.PickupState;//픽업 애니메이션 실행

        if (base_controller.CurrentStateType != base_controller.PickupState)
            return;

        UI_ItemComponent_Inventory inventory_item = (_iteminfo as IInventoryItemMaker).MakeItemComponentInventory();
        inventory_item.transform.SetParent(_ui_player_Inventory.ItemInventoryTr);
        player.DisEnable_Icon_UI();//상호작용 아이콘 제거
        if (Managers.RelayManager.NetworkManagerEx.IsHost)
        {
            DisEnble_Icon_UI_Rpc();
        }
        else
        {
            Call_DisEnable_Icon_UI_Rpc();
        }
        Managers.RelayManager.DeSpawn_NetWorkOBJ(gameObject);
    }

    [Rpc(SendTo.ClientsAndHost,RequireOwnership = false)]
    public void DisEnble_Icon_UI_Rpc()
    {
        Module_Player_Interaction interaction = Managers.GameManagerEx.Player.GetComponentInChildren<Module_Player_Interaction>();
        if (interaction.enabled == false)
            return;

        if (ReferenceEquals(interaction.InteractionTarget, this))
        {
            interaction.DisEnable_Icon_UI();
        }
    }

    [Rpc(SendTo.Server)]
    public void Call_DisEnable_Icon_UI_Rpc()
    {
        DisEnble_Icon_UI_Rpc();
    }
    public void OutInteraction()
    {

    }
}