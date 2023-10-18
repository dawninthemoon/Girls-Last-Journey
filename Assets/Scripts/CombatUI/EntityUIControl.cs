using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EntityUIControl : MonoBehaviour {
    [SerializeField] private Transform _hpBarTransform = null;
    [SerializeField] private Transform _mpBarTransform = null;
    [SerializeField] private SpriteRenderer _equipedItemRenderer;
    private TMP_Text _moraleText;

    public void UpdateHealthBar(int health, int maxHealth) {
        var hpBarScale = _hpBarTransform.localScale;
        hpBarScale.x = (float)health / maxHealth * 0.7f;
        _hpBarTransform.localScale = hpBarScale;
    }

    public void UpdateManaBar(int mana, int maxMana) {
        if (Mathf.Approximately(maxMana, default(float))) {
            return;
        }
        var mpBarScale = _mpBarTransform.localScale;
        mpBarScale.x = (float)mana / maxMana * 0.7f;
        _mpBarTransform.localScale = mpBarScale;
    }

    public void ShowEquipedItem(Sprite sprite) {
        if (_equipedItemRenderer) {
            _equipedItemRenderer.sprite = sprite;
        }
    }
}
