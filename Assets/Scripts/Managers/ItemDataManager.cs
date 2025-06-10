using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;
public class ItemDataManager : IManagerInitializable
{
    private const string ITEM_FRAME_BORDER_PATH = "Art/UI/GUI Pro-FantasyRPG/ResourcesData/Sprites/Component/Frame";
    private Dictionary<Item_Grade_Type, Sprite> _itemGradeBorder;
    public Dictionary<Item_Grade_Type, Sprite> ItemGradeBorder => _itemGradeBorder;

    private Dictionary<Type, Dictionary<int, IItem>> _allItemDataDict = new Dictionary<Type, Dictionary<int, IItem>>();
    private Dictionary<int, IItem> _itemDataKeyDict = new Dictionary<int, IItem>();
    private List<Type> _itemDataType;
    public void Init()
    {

        //폴더내에 있는 타입들을 긁어와서 데이터를 읽는다.
        _itemDataType = Managers.DataManager.LoadSerializableTypesFromFolder("Assets/Scripts/Data/DataType/ItemType"
            , Managers.DataManager.AddSerializableAttributeType);

        foreach (Type itemtype in _itemDataType)
        {
            IDictionary itemTypeIDict = Managers.DataManager.AllDataDict[itemtype] as IDictionary;
            //IDictionary 인터페이스로 변환한 후 모든 Dictionary데이터를 받아올 수 있도록한다.
            if (itemTypeIDict == null)
            {
                Debug.Log($"Not Found Type:{itemtype}");
                continue;
            }

            Dictionary<int, IItem> itemDict = new Dictionary<int, IItem>();
            foreach (int key in itemTypeIDict.Keys)//해당타입의 키 값과 대응 되는 데이터를 캐스팅한 후 딕셔너리에 넣어준다.
            {
                IItem itemValue = itemTypeIDict[key] as IItem;//키에 대응되는 IItem
                if (itemValue == null)
                {
                    Debug.LogError($"Failed to cast item of type {itemtype} to IItem");
                    continue;
                }
                itemDict[key] = itemValue;
                _itemDataKeyDict.Add(key, itemValue);
            }
            _allItemDataDict[itemtype] = itemDict;
            _allItemDataDict[itemtype] = BindImageSources(_allItemDataDict[itemtype]) as Dictionary<int, IItem>;
        }

        _itemGradeBorder = new Dictionary<Item_Grade_Type, Sprite>//아이템 등급 프레임 초기화
        {
            { Item_Grade_Type.Normal, Managers.ResourceManager.Load<Sprite>(ITEM_FRAME_BORDER_PATH + "/ItemFrame_01_Border_White") },
            { Item_Grade_Type.Magic, Managers.ResourceManager.Load<Sprite>(ITEM_FRAME_BORDER_PATH + "/ItemFrame_01_Border_Green") },
            { Item_Grade_Type.Rare, Managers.ResourceManager.Load<Sprite>(ITEM_FRAME_BORDER_PATH + "/ItemFrame_01_Border_Blue") },
            { Item_Grade_Type.Unique, Managers.ResourceManager.Load<Sprite>(ITEM_FRAME_BORDER_PATH + "/ItemFrame_01_Border_Red") },
            { Item_Grade_Type.Epic, Managers.ResourceManager.Load<Sprite>(ITEM_FRAME_BORDER_PATH + "/ItemFrame_01_Border_Yellow") }
        };
    }

    public IItem GetItem(int itemNumber)
    {
        if(_itemDataKeyDict.TryGetValue(itemNumber,out IItem iteminfo))
        {
            return iteminfo;
        }
        return null;
    }

    public IItem GetRandomItem(Type itemtype)
    {
        IItem item = null;
        List<int> keylist;
        int randomKey = 0;

        Dictionary<int, IItem> itemDict = _allItemDataDict[itemtype];
        keylist = itemDict.Keys.ToList();
        randomKey = keylist[Random.Range(0, keylist.Count)];
        item = itemDict[randomKey];

        return item;
    }

    public IItem GetRandomItemFromAll()
    {
        Type randomType = _itemDataType[Random.Range(0, _itemDataType.Count)];
        return GetRandomItem(randomType);
    }
    private IDictionary BindImageSources(IDictionary missingImageItemsDict)
    {
        IDictionary itemDict;
        itemDict = missingImageItemsDict;

        foreach (System.Object key in itemDict.Keys)
        {
            IItem itemType = itemDict[key] as IItem;
            FindImageByKey(itemType);
        }
        return itemDict;
    }//딕셔너리로 던져줘야함.

    public void FindImageByKey(IItem items)
    {
        items.ImageSource[items.ItemIconSourceText] = Managers.ResourceManager.Load<Sprite>($"Art/UI/GUI Pro-FantasyRPG/ResourcesData/Sprites/Component/Icon_EquipIcons/Shadow/256/{items.ItemIconSourceText}");
        //TODO: 나중에 사용하는 모든 이미지파일을 모아서 경로지정을 다시 해야함.
    }



    public GameObject GetEquipLootItem(IItem iteminfo)
    {
        GameObject lootItem;
        switch ((iteminfo as ItemEquipment).Equipment_Slot)
        {
            case Equipment_Slot_Type.Helmet:
            case Equipment_Slot_Type.Armor:
                lootItem = Managers.ResourceManager.Instantiate("NGO/LootingItem/Shield");
                break;
            case Equipment_Slot_Type.Weapon:
                lootItem = Managers.ResourceManager.Instantiate("NGO/LootingItem/Sword");
                break;
            default:
                lootItem = Managers.ResourceManager.Instantiate("NGO/LootingItem/Bag");
                break;
        }
        lootItem.GetComponent<LootItem>().SetIteminfo(iteminfo);
        return lootItem;
    }

    public GameObject GetConsumableLootItem(IItem iteminfo)
    {
        GameObject lootitem = Managers.ResourceManager.Instantiate("NGO/LootingItem/Potion");
        lootitem.GetComponent<LootItem>().SetIteminfo(iteminfo);
        return lootitem;
    }
}
