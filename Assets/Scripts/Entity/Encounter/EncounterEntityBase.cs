using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class EncounterEntityBase : MonoBehaviour {
    public enum Type {
        Collector,
        Vampire,
        HumanTrafficker,
        StarvingOne
    }
    [SerializeField] protected int _moveSpeed;
    [SerializeField] private Type _encounterType;
    private UnityAction _onInteraction;
    public Type EncounterType {
        get { return _encounterType; }
    }

    public virtual void Initialize(UnityAction onInteraction) {
        _onInteraction = onInteraction;
    }

    public abstract void Progress();

    public virtual void OnInteraction() {
        _onInteraction?.Invoke();
    }
}
