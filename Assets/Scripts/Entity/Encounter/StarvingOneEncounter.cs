using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StarvingOneEncounter : EncounterEntityBase {
    private int _numOfClearedItems;
    private System.Func<Vector3, ItemObject> _targetDetectCallback;
    private System.Action<ItemObject> _itemReturnCallback;
    public static readonly int TargetClearCount = 5;
    public void SetCallbackSettings
        (System.Func<Vector3, ItemObject> targetDetectCallback,
         System.Action<ItemObject> itemReturnCallback) {
        _targetDetectCallback = targetDetectCallback;
        _itemReturnCallback = itemReturnCallback;
    }

    public override void OnEncounter(Vector3 initialPosition) {
        base.OnEncounter(initialPosition);
        _numOfClearedItems = 0;
    }

    public override void Progress() {
        Move();
    }

    public void Move() {
        Vector3? targetPosition = null;
        if (_numOfClearedItems >= TargetClearCount) {
            targetPosition = _initialPosition;
        }
        else {
            ItemObject target = _targetDetectCallback.Invoke(transform.position);
            targetPosition = target?.transform.position;
        }

        if (targetPosition != null) {
            Vector3 direction = (targetPosition.Value - transform.position).normalized;
            direction.z = 0f;
            transform.position += direction * _moveSpeed * Time.deltaTime;

            if (_numOfClearedItems >= TargetClearCount 
                && Vector3.SqrMagnitude(transform.position - targetPosition.Value) < 1f) {
                _onEncounterEnd.Invoke(this);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag(CombatReward.ItemObjectTag)) {
            ItemObject itemObj = other.GetComponent<ItemObject>();
            _itemReturnCallback.Invoke(itemObj);
            ++_numOfClearedItems;
        }
    }
}
