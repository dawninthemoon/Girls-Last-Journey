using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "ScriptableObjects/ChestData", fileName = "NewChestData")]
public class ChestData : ScriptableObject {
    public enum Type {
        Normal,
        Desert,
        Corpse,
        Purse,
        Vehicle
    }
    [SerializeField] private Sprite _sprite;
    [SerializeField] private Type _type;

    public Sprite ChestSprite {
        get { return _sprite; }
    }
    public Type ChestType {
        get { return _type; }
    }
}
