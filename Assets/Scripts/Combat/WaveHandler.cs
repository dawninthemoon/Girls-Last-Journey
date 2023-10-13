using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveHandler : MonoBehaviour {
    [SerializeField] private float _waveTimeLimit = 30f;
    [SerializeField] private EnemyHandler _enemyHandler;
    private ExTimeCounter _waveTimeCounter;
    private EntitySpawner _enemySpawner;
    private static readonly string NextWaveTimerKey = "NextWaveTime";
    public int CurrentWave {
        get;
        private set;
    }
    public float NextWaveTime {
        get {
            if (!_waveTimeCounter.Contains(NextWaveTimerKey)) {
                return 0;
            }
            float timeLimit = _waveTimeCounter.GetTimeLimit(NextWaveTimerKey);
            float curr = _waveTimeCounter.GetCurrentTime(NextWaveTimerKey);
            return Mathf.Max(0f, timeLimit - curr);
        }
    }

    private void Awake() {
        _waveTimeCounter = new ExTimeCounter();
    }

    private void Update() {
        if (_waveTimeCounter.Contains(NextWaveTimerKey)) {
            _waveTimeCounter.IncreaseTimer(NextWaveTimerKey, out var limit, GameConfig.GameSpeed);
            if (limit) {
                StartNewWave();
            }
        }
    }

    public void StartNewWave() {
        ++CurrentWave;
        _waveTimeCounter.InitTimer(NextWaveTimerKey, 0f, _waveTimeLimit);
        _enemyHandler.SpawnEnemies(CurrentWave, null);
    }
}
