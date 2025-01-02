using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_ConsumableBar : UI_Scene
{

    private Image[] _consumableIcons;
    private Transform[] _frameTrs;

    public Transform[] FrameTrs => _frameTrs;

    public Image ItemDragImage => _itemDragImage;

    private Image _itemDragImage;

    private InputAction[] _comsumableGetKey;

    private UI_BufferBar _ui_BufferBar;

    private PlayerStats _playerStats;

    
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
        base.AwakeInit();
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
            _comsumableGetKey[i] = Managers.InputManager.GetInputAction(Define.ControllerType.UI, $"Consumabar_GetKey{i+1}");
            _comsumableGetKey[i].Enable();
        }
        _itemDragImage = Utill.FindChild<Image>(gameObject, "ItemDragImage");
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
            foreach (ItemEffect effect in consumable.ItemEffects)
            {
                Managers.BufferManager.InitBuff(_playerStats,consumable.DuringBuffTime,effect);
            }
        }

    }


    protected override void StartInit()
    {
        _playerStats = Managers.GameManagerEx.Player.GetComponent<PlayerStats>();
        foreach (InputAction getKeyEvent in _comsumableGetKey)
        {
            getKeyEvent.performed += UsedPosition;
            getKeyEvent.Enable();
        }
    }
}