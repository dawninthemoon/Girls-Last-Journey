using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class CombatReward : MonoBehaviour {
    [SerializeField] private ItemObject _itemPrefab = null;
    List<EntityItem> _itemDataList;
    private ObjectPool<ItemObject> _itemObjectPool;
    private static readonly string ItemAddressablesKey = "EntityItems";

    private void Awake() {
        AssetLoader assetLoader = AssetLoader.Instance;
        assetLoader.LoadAssetsAsync<EntityItem>(ItemAddressablesKey, (x) => {
            _itemDataList = x.Result as List<EntityItem>;
        });
        _itemObjectPool = new ObjectPool<ItemObject>(
            10,
            () => Instantiate(_itemPrefab),
            (x) => x.gameObject.SetActive(true),
            (x) => x.gameObject.SetActive(false)
        );
    }

    public void SpawnRewardAt(Vector3 position) {
        EntityItem item = _itemDataList.GetRandomElement();
        ItemObject obj = CreateItemObject(item, position);
        obj.SetInteraction(_itemObjectPool);
        obj.SetSprite(item.Sprite);
    }

    private ItemObject CreateItemObject(EntityItem item, Vector3 position) {
        ItemObject obj = _itemObjectPool.GetObject();
        obj.transform.position = position;
        obj.ItemData = item;
        return obj;
    }

    private void OnRewardSelected(ItemObject item, System.Action<EntityItem> onRewardSelected) {
        item.gameObject.SetActive(false);
        onRewardSelected?.Invoke(item.ItemData);
    }
}