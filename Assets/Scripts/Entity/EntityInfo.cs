using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/EntityInfo", fileName = "NewEntityInfo")]
public class EntityInfo : ScriptableObject, IEntityStatus {
    [SerializeField] private string _entityID = null;

    [SerializeField] private float _bodyRadius = 20f;
    [SerializeField] private Sprite _bodySprite = null;
    [SerializeField] private Sprite _weaponSprite = null;
    [SerializeField] private RuntimeAnimatorController _animatorController = null;

    [SerializeField] private int _health = 0;
    [SerializeField] private int _mana = 0;
    [SerializeField] private int _block = 0;
    [SerializeField] private int _attackDamage = 0;
    [SerializeField] private float _attackSpeed = 0;
    [SerializeField] private int _moveSpeed = 0;
    [SerializeField] private int _attackRange = 0;
    [SerializeField] private SynergyType _synergy1;
    [SerializeField] private SynergyType _synergy2;

    [SerializeField] private SOAttackConfig _attackConfig;
    [SerializeField] private SOAttackConfig _skillConfig;

    [SerializeField] private Vector2 _bulletOffset;

    public string EntityID { get { return _entityID; } }

    public float BodyRadius { get { return _bodyRadius; } }
    public Sprite BodySprite { get { return _bodySprite; } set { _bodySprite = value; } }
    public Sprite WeaponSprite { get { return _weaponSprite; } }
    public RuntimeAnimatorController AnimatorController { get { return _animatorController; } }
    
    public int Health { get { return _health; } set { _health = value; } }
    public int Mana { get { return _mana; } set { _mana = value; } }
    public int Block { get { return _block; } set {_block = value; } }
    public int AttackDamage { get { return _attackDamage; } set { _attackDamage = value; } }
    public float AttackSpeed { get { return _attackSpeed; } set { _attackSpeed = value; } }
    public int MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public int AttackRange { get { return _attackRange; } set { _attackRange = value; } }

    public SynergyType Synergy1 {
        get { return _synergy1; }
        set { _synergy1 = value; }
    }
    public SynergyType Synergy2 {
        get { return _synergy2; }
        set { _synergy2 = value; }
    }

    public SOAttackConfig EntityAttackConfig { get { return _attackConfig; } set { _attackConfig = value; }}
    public SOAttackConfig EntitySkillConfig { get { return _skillConfig; } set { _skillConfig = value; } }

    public Vector2 BulletOffset { get { return _bulletOffset; } }

    public static EntityInfo CreateWithStatus(EntityStatus status) {
        EntityInfo info = CreateInstance("EntityInfo") as EntityInfo;
        info.Health = status.Health;
        info.Mana = status.Mana;
        info.Block = status.Block;
        info.AttackDamage = status.AttackDamage;
        info.AttackSpeed = status.AttackSpeed;
        info.MoveSpeed = status.MoveSpeed;
        info.AttackRange = status.AttackRange;
        return info;
    }
}
