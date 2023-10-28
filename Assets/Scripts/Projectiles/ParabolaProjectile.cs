using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AttackBehaviours.Effects;

public class ParabolaProjectile : ProjectileBase {
    [SerializeField] private float _height;
    private Transform _target;
    private Vector3 _startPos;
    private float _timeAgo;

    public override void Initialize(EntityBase caster, EntityBase target, float moveSpeed, IAttackEffect[] effects) {
        _caster = caster;
        _target = target.transform;
        _moveSpeed = moveSpeed;
        _effects = effects;

        _startPos = caster.transform.position;
        _timeAgo = 0f;
    }

    public override void Initialize(EntityBase caster, EntityBase target, float moveSpeed, IAttackEffect[] effects, float angle) {
        Initialize(caster, target, moveSpeed, effects);
    }

    private Vector3 Parabola(Vector3 start, Vector3 end, float height, float t) {
        float distance = Vector3.Distance(start, end);
        t = t / distance * _moveSpeed;

        var mid = Vector3.Lerp(start, end, t);
        return new Vector3(mid.x, GetY(t) + Mathf.Lerp(start.y, end.y, t), mid.z);

        float GetY(float x) {
            return -4f * height * x * x + 4f * height * x; 
        }
    }

    protected override void Update() {
        if (!_target.gameObject.activeSelf) {
            ProjectileSpawner.Instance.RemoveProjectile(this);
            return;
        }

        _timeAgo += Time.deltaTime;

        Vector3 nextPosition = Parabola(_startPos, _target.position, _height, _timeAgo);
        transform.position = nextPosition;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!other.transform.Equals(_target)) return;

        _cachedEntityList.Clear();
        _cachedEntityList.Add(other.GetComponent<EntityBase>());

        foreach (IAttackEffect effect in _effects) {
            effect.ApplyEffect(_caster, _cachedEntityList);
        }

        ProjectileSpawner.Instance.RemoveProjectile(this);
    }
}
