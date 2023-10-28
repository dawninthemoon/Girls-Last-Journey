using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BuffInfo {
    public string buffName;
    public float buffDuration;
    public int health;
    public int mana;
    public int block;
    [Tooltip("Applied by Percent")] public float blockPercent;
    public int attackDamage;
    [Tooltip("Applied by Percent")] public float attackDamagePercent;
    [Tooltip("Applied by Percent")] public float attackSpeedPercent;
    public int moveSpeed;
    [Tooltip("Applied by Percent")] public float moveSpeedPercent;
    public int healthRegen;
    [Tooltip("Applied by Percent")] public float healthRegenPercent;
    public int manaRegen;
    [Tooltip("Applied by Percent")] public float manaRegenPercent;
    [Tooltip("Applied by Percent")] public float aimingEfficiency;
    public int extraLevel;
}

[CreateAssetMenu(fileName = "NewBuffConfig", menuName = "ScriptableObjects/BuffConfig")]
public class BuffConfig : ScriptableObject, IEntityStatus {
    [SerializeField] private BuffInfo _buffInfo;
    public BuffInfo Info {
        get { return _buffInfo; }
    }
    public string Name { get { return _buffInfo.buffName; } }
    public int Health { get { return _buffInfo.health; } }
    public int Mana { get { return _buffInfo.mana; } }
    public int Block { get { return _buffInfo.block; } }
    public float BlockPercent { get { return _buffInfo.blockPercent; } }
    public int AttackDamage { get { return _buffInfo.attackDamage; } }
    public float AttackDamagePercent { get { return _buffInfo.attackDamagePercent; } }
    public float AttackSpeed { get { return _buffInfo.attackSpeedPercent; } }
    public int MoveSpeed { get { return _buffInfo.moveSpeed; } }
    public float MoveSpeedPercent { get { return _buffInfo.moveSpeed; } }
    public int HealthRegen { get { return _buffInfo.healthRegen; } }
    public int ManaRegen { get { return _buffInfo.manaRegen; }}
    public float ManaRegenPercent { get { return _buffInfo.manaRegenPercent; } }
    public float HealthRegenPercent { get { return _buffInfo.healthRegenPercent; } }
    public float AimingEfficiency { get { return _buffInfo.aimingEfficiency; } }
    public int ExtraLevel { get { return _buffInfo.extraLevel; } }
}