using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EntityInfo {
    public string entityID = null;

    public float bodyRadius = 20f;
    public Sprite bodySprite = null;
    public RuntimeAnimatorController animatorController = null;

    public EntityStatus status;
    public SynergyType synergy1;
    public SynergyType synergy2;

    public SOAttackConfig attackConfig;

    public Vector2 bulletOffset;
}

[CreateAssetMenu(menuName = "ScriptableObjects/EntityConfig", fileName = "NewEntityConfig")]
public class EntityConfig : ScriptableObject {
    [SerializeField] private EntityInfo _info;
    public EntityInfo Info {
        get { return _info; }
    }
}
