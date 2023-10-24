using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class WaveHandler : MonoBehaviour {
    [SerializeField] private float _waveTimeLimit = 30f;
    [SerializeField] private Button _waveTimerButton;
    [SerializeField] private EnemyHandler _enemyHandler;
    [SerializeField] private EncounterHandler _encounterHandler;
    private ExTimeCounter _waveTimeCounter;
    private EntitySpawner _enemySpawner;
    private static readonly string NextWaveTimerKey = "NextWaveTime";
    public int CurrentWave {
        get;
        private set;
    }
    public float NextWaveTime {
        get {
            if (!IsWaveStarted) {
                return 0;
            }
            float timeLimit = _waveTimeCounter.GetTimeLimit(NextWaveTimerKey);
            float curr = _waveTimeCounter.GetCurrentTime(NextWaveTimerKey);
            return Mathf.Max(0f, timeLimit - curr);
        }
    }
    public bool IsWaveStarted {
        get {
            return _waveTimeCounter.Contains(NextWaveTimerKey);
        }
    }

    private void Awake() {
        _waveTimeCounter = new ExTimeCounter();
    }

    private void Start() {
        var stream = _waveTimerButton.OnClickAsObservable();
        stream.Buffer(stream.ThrottleFirst(System.TimeSpan.FromSeconds(0.5)))
            .Subscribe(_ => StartNewWave());
    }

    private void Update() {
        if (IsWaveStarted) {
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

        if (CurrentWave % 5 == 0) {
            _encounterHandler.SpawnRandomEncounter();
        }
    }
}
