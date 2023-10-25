using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveEncounter : EncounterEntityBase {
    private bool _isEntrance;
    private bool _doMove;

    public override void OnEncounter(Vector3 initialPosition) {
        base.OnEncounter(initialPosition);
        _isEntrance = _doMove = true;
    }

    public override void Progress() {
        Move();
    }

    public void Move() {
        if (_doMove) {
            Vector3 targetPosition = _isEntrance ? Vector3.zero : _initialPosition;
            Vector3 direction = (targetPosition - transform.position).normalized;
            transform.position += direction * _moveSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(gameObject.tag)) {
            _doMove = false;
        }
    }
}
