using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;
using DG.Tweening;

public class CombatReward : MonoBehaviour {
    [SerializeField] private GoldHandler _goldHandler;
    [SerializeField] private ItemObject _itemPrefab;
    [SerializeField] private ChestObject _chestPrefab;
    private List<ItemData> _itemDataList;
    private List<ChestData> _chestDataList;
    private ItemData _stuffData;
    private ObjectPool<ItemObject> _itemObjectPool;
    private ObjectPool<ChestObject> _chestObjectPool;
    private KdTree<ItemObject> _activeItems;
    public int NumOfItems {
        get { return _activeItems.Count; }
    }
    private static readonly string ItemAddressablesKey = "ItemData";
    public static readonly string ItemObjectTag = "Item";

    private void Awake() {
        _activeItems = new KdTree<ItemObject>();
        AssetLoader assetLoader = AssetLoader.Instance;

        assetLoader.LoadAssetsAsync<ItemData>(ItemAddressablesKey, (x) => {
            _itemDataList = x.Result as List<ItemData>;
        });
        assetLoader.LoadAssetsAsync<ChestData>(ItemAddressablesKey, (x) => {
            _chestDataList = x.Result as List<ChestData>;
        });

        string stuffSOName = "ItemStuff";
        assetLoader.LoadAssetAsync<ItemData>(stuffSOName, (x) => {
            _stuffData = x.Result;
        });

        _itemObjectPool = new ObjectPool<ItemObject>(
            10,
            () => Instantiate(_itemPrefab),
            (x) => x.gameObject.SetActive(true),
            (x) => x.gameObject.SetActive(false)
        );
        _chestObjectPool = new ObjectPool<ChestObject>(
            5,
            () => Instantiate(_chestPrefab),
            (x) => x.gameObject.SetActive(true),
            (x) => x.gameObject.SetActive(false)
        );
    }

    private void Update() {
        for (int i = 0; i < _activeItems.Count; ++i) {
            if (!_activeItems[i].gameObject.activeSelf) {
                _activeItems.RemoveAt(i--);
            }
        }
    }

    public void SpawnItemRewardAt(Vector3 position) {
        ItemData item = _itemDataList.GetRandomElement();
        SpawnRewardAt(item, position, true);
    }

    public void SpawnStuffRewardAt(Vector3 position) {
        SpawnRewardAt(_stuffData, position, false);
    }

    private ItemObject SpawnRewardAt(ItemData item, Vector3 position, bool canEquip) {
        ItemObject obj = CreateItemObject(item, position);
        obj.SetInteraction(OnItemReturn, canEquip);
        obj.SetSprite(item.Sprite);
        _activeItems.Add(obj);
        return obj;
    }

    private ItemObject CreateItemObject(ItemData item, Vector3 position) {
        ItemObject obj = _itemObjectPool.GetObject();
        obj.transform.position = position;
        obj.ItemData = item;
        return obj;
    }
    
    public void OnItemRelease(Vector3 origin, ItemData item) {
        Vector3 targetPosition = origin.ChangeYPos(origin.y + 50f);
        ItemObject itemObject = SpawnRewardAt(item, origin, true);
        itemObject.transform.DOLocalMoveY(-50f, 0.5f).SetRelative();
    }

    public void SpawnChestAt(Vector3 position) {
        ChestObject obj = _chestObjectPool.GetObject();
        obj.transform.position = position;
        InitializeChest(obj, _chestDataList.GetRandomElement());
    }

    public ItemObject GetClosestItem(Vector3 position) {
        return _activeItems.FindClosest(position);
    }

    public void OnItemReturn(ItemObject item) {
        _itemObjectPool.ReturnObject(item);
    }

    private void InitializeChest(ChestObject obj, ChestData data) {
        Vector3 chestPosition = obj.transform.position;
        UnityAction onChestClicked = null;

        switch (data.ChestType) {
        case ChestData.Type.Normal:
            onChestClicked = () => {
                SpawnItemRewardAt(chestPosition);
                SpawnStuffRewardAt(chestPosition);
                int amount = 40;
                _goldHandler.GainGold(amount);
            };
            break;
        case ChestData.Type.Desert:
            break;
        case ChestData.Type.Corpse:
            break;
        case ChestData.Type.Purse:
            break;
        case ChestData.Type.Vehicle:
            break;
        }
        
        obj.SetSprite(data.ChestSprite);
        obj.SetInteraction(_chestObjectPool, onChestClicked);
    }
}