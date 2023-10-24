using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;
using DG.Tweening;

public class CombatReward : MonoBehaviour {
    [SerializeField] private ItemObject _itemPrefab = null;
    [SerializeField] private ChestObject _chestPrefab = null;
    private List<ItemData> _itemDataList;
    private List<ChestData> _chestDataList;
    private ItemData _stuffData;
    private ObjectPool<ItemObject> _itemObjectPool;
    private ObjectPool<ChestObject> _chestObjectPool;
    private static readonly string ItemAddressablesKey = "ItemData";

    private void Awake() {
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

    public void SpawnItemRewardAt(Vector3 position) {
        ItemData item = _itemDataList.GetRandomElement();
        SpawnRewardAt(item, position, true);
    }

    public void SpawnStuffRewardAt(Vector3 position) {
        SpawnRewardAt(_stuffData, position, false);
    }

    private void SpawnRewardAt(ItemData item, Vector3 position, bool canEquip) {
        ItemObject obj = CreateItemObject(item, position);
        obj.SetInteraction(_itemObjectPool, canEquip);
        obj.SetSprite(item.Sprite);
    }

    private ItemObject CreateItemObject(ItemData item, Vector3 position) {
        ItemObject obj = _itemObjectPool.GetObject();
        obj.transform.position = position;
        obj.ItemData = item;
        return obj;
    }
    
    public void OnItemRelease(Vector3 origin, ItemData item) {
        Vector3 targetPosition = origin.ChangeYPos(origin.y + 50f);
        ItemObject itemObject = CreateItemObject(item, origin);
        itemObject.transform.DOLocalMoveY(-50f, 0.5f).SetRelative();
    }

    public void SpawnChestAt(Vector3 position) {
        ChestObject obj = _chestObjectPool.GetObject();
        obj.transform.position = position;
        InitializeChest(obj, _chestDataList.GetRandomElement());
    }

    private void InitializeChest(ChestObject obj, ChestData data) {
        Vector3 chestPosition = obj.transform.position;
        UnityAction onChestClicked = null;

        switch (data.ChestType) {
        case ChestData.Type.Normal:
            onChestClicked = () => {
                SpawnItemRewardAt(chestPosition);
                SpawnStuffRewardAt(chestPosition);
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