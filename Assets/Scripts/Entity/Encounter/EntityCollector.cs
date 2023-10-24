using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityCollector : EncounterEntityBase {
    private bool _isEntrance;

    private void Awake() {

    }

    private void Start() {
        
    }

    public override void Progress() {
        Move();
    }

    public void Move() {
        Vector3 targetPosition = _isEntrance ? Vector3.zero : Vector3.zero;

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * _moveSpeed * Time.deltaTime;
    }
}
