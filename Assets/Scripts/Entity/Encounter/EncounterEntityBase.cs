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
    protected Vector3 _initialPosition;
    private UnityAction _onInteraction;
    protected UnityAction<EncounterEntityBase> _onEncounterEnd;
    public Type EncounterType {
        get { return _encounterType; }
    }

    public virtual void Initialize(UnityAction onInteraction, UnityAction<EncounterEntityBase> onEncounterEnd) {
        _onInteraction = onInteraction;
        _onEncounterEnd = onEncounterEnd;
    }

    public virtual void OnEncounter(Vector3 initialPosition) {
        transform.position = _initialPosition = initialPosition;
    }

    public abstract void Progress();

    public virtual void OnInteraction() {
        _onInteraction?.Invoke();
    }
}
