using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AttackConfig", fileName = "NewAttackConfig")]
public class SOAttackConfig : ScriptableObject {
    [SerializeField] private AttackConfig _config;
    public AttackConfig Config {
        get { return _config; }
    }
}
