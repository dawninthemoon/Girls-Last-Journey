using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EncounterEntityBase : MonoBehaviour {
    [SerializeField] protected int _moveSpeed;
    private UnityAction _onInteraction;

    public virtual void Initialize(UnityAction onInteraction) {
        _onInteraction = onInteraction;
    }

    public abstract void Progress();

    public virtual void OnInteraction() {
        _onInteraction?.Invoke();
    }
}
