using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AttackBehaviours.Effects {
    [CreateAssetMenu(fileName = "HealEffect", menuName = "ScriptableObjects/AttackEffects/HealEffect")]
    public class HealEffect : AttackEffect {
        public override void ApplyEffect(EntityBase caster, List<EntityBase> targets) {
            foreach (EntityBase target in targets) {
                target.Heal(caster.AttackDamage);
            }
        }
    }
}