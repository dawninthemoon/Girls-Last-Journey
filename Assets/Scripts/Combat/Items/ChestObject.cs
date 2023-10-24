using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChestObject : MonoBehaviour {
    public InteractiveEntity Interactive { get; private set; }
    private SpriteRenderer _renderer;

    private void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
        Interactive = GetComponent<InteractiveEntity>();
    }
    
    public void SetSprite(Sprite sprite) {
        _renderer.sprite = sprite;
    }

    public void SetInteraction(ObjectPool<ChestObject> chestPool, UnityAction spawnRewardCallback) {
        Interactive.OnMouseDownEvent.AddListener(spawnRewardCallback);
        Interactive.OnMouseDownEvent.AddListener(() => chestPool.ReturnObject(this));
    }
}
