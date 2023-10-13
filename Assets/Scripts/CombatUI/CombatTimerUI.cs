using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatTimerUI : MonoBehaviour {
    [SerializeField] private Canvas _combatUICanvas = null;
    [SerializeField] private Button _timerButton = null;
    private TextMeshProUGUI _timerText;

    void Awake() {
        _combatUICanvas.worldCamera = Camera.main;
        _timerText = _timerButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update() {
        //if (_combatScene.IsCombatStarted) {
        //    _timerText.text = Mathf.Floor(_combatScene.NextWaveTime).ToString();
        //}
    }
}
