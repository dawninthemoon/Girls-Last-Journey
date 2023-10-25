using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class EncounterHandler : MonoBehaviour {
    [SerializeField] private CombatReward _rewardControl;
    [SerializeField] private GoldHandler _goldHandler;
    [SerializeField] private InteractiveEncounter _collectorPrefab;
    private List<EncounterEntityBase> _activeEncounters;
    public static readonly string EncountersLabel = "Encounters";
    private UnityAction[] _interactiveSettingArray;
    private ObjectPool<EncounterEntityBase> _collectorObjectPool;
    private Dictionary<EncounterEntityBase.Type, ObjectPool<EncounterEntityBase>> _encounterObjectPool;

    private void Awake() {
        _encounterObjectPool = new Dictionary<EncounterEntityBase.Type, ObjectPool<EncounterEntityBase>>();
        _activeEncounters = new List<EncounterEntityBase>();
        _interactiveSettingArray = new UnityAction[] {
            OnCollectorInteraction,
            OnVampireInteraction,
            null,
            null
        };
        InitializeEncounters();
        _collectorObjectPool = CreateObjectPool(_collectorPrefab, OnCollectorInteraction);
    }

    private void Update() {
        if (_activeEncounters == null)
            return;

        for (int i = 0; i < _activeEncounters.Count; ++i) {
            if (_activeEncounters[i].gameObject.activeSelf) {
                _activeEncounters[i].Progress();
            }
            else {
                _activeEncounters.RemoveAt(i--);
            }
        }
    }

    public void SpawnRandomEncounter() {
        EncounterEntityBase.Type encounterType = EncounterEntityBase.Type.HumanTrafficker;
        bool canAppear;
        do { 
            canAppear = true;
            encounterType = _encounterObjectPool.GetRandomKey();
            if (encounterType.Equals(EncounterEntityBase.Type.StarvingOne)) {
                if (_rewardControl.NumOfItems < StarvingOneEncounter.TargetClearCount) {
                    canAppear = false;
                }
            }
        } while (!canAppear);

        var encounter = _encounterObjectPool[encounterType].GetObject();
        encounter.OnEncounter(GetInitialPosition());
        encounter.gameObject.SetActive(true);
        
        _activeEncounters.Add(encounter);
    }

    public void SpawnCollectorEncounter() {
        var collector = _collectorObjectPool.GetObject();
        collector.OnEncounter(GetInitialPosition());

        _activeEncounters.Add(collector);
    }

    private void InitializeEncounters() {
        var assetLoader = AssetLoader.Instance;
        assetLoader.LoadAssetsAsync<GameObject>(EncountersLabel, (op) => {
            var entities = op.Result as List<GameObject>;

            foreach (GameObject entity in entities) {
                EncounterEntityBase prefab = entity.GetComponent<EncounterEntityBase>();
                var interactionCallback = _interactiveSettingArray[(int)prefab.EncounterType];
                _encounterObjectPool.Add(prefab.EncounterType, CreateObjectPool(prefab, interactionCallback));
            }
        });
    }

    private ObjectPool<EncounterEntityBase> CreateObjectPool(EncounterEntityBase prefab, UnityAction interactionCallback) {
        var objectPool = new ObjectPool<EncounterEntityBase>(
            3,
            () => {
                var encounter = Instantiate(prefab);
                encounter.Initialize(interactionCallback, OnEncounterEnd);
                if (encounter is StarvingOneEncounter) {
                    (encounter as StarvingOneEncounter)
                        .SetCallbackSettings(
                            _rewardControl.GetClosestItem,
                            _rewardControl.OnItemReturn
                        );
                }
                return encounter;
            },
            (x) => x.gameObject.SetActive(true),
            (x) => x.gameObject.SetActive(false)
        );
        return objectPool;
    }

    private void OnCollectorInteraction() {
        int goldAmount = Random.Range(30, 50);
        _goldHandler.GainGold(goldAmount);
    }

    private void OnVampireInteraction() {
        int goldAmount = Random.Range(30, 50);
        _goldHandler.GainGold(goldAmount);
    }

    private void OnEncounterEnd(EncounterEntityBase encounter) {
        encounter.gameObject.SetActive(false);
        if (encounter.EncounterType.Equals(EncounterEntityBase.Type.Collector)) {
            _collectorObjectPool.ReturnObject(encounter as InteractiveEncounter);
        }
        else {
            _encounterObjectPool[encounter.EncounterType].ReturnObject(encounter);
        }
    }

    private Vector3 GetInitialPosition() {
        Vector2 stageMinSize = CombatMap.StageMinSize;
        Vector2 stageMaxSize = CombatMap.StageMaxSize;

        float randX = Random.Range(stageMinSize.x, stageMaxSize.x);
        float randY = Random.Range(0, 2) > 0 ? stageMaxSize.y : stageMinSize.y;

        return new Vector3(randX, randY);
    }
}
