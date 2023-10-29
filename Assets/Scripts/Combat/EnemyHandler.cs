using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using Cysharp.Threading.Tasks;
using RieslingUtils;

public class EnemyHandler : MonoBehaviour {
    [SerializeField] private CombatDamageDisplay _damageDisplayer;
    private Dictionary<int, CombatWaveConfig> _waveConfigDictionary;
    private KdTree<EntityBase> _activeEnemies;
    public KdTree<EntityBase> ActiveEnemies { get { return _activeEnemies; } }
    private Dictionary<string, EntityInfo> _enemyInfoDictionary;
    private EntityBase _enemyPrefab;
    private EntitySpawner _enemySpawner;
    private static readonly string EnemyPrefabName = "EnemyPrefab";
    public static readonly string EnemyTagName = "Enemy";

    private void Awake() {
        _activeEnemies = new KdTree<EntityBase>();
        _enemySpawner = new EntitySpawner(EnemyPrefabName, transform, _damageDisplayer);
        var assetLoader = AssetLoader.Instance;

        assetLoader.LoadAssetsAsync<CombatWaveConfig>("CombatConfig", (x) => {
            _waveConfigDictionary = x.Result.ToDictionary(x => x.WaveRank);
        });
        assetLoader.LoadAssetsAsync<EntityConfig>("EnemyInfo", (x) => {
            _enemyInfoDictionary = x.Result
                                    .Select(config => config.Info)
                                    .ToDictionary(info => info.entityID);
        });

        assetLoader.LoadAssetAsync<GameObject>("EnemyPrefab", (x) => {
            _enemyPrefab = x.Result.GetComponent<EntityBase>();
        });
    }

    public void Progress(KdTree<EntityBase> allies, UnityAction<Vector3> onEnemyDead) {
        foreach (EntityBase enemy in _activeEnemies) {
            EntityBase targetEntity = allies.FindClosest(enemy.transform.position);
            ITargetable target = targetEntity?.GetComponent<Agent>();
            enemy.SetTarget(target);
        }

        for (int i = 0; i < _activeEnemies.Count; ++i) {
            var enemy = _activeEnemies[i];
            if (enemy.Health <= 0 || !enemy.gameObject.activeSelf) {
                onEnemyDead.Invoke(enemy.transform.position);
                _activeEnemies.RemoveAt(i--);
                _enemySpawner.RemoveEntity(enemy);
            }
        }
    }

    public EntityBase GetRandomEnemyInCamera() {
        if (_activeEnemies.Count == 0) {
            return null;
        }
        EntityBase result;
        do {
            int randomIndex = Random.Range(0, _activeEnemies.Count);
            result =  _activeEnemies[randomIndex];
        } while (!IsEnemyExistsInCamera(result));
        return result;
    }

    public void SpawnEnemies(int waveCount, CombatStageConfig stageConfig) {
        Vector2 stageMinSize = CombatMap.StageMinSize;
        Vector2 stageMaxSize = CombatMap.StageMaxSize;

        for (int i = 0; i < waveCount; ++i) {
             float randX = Random.Range(stageMinSize.x, stageMaxSize.x);
            float randY = Random.Range(0, 2) > 0 ? stageMaxSize.y + _enemyPrefab.Radius : stageMinSize.y - _enemyPrefab.Radius;
            
            EntityInfo selectedInfo = _enemyInfoDictionary.GetRandomValue();
            EntityDecorator decorator = new EntityDecorator(selectedInfo);
            EntityBase enemy = _enemySpawner.CreateEntity(decorator);
            enemy.transform.position = new Vector3(randX, randY, -5f);

            _activeEnemies.Add(enemy);
        }
    }

    public void RemoveAllEnemies(EntitySpawner entitySpanwer) {
        for (int i = 0; i < _activeEnemies.Count; ++i) {
            entitySpanwer.RemoveEntity(_activeEnemies[i]);
            _activeEnemies.RemoveAt(i--);
        }
    }

    public bool IsEnemyExistsInCamera() {
        bool result = false;
        foreach (EntityBase enemy in _activeEnemies) {
            if (IsEnemyExistsInCamera(enemy)) {
                result = true;
                break;
            }
        }
        return result;
    }

    private bool IsEnemyExistsInCamera(EntityBase enemy) {
        return CombatMap.IsInside(enemy.transform.position, enemy.Radius);
    }
}
