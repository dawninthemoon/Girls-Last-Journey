using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class EntityBuff {
    private MonoBehaviour _executer;
    private EntityDecorator _status;
    private Dictionary<string, int> _currentDebuffSet;

    public EntityBuff(MonoBehaviour executer) {
        _currentDebuffSet = new Dictionary<string, int>();
        _executer = executer;
    }

    public void Initialize(EntityDecorator status) {
        _status = status;
    }

    public void AddBuff(BuffConfig buff) {
        if (buff != null) {
            _status.AddBuff(buff);
        }
    }

    public void RemoveBuff(BuffConfig buff) {
        if (buff != null) {
            _status.RemoveBuff(buff);
        }
    }

    public void AddBuffWithDuration(BuffConfig buffConfig) {
        AddBuff(buffConfig, buffConfig.Info.buffDuration).Forget();
    }

    public void StartAddDebuff(DebuffConfig debuffConfig) {
        DebuffInfo info = debuffConfig.Info;

        if (info.stun.value) {
            AddDebuff(nameof(info.stun), info.stun.durtaion).Forget();
        }
    }

    private async UniTaskVoid AddBuff(BuffConfig buff, float duration) {
        AddBuff(buff);

        await UniTask.Delay(TimeSpan.FromSeconds(duration));

        RemoveBuff(buff);
    }

    private async UniTaskVoid AddDebuff(string debuffName, float duration) {
        int debuffCount;
        if (_currentDebuffSet.TryGetValue(debuffName, out debuffCount)) {
            ++_currentDebuffSet[debuffName];
        }
        else {
            _currentDebuffSet.Add(debuffName, 1);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(duration));

        if (_currentDebuffSet.TryGetValue(debuffName, out debuffCount)) {
            --_currentDebuffSet[debuffName];
        }
    }

    public bool IsDebuffExists(string debuffName) {
        bool result = false;
        if (_currentDebuffSet.TryGetValue(debuffName, out int debuffCount)) {
            if (debuffCount > 0) {
                result = true;
            }
        }
        return result;
    }
}
