using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Defective.JSON;
using RieslingUtils;

public class SynergyHandler : MonoBehaviour {
    [SerializeField] private GameObject _popupWindowPrefab;
    [SerializeField] private DescriptionUI _synergyIconPrefab;
    [SerializeField] private Transform _synergyCanvas;
    [SerializeField] private Transform _synergyUIParent;
    private int[] _numOfSynergiesArray;
    private Dictionary<SynergyType, SynergyConfig> _synergyConfigDictionary;
    private Dictionary<SynergyType, DescriptionUI> _synergyIconDictionary;
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
        _synergyIconDictionary = new Dictionary<SynergyType, DescriptionUI>();
        _numOfSynergiesArray = new int[(int)SynergyType.Count];

        ExtraGoldGainPercent = ExtraItemDropPercent = 1f;

        AssetLoader.Instance.LoadAssetsAsync<SynergyConfig>("MemberInfo", (op) => {
            _synergyConfigDictionary = op.Result.ToDictionary(x => x.Type);
        });
        InitializeSynergyDescriptionInfo();
    }

    private void InitializeSynergyDescriptionInfo() {
        AssetLoader.Instance.LoadAssetAsync<TextAsset>("SynergyDescriptionInfo", (op) => {
            JSONObject jsonObject = new JSONObject(op.Result.ToString());
            foreach (JSONObject synergyDescriptionData in jsonObject.list) {
                string key = synergyDescriptionData.GetField("Key").stringValue;
                string name = synergyDescriptionData.GetField("Name").stringValue;
                string desc = synergyDescriptionData.GetField("Description").stringValue;
                SynergyType type = ExEnum.Parse<SynergyType>(key);

                DescriptionUI synergyIcon = Instantiate(_synergyIconPrefab, _synergyUIParent);
                GameObject popupWindow = Instantiate(_popupWindowPrefab, _synergyCanvas);
                
                synergyIcon.Initialize(popupWindow, desc);
                synergyIcon.gameObject.SetActive(false);

                _synergyIconDictionary.Add(type, synergyIcon);
            }
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
            ToggleSynergyUI(synergy, true);
        }
        else if ((_numOfSynergiesArray[synergyIndex] < 2) && isSynergyAlreadyExists) {
            _currentSynergySet.Remove(synergyConfig);
            ToggleSynergyUI(synergy, false);
        }
    }

    // TODO: 최적화
    private void ToggleSynergyUI(SynergyType synergy, bool isAdded) {
        _synergyIconDictionary[synergy].gameObject.SetActive(isAdded);
    }
}
