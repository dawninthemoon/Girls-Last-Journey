using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

public class HitEffect : MonoBehaviour {
    [SerializeField] private SpriteRenderer _bodyRenderer = null;
    [SerializeField] private float _knockbackDuration = 0.5f;
    [SerializeField] private float _friction = 1f;
    private EntityBase _entityBase;
    private static readonly string FlashAmountKey = "_FlashAmount";

    private void Start() {
        _entityBase = GetComponent<EntityBase>();
    }

    public void ApplyKnockback(Vector2 direction, float force, int damage, float freezeDuration, DebuffConfig debuff) {
        ApplyHitEffect(direction, force, damage, freezeDuration, debuff).Forget();
    }

    private async UniTaskVoid ApplyHitEffect(Vector2 direction, float force, int damage, float freezeDuration, DebuffConfig debuff) {
        _bodyRenderer.material.SetFloat(FlashAmountKey, 1f);

        await UniTask.Delay(System.TimeSpan.FromSeconds(freezeDuration));

        _bodyRenderer.material.SetFloat(FlashAmountKey, 0f);

        _entityBase.ReceiveDamage(damage);
        _entityBase.BuffControl.StartAddDebuff(debuff);

        float timeAgo = 0f;
        while (timeAgo < _knockbackDuration) {
            Vector3 knockbackAmount = direction * Mathf.Max(0f, force - _friction * timeAgo);
            transform.position += knockbackAmount * Time.deltaTime;

            await UniTask.Yield();

            timeAgo += Time.deltaTime;
        }
    }
}
