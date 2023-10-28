using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EntityStatus", fileName = "NewEntityStatus")]
public class EntityStatus : ScriptableObject, IEntityStatus {
    [SerializeField] private int _health;
    [SerializeField] private int _mana;
    [SerializeField] private int _block;
    [SerializeField] private int _attackDamage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private int _moveSpeed;
    [SerializeField] private int _healthRegen;
    [SerializeField] private int _manaRegen;

    public int Health { get { return _health; } }
    public int Mana { get { return _mana; } }
    public int Block { get { return _block; } }
    public int AttackDamage { get { return _attackDamage; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    public int MoveSpeed { get { return _moveSpeed; } }
    public int HealthRegen { get { return _healthRegen; } }
    public int ManaRegen { get { return _manaRegen; } }
}
