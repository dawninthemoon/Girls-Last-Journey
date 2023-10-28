using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttackBehaviours.Effects {
    [CreateAssetMenu(fileName = "KnockbackEffect", menuName = "ScriptableObjects/AttackEffects/KnockbackEffect")]
    public class KnockbackEffect : AttackEffect {
        public override void ApplyEffect(EntityBase caster, List<EntityBase> targets) {
            foreach (EntityBase target in targets) {
                HitEffect hitEffect = target.GetComponent<HitEffect>();
                Vector2 direction = (target.transform.position - caster.transform.position).normalized;
                hitEffect.ApplyKnockback(direction, caster.AttackDamage, 0, 0.025f);
            }
        }
    }
}