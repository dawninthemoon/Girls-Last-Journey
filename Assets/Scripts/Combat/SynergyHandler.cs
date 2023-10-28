using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SynergyHandler : MonoBehaviour {
    private int[] _numOfSynergiesArray;
    private Dictionary<SynergyType, SynergyConfig> _synergyConfigDictionary;
    private HashSet<SynergyConfig> _currentSynergySet;
    public float ExtraItemDropPercent {
        get;
        private set;
    }
    public float ExtraGoldGainPercent {
        get;
        private set;
    }

    private void Awake() {
        _currentSynergySet = new HashSet<SynergyConfig>();
        _numOfSynergiesArray = new int[(int)SynergyType.Count];

        ExtraGoldGainPercent = ExtraItemDropPercent = 1f;

        AssetLoader.Instance.LoadAssetsAsync<SynergyConfig>("MemberInfo", (op) => {
            _synergyConfigDictionary = op.Result.ToDictionary(x => x.Type);
        });
    }

    public void ApplyAllSynergyBuffs(KdTree<EntityBase> members, bool remove) {
        foreach (EntityBase member in members) {
            foreach (SynergyConfig synergy in _currentSynergySet) {
                if (remove) {
                    member.BuffControl.RemoveBuff(synergy?.Buff);
                }
                else {
                    member.BuffControl.AddBuff(synergy?.Buff);
                    ExtraItemDropPercent += synergy.ExtraItemDropPercent;
                    ExtraGoldGainPercent += synergy.ExtraGoldGainPercent;
                }
            }
        }

        if (remove) {
            ExtraGoldGainPercent = ExtraItemDropPercent = 1f;
        }
    }

    public void ChangeSynergy(EntityBase ally, bool increase) {
        OnSynergyChanged(ally.Synergy1, increase);
        OnSynergyChanged(ally.Synergy2, increase);
        if (!ally.ExtraSynergy.Equals(SynergyType.None)) {
            OnSynergyChanged(ally.ExtraSynergy, increase);
        }
    }

    private void OnSynergyChanged(SynergyType synergy, bool increase) {
        int synergyIndex = (int)synergy;
        int increaseAmount = increase ? 1 : -1;
        _numOfSynergiesArray[synergyIndex] += increaseAmount;
        _numOfSynergiesArray[synergyIndex] = Mathf.Max(_numOfSynergiesArray[synergyIndex], 0);

        SynergyConfig synergyConfig = _synergyConfigDictionary[synergy];
        bool isSynergyAlreadyExists = _currentSynergySet.TryGetValue(synergyConfig, out SynergyConfig value);

        if ((_numOfSynergiesArray[synergyIndex] > 1) && !isSynergyAlreadyExists) {
            _currentSynergySet.Add(synergyConfig);
        }
        else if ((_numOfSynergiesArray[synergyIndex] < 2) && isSynergyAlreadyExists) {
            _currentSynergySet.Remove(synergyConfig);
        }
    }
}
