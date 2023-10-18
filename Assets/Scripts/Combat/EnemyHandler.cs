using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

public class EnemyHandler : MonoBehaviour {
    [SerializeField] private CombatDamageDisplay _damageDisplayer;
    private Dictionary<int, CombatWaveConfig> _waveConfigDictionary;
    private KdTree<EntityBase> _activeEnemies;
    public KdTree<EntityBase> ActiveEnemies { get { return _activeEnemies; } }
    private Dictionary<string, EntityInfo> _enemyInfoDictionary;
    private EntityBase _enemyPrefab;
    private EntitySpawner _enemySpawner;
    private static readonly string EnemyPrefabName = "EnemyPrefab";

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

    public void Progress(KdTree<EntityBase> allies) {
        foreach (EntityBase enemy in _activeEnemies) {
            EntityBase targetEntity = allies.FindClosest(enemy.transform.position);
            ITargetable target = targetEntity?.GetComponent<Agent>();
            enemy.SetTarget(target);
        }

        for (int i = 0; i < _activeEnemies.Count; ++i) {
            var enemy = _activeEnemies[i];
            if (enemy.Health <= 0 || !enemy.gameObject.activeSelf) {
                _activeEnemies.RemoveAt(i--);
            }
        }
    }

    public EntityBase GetRandomEnemy() {
        if (_activeEnemies.Count == 0) {
            return null;
        }
        int randomIndex = Random.Range(0, _activeEnemies.Count);
        return _activeEnemies[randomIndex];
    }

    public void SpawnEnemies(int waveCount, CombatStageConfig stageConfig) {
        Vector2 stageMinSize = CombatMap.StageMinSize;
        Vector2 stageMaxSize = CombatMap.StageMaxSize;

        //int waveRank = stageConfig.StageInfoArray[waveCount - 1];
        int waveRank = 1;

        CombatWaveConfig waveConfig = _waveConfigDictionary[waveRank];
        CombatWaveInfo selectedWave = waveConfig.WaveInfoArray[Random.Range(0, waveConfig.WaveInfoArray.Length)];
        
        for (int i = 0; i < selectedWave.enemyIDArray.Length; ++i) {
            float randX = Random.Range(stageMinSize.x, stageMaxSize.x);
            float y;
            if (waveCount == 1) {
                y = Random.Range(stageMinSize.y / 4f, stageMaxSize.y / 4f);
            }
            else {
                 y = Random.Range(0, 2) > 0 ? stageMaxSize.y + _enemyPrefab.Radius : stageMinSize.y - _enemyPrefab.Radius;
            }
            
            EntityInfo selectedInfo = _enemyInfoDictionary[selectedWave.enemyIDArray[i]];
            EntityDecorator decorator = new EntityDecorator(selectedInfo);
            EntityBase enemy = _enemySpawner.CreateEntity(decorator);
            enemy.transform.position = new Vector3(randX, y);

            _activeEnemies.Add(enemy);
        }
    }

    public void RemoveAllEnemies(EntitySpawner entitySpanwer) {
        for (int i = 0; i < _activeEnemies.Count; ++i) {
            entitySpanwer.RemoveEntity(_activeEnemies[i]);
            _activeEnemies.RemoveAt(i--);
        }
    }
}
