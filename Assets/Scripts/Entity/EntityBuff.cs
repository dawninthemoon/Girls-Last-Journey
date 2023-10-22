using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class EntityBuff {
    private MonoBehaviour _executer;
    private EntityDecorator _status;
    private Dictionary<string, int> _currentDebuffSet;

    public EntityBuff(MonoBehaviour executer, EntityDecorator status) {
        _currentDebuffSet = new Dictionary<string, int>();
        _executer = executer;
        _status = status;
    }

    public void AddBuff(BuffConfig buff) {
        _status.AddBuff(buff);
    }

    public void RemoveBuff(BuffConfig buff) {
        _status.RemoveBuff(buff);
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
        if (IsDebuffExists(debuffName)) {
            ++_currentDebuffSet[debuffName];
        }
        else {
            _currentDebuffSet.Add(debuffName, 1);
        }

        await UniTask.Delay(TimeSpan.FromSeconds(duration));

        if (IsDebuffExists(debuffName)) {
            --_currentDebuffSet[debuffName];
        }
    }

    public bool IsDebuffExists(string debuffName) {
        if (_currentDebuffSet.TryGetValue(debuffName, out int count)) {
            return true;
        }
        return false;
    }
}
