using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

public class ItemObject : MonoBehaviour {
    public EntityItem ItemData { get; set; }
    public InteractiveEntity Interactive { get; private set; }
    private SpriteRenderer _renderer;

    private void Awake() {
        _renderer = GetComponent<SpriteRenderer>();
        Interactive = GetComponent<InteractiveEntity>();
    }

    public void SetSprite(Sprite sprite) {
        _renderer.sprite = sprite;
    }

    public void SetInteraction(ObjectPool<ItemObject> itemPool) {
        Interactive.ClearAllEvents();

        Interactive.OnMouseDragEvent.AddListener(() => {
            Vector2 mousePos = ExMouse.GetMouseWorldPosition();
            transform.position = mousePos;
        });
        Interactive.OnMouseExitEvent.AddListener(() => {
            int layerMask = LayerMask.NameToLayer("Ally");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hit = Physics2D.GetRayIntersection(ray, 100f, (1 << layerMask));
            if (hit.collider != null) {
                EntityBase entity = hit.collider.GetComponent<EntityBase>();
                entity.EquipItem(ItemData);

                itemPool.ReturnObject(this);
            }
        });
    }
}