using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public class Truck : MonoBehaviour {
    [SerializeField] private float _width = 1f;
    [SerializeField] private float _height = 1f;
    [SerializeField] private float _acceleration = 1.2f;
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _knockbackForce = 20f;
    [SerializeField] private float _knockbackMin = 20f;
    [SerializeField] private int _knockbackDamage = 60;
    [SerializeField] private int _upgradedKnockbackDamage = 150;
    [SerializeField] private float _freezeDuration = 0.08f;
    [SerializeField] private DebuffConfig _stunConfig;
    [SerializeField] private DebuffConfig _upgradedStunConfig;

    public float Width {
        get { return _width; }
    }
    public float Height {
        get { return _height; }
    }
    public Vector3 Position {
        get { return transform.position; }
    }

    private Vector3 _direction;
    private float _currentSpeed;
    private float _freezeTimeAgo;
    public static bool IsStunDurationUpgraded;
    public static bool IsDamageUpgraded;

    public void StartMove(Vector3 position, Vector3 direction, float angle, UnityAction<Truck> onTruckmoveEnd) {
        transform.eulerAngles = new Vector3(0f, 0f, angle);
        transform.position = position;
        _acceleration = Mathf.Abs(_acceleration);
        _direction = direction;
        _freezeTimeAgo = 0f;
        
        MoveProgress(direction, onTruckmoveEnd).Forget();
    }

    private async UniTaskVoid MoveProgress(Vector3 direction, UnityAction<Truck> onTruckmoveEnd) {
        float initialSpeed = _currentSpeed = _speed;
        float acc = _acceleration;
        float timeAgo = 0f;
        
        while (timeAgo < 1.5f) {
            if (_freezeTimeAgo > 0f) {
                await UniTask.Delay(System.TimeSpan.FromSeconds(_freezeTimeAgo));
                _freezeTimeAgo = 0f;
            }

            _currentSpeed = initialSpeed + acc * timeAgo;

            Vector3 velocity = direction * _currentSpeed;
            transform.position += velocity * Time.deltaTime;

            timeAgo += Time.deltaTime;
            
            await UniTask.Yield();
        }

        transform.localRotation = Quaternion.identity;
        onTruckmoveEnd.Invoke(this);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag.Equals("Enemy")) {
            Vector2 direction = (other.transform.position - transform.position).normalized;
            float speed = _currentSpeed > 0f ? _currentSpeed : _speed / 10f;
            float finalForce = _knockbackMin + speed * _knockbackForce;
            
            _freezeTimeAgo = _freezeDuration;

            var hitEffect = other.GetComponent<HitEffect>();
            DebuffConfig stunEffect = IsStunDurationUpgraded ? _upgradedStunConfig : _stunConfig;
            int knockbackDamage = IsDamageUpgraded ? _upgradedKnockbackDamage : _knockbackDamage;

            hitEffect.ApplyKnockback(
                direction,
                finalForce, 
                knockbackDamage, 
                _freezeDuration,
                stunEffect
            );
        }
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("Enemy") || other.CompareTag("Ally")) {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            direction.z = 0f;
            other.transform.position += direction * _knockbackForce * 200f * Time.deltaTime;
        }
    }
}
