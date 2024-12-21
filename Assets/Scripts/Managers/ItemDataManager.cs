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

    private List<Type> _itemDataType;
    public void Init()
    {

        //�������� �ִ� Ÿ�Ե��� �ܾ�ͼ� �����͸� �д´�.
        _itemDataType = Managers.DataManager.LoadSerializableTypesFromFolder("Assets/Scripts/Data/DataType/ItemType"
            , Managers.DataManager.AddSerializableAttributeType);

        foreach (Type itemtype in _itemDataType)
        {
            IDictionary itemTypeIDict = Managers.DataManager.AllDataDict[itemtype] as IDictionary;
            //IDictionary �������̽��� ��ȯ�� �� ��� Dictionary�����͸� �޾ƿ� �� �ֵ����Ѵ�.
            if (itemTypeIDict == null)
            {
                Debug.Log($"Not Found Type:{itemtype}");
                continue;
            }

            Dictionary<int, IItem> itemDict = new Dictionary<int, IItem>();
            foreach (int key in itemTypeIDict.Keys)//�ش�Ÿ���� Ű ���� ���� �Ǵ� �����͸� ĳ������ �� ��ųʸ��� �־��ش�.
            {
                IItem itemValue = itemTypeIDict[key] as IItem;//Ű�� �����Ǵ� IItem
                if (itemValue == null)
                {
                    Debug.LogError($"Failed to cast item of type {itemtype} to IItem");
                    continue;
                }
                itemDict[key] = itemValue;
            }
            _allItemDataDict[itemtype] = itemDict;
            _allItemDataDict[itemtype] = BindImageSources(_allItemDataDict[itemtype]) as Dictionary<int, IItem>;
        }

        _itemGradeBorder = new Dictionary<Item_Grade_Type, Sprite>//������ ��� ������ �ʱ�ȭ
        {
            { Item_Grade_Type.Normal, Managers.ResourceManager.Load<Sprite>(ITEM_FRAME_BORDER_PATH + "/ItemFrame_01_Border_White") },
            { Item_Grade_Type.Magic, Managers.ResourceManager.Load<Sprite>(ITEM_FRAME_BORDER_PATH + "/ItemFrame_01_Border_Green") },
            { Item_Grade_Type.Rare, Managers.ResourceManager.Load<Sprite>(ITEM_FRAME_BORDER_PATH + "/ItemFrame_01_Border_Purple") },
            { Item_Grade_Type.Unique, Managers.ResourceManager.Load<Sprite>(ITEM_FRAME_BORDER_PATH + "/ItemFrame_01_Border_Red") },
            { Item_Grade_Type.Epic, Managers.ResourceManager.Load<Sprite>(ITEM_FRAME_BORDER_PATH + "/ItemFrame_01_Border_Yellow") }
        };
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
    }//��ųʸ��� ���������.

    public void FindImageByKey(IItem items)
    {
        items.ImageSource[items.ItemIconSourceText] = Managers.ResourceManager.Load<Sprite>($"Art/UI/GUI Pro-FantasyRPG/ResourcesData/Sprites/Component/Icon_EquipIcons/Shadow/256/{items.ItemIconSourceText}");
        //TODO: ���߿� ����ϴ� ��� �̹��������� ��Ƽ� ��������� �ٽ� �ؾ���.
    }
}
