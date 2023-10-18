using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class EntityBuff {
    private MonoBehaviour _executer;
    private EntityDecorator _status;
    private HashSet<string> _currentDebuffSet;

    public EntityBuff(MonoBehaviour executer, EntityDecorator status) {
        _currentDebuffSet = new HashSet<string>();
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
        _currentDebuffSet.Add(debuffName);

        await UniTask.Delay(TimeSpan.FromSeconds(duration));

        _currentDebuffSet.Remove(debuffName);
    }

    public bool IsDebuffExists(string debuffName) {
        if (_currentDebuffSet.TryGetValue(debuffName, out string value)) {
            return true;
        }
        return false;
    }
}
