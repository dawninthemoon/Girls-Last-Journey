using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class EncounterHandler : MonoBehaviour {
    [SerializeField] private GoldHandler _goldHandler;
    [SerializeField] private InteractiveEncounter _collectorPrefab;
    private List<EncounterEntityBase> _inactiveEncounters;
    private List<EncounterEntityBase> _activeEncounters;
    public static readonly string EncountersLabel = "Encounters";
    private UnityAction[] _interactiveSettingArray;
    private ObjectPool<InteractiveEncounter> _collectorObjectPool;

    private void Awake() {
        _interactiveSettingArray = new UnityAction[] {
            OnCollectorInteraction,
            OnVampireInteraction,
            OnHumanTraffickerInteraction,
            null
        };
        InitializeEncounters();
        InitializeCollectorObjectPool();
    }

    private void Update() {
        if (_activeEncounters == null)
            return;

        foreach (EncounterEntityBase encounter in _activeEncounters) {
            encounter.Progress();
        }
    }

    public void SpawnRandomEncounter() {
        if (_inactiveEncounters.Count == 0) {
            return;
        }

        EncounterEntityBase encounter = _inactiveEncounters.GetRandomElement();
        encounter.transform.position = GetInitialPosition();
        encounter.gameObject.SetActive(true);
        
        _activeEncounters.Add(encounter);
        _inactiveEncounters.Remove(encounter);
    }

    public void SpawnCollectorEncounter() {
        var collector = _collectorObjectPool.GetObject();
        collector.transform.position = GetInitialPosition();

        _activeEncounters.Add(collector);
    }

    private void InitializeEncounters() {
        var assetLoader = AssetLoader.Instance;
        assetLoader.LoadAssetsAsync<GameObject>(EncountersLabel, (op) => {
            var entities = op.Result as List<GameObject>;

            _activeEncounters = new List<EncounterEntityBase>(op.Result.Count);
            _inactiveEncounters = new List<EncounterEntityBase>(op.Result.Count);

            foreach (GameObject entity in entities) {
                EncounterEntityBase encounter = entity.GetComponent<EncounterEntityBase>();
                encounter = Instantiate(encounter);

                _inactiveEncounters.Add(encounter);
                _interactiveSettingArray[(int)encounter.EncounterType]?.Invoke();

                encounter.gameObject.SetActive(false);
            }
        });
    }

    private void InitializeCollectorObjectPool() {
        _collectorObjectPool = new ObjectPool<InteractiveEncounter>(
            3,
            () => Instantiate(_collectorPrefab),
            (x) => x.gameObject.SetActive(true),
            (x) => x.gameObject.SetActive(false)
        );
    }

    private void OnCollectorInteraction() {
        int goldAmount = Random.Range(30, 50);
        _goldHandler.GainGold(goldAmount);
    }

    private void OnVampireInteraction() {
        int goldAmount = Random.Range(30, 50);
        _goldHandler.GainGold(goldAmount);
    }

    private void OnHumanTraffickerInteraction() {
        
    }

    private Vector3 GetInitialPosition() {
        Vector2 stageMinSize = CombatMap.StageMinSize;
        Vector2 stageMaxSize = CombatMap.StageMaxSize;

        float randX = Random.Range(stageMinSize.x, stageMaxSize.x);
        float randY = Random.Range(0, 2) > 0 ? stageMaxSize.y : stageMinSize.y;

        return new Vector3(randX, randY);
    }
}
