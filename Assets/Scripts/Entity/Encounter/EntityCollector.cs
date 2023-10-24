using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityCollector : MonoBehaviour {
    [SerializeField] private int _moveSpeed;
    private bool _isEntrance;
    private UnityAction _onItemSold;

    private void Awake() {

    }

    private void Start() {
        
    }

    public void Initialize(UnityAction onItemSold) {

    }
    
    private void Update() {
        Move();
    }

    public void Move() {
        Vector3 targetPosition = _isEntrance ? Vector3.zero : Vector3.zero;

        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * _moveSpeed * Time.deltaTime;
    }

    public void OnItemSold() {
        _onItemSold.Invoke();
    }
}
