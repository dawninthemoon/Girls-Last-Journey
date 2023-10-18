using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class CombatMain : MonoBehaviour {
    [SerializeField] private float _targetDetectionDelay = 0.05f;
    [SerializeField] private MemberHandler _memberHandler;
    [SerializeField] private EnemyHandler _enemyHandler;
    [SerializeField] private WaveHandler _waveHandler;
    [SerializeField] private TruckHandler _truckHandler;

    private async UniTaskVoid Start() {
        var soundManager = SoundManager.Instance;
        var projectileSpawner = ProjectileSpawner.Instance;

        // 나중에 로드되면 시작하게끔 수정
        await UniTask.Delay(System.TimeSpan.FromSeconds(3f));

        CombatMap.SetMapView(Vector2.zero);

        _memberHandler.InitalizeMember();
        _waveHandler.StartNewWave();
        TargetDetectProgress().Forget();
    }
    
    private async UniTaskVoid TargetDetectProgress() {
        while (true) {
            _memberHandler.Progress(_enemyHandler);
            _enemyHandler.Progress(_memberHandler.Members);

            await UniTask.Delay(System.TimeSpan.FromSeconds(_targetDetectionDelay));
        }
    }

    public void SpawnTruck() {
        EntityBase target = _enemyHandler.GetRandomEnemy();
        if (target != null) {
            _truckHandler.SpawnTruck(target.transform.position);
        }
    }
}
