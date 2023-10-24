using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public void SetInteraction(ObjectPool<ItemObject> itemPool, bool canEquip) {
        Interactive.ClearAllEvents();

        Interactive.OnMouseDragEvent.AddListener(() => {
            Vector2 mousePos = ExMouse.GetMouseWorldPosition();
            transform.position = mousePos;
        });

        Interactive.OnMouseUpEvent.AddListener(() => {
            var collider = GetOverlapedWithMouse(EncounterHandler.EncountersLabel);
            if (collider != null) {
                var collector = collider.GetComponent<EntityCollector>();
                itemPool.ReturnObject(this);
                collector.OnInteraction();
            }
        });

        if (canEquip) {
            Interactive.OnMouseUpEvent.AddListener(() => {
                var collider = GetOverlapedWithMouse(MemberHandler.MemberTagName);
                if (collider != null) {
                    EntityBase entity = collider.GetComponent<EntityBase>();
                    entity.EquipItem(ItemData);
                    itemPool.ReturnObject(this);
                }
            });
        }

        Collider2D GetOverlapedWithMouse(string layerName) {
            int layerMask = LayerMask.NameToLayer(layerName);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 100f, (1 << layerMask));
            return hit.collider;
        }
    }
}