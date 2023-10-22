using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using UniRx;
using UniRx.Triggers;

public class TruckHandler : MonoBehaviour {
    [SerializeField] private EnemyHandler _enemyHandler;
    [SerializeField] private CombatReward _rewardControl;
    [SerializeField] private Truck _truckObject;
    [SerializeField] private Button _truckSpawnButton;
    private TruckDirectionSelect _truckDirectionSelector;

    private void Awake() {
        _truckDirectionSelector = GetComponent<TruckDirectionSelect>();
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

        Vector2 startPosition = _truckDirectionSelector.GetStartPoint(start, targetPosition);
        var config = _truckDirectionSelector.GetTruckMoveConfig(start, targetPosition);

        _truckObject.StartMove(config.Item1, config.Item2, config.Item3, OnTruckMoveEnd);

        DropChest().Forget();
    }

    private async UniTaskVoid DropChest() {
        await UniTask.Delay(System.TimeSpan.FromSeconds(0.5f));

        _rewardControl.SpawnRewardAt(_truckObject.transform.position);
    }

    private void OnTruckMoveEnd() {

    }
}
