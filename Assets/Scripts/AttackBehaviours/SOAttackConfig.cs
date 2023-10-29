using System.Collections;
using System.Collections.Generic;
using AttackBehaviours;
using AttackBehaviours.Effects;
using UnityEngine;

[System.Serializable]
public struct AttackInfo {
    public LayerMask targetLayerMask;
    public int cost;
    public AttackBehaviour attackBehaviour;
    public AttackEffect[] attackEffects;
    public Sprite weaponSprite;
    public float attackDistance;
    public string soundEffectName;
}


[CreateAssetMenu(menuName = "ScriptableObjects/AttackConfig", fileName = "NewAttackConfig")]
public class SOAttackConfig : ScriptableObject {
    [SerializeField] private AttackInfo _attackConfig;
    [SerializeField] private AttackInfo _skillConfig;
    public AttackInfo AttackConfig {
        get { return _attackConfig; }
    }
    public AttackInfo SkillConfig {
        get { return _skillConfig; }
    }
}
