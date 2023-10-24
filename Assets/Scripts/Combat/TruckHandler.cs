using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;

public class TruckHandler : MonoBehaviour {
    [SerializeField] private MemberHandler _memberHandler;
    [SerializeField] private EnemyHandler _enemyHandler;
    [SerializeField] private CombatReward _rewardControl;
    [SerializeField] private Truck _truckPrefab;
    [SerializeField] private Button _truckSpawnButton;
    private TruckDirectionSelect _truckDirectionSelector;
    private ObjectPool<Truck> _truckObjectPool;

    private void Awake() {
        _truckDirectionSelector = GetComponent<TruckDirectionSelect>();

        _truckObjectPool = new ObjectPool<Truck>(
            5,
            () => Instantiate(_truckPrefab),
            (x) => x.gameObject.SetActive(true),
            (x) => x.gameObject.SetActive(false)
        );
    }

    private void Start() {
        var stream = _truckSpawnButton.OnClickAsObservable();
        stream.Buffer(stream.ThrottleFirst(System.TimeSpan.FromSeconds(0.5)))
            .Subscribe(_ => SpawnTruck());
    }

    private void SpawnTruck() {
        EntityBase target = _enemyHandler.GetRandomEnemy();
        if (target != null) {
            SpawnTruck(target.transform.position);
        }
    }

    private void SpawnTruck(Vector3 targetPosition) {
        Vector2 stageMinSize = CombatMap.StageMinSize;
        Vector2 stageMaxSize = CombatMap.StageMaxSize;

        float randX = Random.Range(stageMinSize.x, stageMaxSize.x);
        float randY = Random.Range(0, 2) > 0 ? stageMaxSize.y : stageMinSize.y;
        Vector2 start = new Vector2(randX, randY);

        var truckObject = _truckObjectPool.GetObject();

        Vector2 startPosition 
            = _truckDirectionSelector.GetStartPoint(truckObject, start, targetPosition);
        var config 
            = _truckDirectionSelector.GetTruckMoveConfig(truckObject, start, targetPosition);

        truckObject.StartMove(config.Item1, config.Item2, config.Item3, OnTruckMoveEnd);

        DropReward(truckObject).Forget();
    }

    private async UniTaskVoid DropReward(Truck truckObject) {
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

        Vector3 truckPosition = truckObject.transform.position;

        if (_memberHandler.NumOfMembers < 3) {
            _memberHandler.SpawnMember(truckPosition);
        }
        else if (!_memberHandler.DoesEveryoneHaveItems()) {
            _rewardControl.SpawnItemRewardAt(truckPosition);
        }
        else {
            _rewardControl.SpawnChestAt(truckPosition);
        }
    }

    private void OnTruckMoveEnd(Truck truckObject) {
        _truckObjectPool.ReturnObject(truckObject);
    }
}
