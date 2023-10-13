using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EntitySpawner {
    private ObjectPool<EntityBase> _objectPool;
    private CombatDamageDisplay _damageDisplayer;
    
    private static readonly string AllyPrefabName = "AllyPrefab";

    public EntitySpawner(string prefabName, Transform entityParent, CombatDamageDisplay damageDisplayer) {
        _damageDisplayer = damageDisplayer;

        AssetLoader.Instance.LoadAssetAsync<GameObject>(
            prefabName,
            (op) => OnPrefabLoadCompleted(ref _objectPool, op.Result.GetComponent<EntityBase>(), entityParent)
        );
    }

    private void OnPrefabLoadCompleted(ref ObjectPool<EntityBase> objectPool, EntityBase prefab, Transform entityParent) {
        objectPool = new ObjectPool<EntityBase>(
            10,
            () => CreateEntityBase(prefab, entityParent),
            OnEntityActive,
            OnEntityDisable
        );
    }

    public EntityBase CreateEntity(EntityDecorator entityDecorator) {
        EntityBase instance = _objectPool.GetObject();
        instance.Initialize(entityDecorator);
        return instance;
    }

    public void RemoveEntity(EntityBase entity) {
        _objectPool.ReturnObject(entity);
    }

    private EntityBase CreateEntityBase(EntityBase prefab, Transform entityParent) {
        EntityBase instance = GameObject.Instantiate(prefab, entityParent);
        return instance;
    }

    private void OnEntityActive(EntityBase entity) {
        entity.gameObject.SetActive(true);
        entity.FinalDamageInfo?.Attach(_damageDisplayer);
        //GameMain.PlayerData.Attach(entity);
    }

    private void OnEntityDisable(EntityBase entity) {
        entity.gameObject.SetActive(false);
        entity.FinalDamageInfo?.Detach(_damageDisplayer);
        entity.SetTarget(null);
        //GameMain.PlayerData.Detach(entity);
    }
}
