using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class InteractiveEncounter : EncounterEntityBase {
    [SerializeField] private double _waitTimeForExit;
    private bool _isEntrance;
    private bool _doMove;

    public override void OnEncounter(Vector3 initialPosition) {
        base.OnEncounter(initialPosition);
        _isEntrance = _doMove = true;
        WaitForExit().Forget();
    }

    public override void Progress() {
        Move();
    }

    public void Move() {
        if (_doMove) {
            Vector3 targetPosition = _isEntrance ? Vector3.zero : _initialPosition;
            if (Vector3.SqrMagnitude(transform.position - targetPosition) < 1f) {
                _doMove = false;
                if (!_isEntrance) {
                    _onEncounterEnd.Invoke(this);
                }
            }
            Vector3 direction = (targetPosition - transform.position).normalized;
            direction.z = 0f;
            transform.position += direction * _moveSpeed * Time.deltaTime;
        }
    }

    private async UniTaskVoid WaitForExit() {
        await UniTask.Delay(System.TimeSpan.FromSeconds(_waitTimeForExit));

        _doMove = true;
        _isEntrance = false;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (_isEntrance && other.CompareTag(gameObject.tag)) {
            _doMove = false;
        }
    }
}
