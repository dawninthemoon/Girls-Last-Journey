using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveEncounter : EncounterEntityBase {
    private bool _isEntrance;
    public override void Progress() {
        Move();
    }
    public void Move() {
        Vector3 targetPosition = _isEntrance ? Vector3.zero : Vector3.zero;

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * _moveSpeed * Time.deltaTime;
    }
}
