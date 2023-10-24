using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldHandler : MonoBehaviour {
    [SerializeField] private int _initialGold;
    public int CurrentGold {
        get;
        private set;
    }

    private void Start() {
        CurrentGold = _initialGold;
    }

    public void GainGold(int amount) {
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
}
