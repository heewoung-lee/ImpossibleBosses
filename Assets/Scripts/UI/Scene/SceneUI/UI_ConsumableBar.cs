using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_ConsumableBar : UI_Scene
{

    private Image[] _consumableIcons;
    [SerializeField]private Transform[] _frameTrs;

    public Transform[] FrameTrs => _frameTrs;

    private InputAction[] _comsumableGetKey;

    private UI_BufferBar _ui_BufferBar;

    private PlayerStats _playerStats;

    private UI_ItemDragImage _itemDragImage;

    public UI_ItemDragImage ItemDragImage
    {
        get
        {
            if(_itemDragImage == null)
            {
                _itemDragImage = Managers.UI_Manager.Get_Scene_UI<UI_ItemDragImage>();
            }
            return _itemDragImage;
        }
    }

    private UI_Description _ui_Description;

    public UI_Description UI_Description
    {
        get
        {
            if(_ui_Description == null)
            {
                _ui_Description = Managers.UI_Manager.Get_Scene_UI<UI_Description>();
            }
            return _ui_Description;
        }
    }

    
    enum ConsumableIcons
    {
        Consumable1icon,
        Consumable2icon,
        Consumable3icon,
        Consumable4icon,
    }

    enum FrameCoordinate
    {
        ContextFrame1,
        ContextFrame2,
        ContextFrame3,
        ContextFrame4
    }

    protected override void AwakeInit()
    {
        Debug.Log("여기 울리지 않음?");
        base.AwakeInit();
        InitalizeConsumable();
    }

    private void InitalizeConsumable()
    {
        _consumableIcons = new Image[Enum.GetValues(typeof(ConsumableIcons)).Length];
        Bind<Image>(typeof(ConsumableIcons));
        ConsumableIcons[] consumableIcons = (ConsumableIcons[])System.Enum.GetValues(typeof(ConsumableIcons));
        for (int i = 0; i < _consumableIcons.Length; i++)
        {
            _consumableIcons[i] = Get<Image>((int)consumableIcons[i]);
        }
        _frameTrs = new Transform[Enum.GetValues(typeof(FrameCoordinate)).Length];
        Bind<Transform>(typeof(FrameCoordinate));
        FrameCoordinate[] frameCoordinates = (FrameCoordinate[])System.Enum.GetValues(typeof(FrameCoordinate));
        for (int i = 0; i < _frameTrs.Length; i++)
        {
            _frameTrs[i] = Get<Transform>((int)frameCoordinates[i]);
        }
        _comsumableGetKey = new InputAction[_frameTrs.Length];
        for (int i = 0; i < _comsumableGetKey.Length; i++)
        {
            _comsumableGetKey[i] = Managers.InputManager.GetInputAction(Define.ControllerType.UI, $"Consumabar_GetKey{i + 1}");
            _comsumableGetKey[i].Enable();
        }
    }


    public void UsedPosition(InputAction.CallbackContext context)
    {
        int inputKey = int.Parse(context.control.path.Replace("/Keyboard/",""))-1;

        if (_frameTrs[inputKey].gameObject.TryGetComponentInChildren(out UI_ItemComponent_Consumable consumable))
        {
            if (consumable.ItemCount > 1)
            {
                consumable.ItemCount--;
            }
            else
            {
                Managers.ResourceManager.DestroyObject(consumable.gameObject);
            }
            foreach (StatEffect effect in consumable.ItemEffects)
            {
                Managers.BufferManager.InitBuff(_playerStats,consumable.DuringBuffTime,effect);
            }
        }

        if (ItemDragImage.IsDragImageActive == true)
        {
            ItemDragImage.SetItemImageDisable();
        }
        if(UI_Description.isDescriptionActive == true)
        {
            UI_Description.UI_DescriptionDisable();
        }

    }


    protected override void StartInit()
    {
        
    }

    private void OnEnable()
    {
        if (Managers.GameManagerEx.Player == null)
        {
            Managers.GameManagerEx.OnPlayerSpawnEvent += SetPlayerComsumableBarUI;
        }
        else
        {
            SetPlayerComsumableBarUI(Managers.GameManagerEx.Player.GetComponent<PlayerStats>());
        }
    }

    private void OnDisable()
    {
        foreach (InputAction getKeyEvent in _comsumableGetKey)
        {
            getKeyEvent.performed -= UsedPosition;
            getKeyEvent.Disable();
        }
    }

    private void SetPlayerComsumableBarUI(PlayerStats stats)
    {
        _playerStats = stats;
        foreach (InputAction getKeyEvent in _comsumableGetKey)
        {
            getKeyEvent.performed += UsedPosition;
            getKeyEvent.Enable();
        }
    }
}