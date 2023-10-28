using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttackBehaviours.Effects;

public abstract class ProjectileBase : MonoBehaviour {
    protected float _moveSpeed;
    protected EntityBase _caster;
    protected IAttackEffect[] _effects;
    protected List<EntityBase> _cachedEntityList = new List<EntityBase>();
    public abstract void Initialize(EntityBase caster, EntityBase target, float moveSpeed, IAttackEffect[] effects);
    public abstract void Initialize(EntityBase caster, EntityBase target, float moveSpeed, IAttackEffect[] effects, float angle);
    protected abstract void Update();
}
