using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldHandler : MonoBehaviour {
    [SerializeField] private SynergyHandler _synergyHandler;
    [SerializeField] private TMP_Text _goldUIText;
    [SerializeField] private int _initialGold;
    private int _currentGold;
    public int CurrentGold {
        get { 
            return _currentGold; 
        }
        private set {
            _currentGold = value;
            OnGoldChanged();
        }
    }

    private void Start() {
        CurrentGold = _initialGold;
    }

    public void GainGold(int amount) {
        amount = Mathf.FloorToInt(amount * _synergyHandler.ExtraGoldGainPercent);
        CurrentGold += amount;
    }

    public bool TryPayGold(int amount) {
        bool result = false;
        if (CurrentGold >= amount) {
            result = true;
            CurrentGold -= amount;
        }
        return result;
    }

    private void OnGoldChanged() {
        _goldUIText.text = CurrentGold.ToString();
    }
}
