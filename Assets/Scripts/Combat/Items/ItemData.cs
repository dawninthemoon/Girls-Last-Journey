using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemData", fileName = "NewItemData")]
public class ItemData : ScriptableObject, IEntityStatus {
    [SerializeField] private Sprite _sprite = null;

    [SerializeField] private int _health;
    [SerializeField] private int _mana;
    [SerializeField] private int _block;
    [SerializeField] private int _attackDamage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private int _moveSpeed;
    [SerializeField] private int _attackRange;
    [SerializeField] private int _healthRegen;
    [SerializeField] private SynergyType _extraSynergy = SynergyType.None;
    
    public int Health { get { return _health; } }
    public int Mana { get { return _mana; } }
    public int Block { get { return _block; } }
    public int AttackDamage { get { return _attackDamage; } }
    public float AttackSpeed { get { return _attackSpeed; } }
    public int MoveSpeed { get { return _moveSpeed; } }
    public int AttackRange { get { return _attackRange; } }
    public int HealthRegen { get { return _healthRegen; } }
    public SynergyType ExtraSynergy { get { return _extraSynergy; } }

    public Sprite Sprite { get { return _sprite; } }
}
