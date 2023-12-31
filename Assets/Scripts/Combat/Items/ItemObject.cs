using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class ItemObject : MonoBehaviour {
    public ItemData ItemData { get; set; }
    public InteractiveEntity Interactive { get; private set; }
    private SpriteRenderer _renderer;

    private void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
        Interactive = GetComponent<InteractiveEntity>();
    }

    public void SetSprite(Sprite sprite) {
        _renderer.sprite = sprite;
    }

    public void SetInteraction(UnityAction<ItemObject> itemReturnCallback, bool canEquip) {
        Interactive.ClearAllEvents();

        Interactive.OnMouseDragEvent.AddListener(() => {
            Vector2 mousePos = ExMouse.GetMouseWorldPosition();
            transform.position = mousePos;
        });

        Interactive.OnMouseUpEvent.AddListener(() => {
            var collider = ExMouse.GetOverlapedCollider(EncounterHandler.EncountersLabel);
            if (collider != null) {
                var collector = collider.GetComponent<InteractiveEncounter>();
                if (collector.EncounterType.Equals(EncounterEntityBase.Type.Collector)) {
                    itemReturnCallback.Invoke(this);
                    collector.OnInteraction();
                }
            }
        });

        if (canEquip) {
            Interactive.OnMouseUpEvent.AddListener(() => {
                var collider = ExMouse.GetOverlapedCollider(MemberHandler.MemberTagName);
                if (collider != null) {
                    EntityBase entity = collider.GetComponent<EntityBase>();
                    if (!entity.HasItem) {
                        entity.EquipItem(ItemData);
                        itemReturnCallback.Invoke(this);
                    }
                }
            });
        }
    }
}