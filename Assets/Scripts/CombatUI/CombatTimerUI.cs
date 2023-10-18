using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatTimerUI : MonoBehaviour {
    [SerializeField] private Canvas _combatUICanvas = null;
    [SerializeField] private Button _timerButton = null;
    [SerializeField] private WaveHandler _waveHandler;
    private TextMeshProUGUI _timerText;

    void Awake() {
        _combatUICanvas.worldCamera = Camera.main;
        _timerText = _timerButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update() {
        if (_waveHandler.IsWaveStarted) {
            _timerText.text = Mathf.Floor(_waveHandler.NextWaveTime).ToString();
        }
    }
}
