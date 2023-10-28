using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityDecorator : IEntityStatus {
    private List<BuffConfig> _buffList;
    public EntityInfo Info {
        get;
        private set;
    }
    public ItemData Item {
        get;
        set;
    }
    public int Level {
        get;
        set;
    }

    public EntityDecorator(EntityInfo entityInfo) {
        _buffList = new List<BuffConfig>();
        Info = entityInfo;
    }

    public void Initialize(EntityInfo entityInfo) {
        Info = entityInfo;
        _buffList.Clear();
    }

    public void AddBuff(BuffConfig config) {
        _buffList.Add(config);
    }

    public void RemoveBuff(BuffConfig config) {
        _buffList.Remove(config);
    }

    public int Health { 
        get {
            int finalHealth = Info.status.Health;

            if (Item)
                finalHealth += Item.Health;
            foreach (IEntityStatus buff in _buffList) {
                finalHealth += buff.Health;
            }
            return finalHealth;
        }
    }
    public int Mana { 
        get {
            int finalMana = Info.status.Mana;

            if (Item)
                finalMana += Item.Mana;
            foreach (IEntityStatus buff in _buffList) {
                finalMana += buff.Mana;
            }
            return finalMana;
        }
    }
    public int Block {
        get {
            int finalBlock = Info.status.Block;
            float blockMultiplier = 0f;

            if (Item)
                finalBlock += Item.Block;
            foreach (BuffConfig buff in _buffList) {
                finalBlock += buff.Block;
            }
            foreach (BuffConfig buff in _buffList) {
                blockMultiplier += buff.BlockPercent;
            }
            finalBlock += Mathf.FloorToInt(finalBlock * blockMultiplier);
            return finalBlock;
        }
    }
    public int AttackDamage { 
        get {
            int finalDamage = Info.status.AttackDamage;
            float damageMultiplier = 0f;

            if (Item)
                finalDamage += Item.AttackDamage;
            foreach (IEntityStatus buff in _buffList) {
                finalDamage += buff.AttackDamage;
            }
            foreach (BuffConfig buff in _buffList) {
                damageMultiplier += buff.AttackDamagePercent;
            }
            finalDamage += Mathf.FloorToInt(finalDamage * damageMultiplier);
            return finalDamage;
        }
    }
    public float AttackSpeed { 
        get {
            float finalAttackSpeed = Info.status.AttackSpeed;
            float attackSpeedMultiplier = 0f;

            if (Item)
                attackSpeedMultiplier += Item.AttackSpeed;
            foreach (IEntityStatus buff in _buffList) {
                attackSpeedMultiplier += buff.AttackSpeed;
            }
            finalAttackSpeed += Info.status.AttackSpeed * attackSpeedMultiplier;
            return finalAttackSpeed;
        }
    }
    public int MoveSpeed { 
        get {
            int finalMoveSpeed = Info.status.MoveSpeed;

            if (Item)
                finalMoveSpeed += Item.MoveSpeed;
            foreach (IEntityStatus buff in _buffList) {
                finalMoveSpeed += buff.MoveSpeed;
            }
            return finalMoveSpeed;
        }
    }

    public int HealthRegen {
        get { 
            int finalHealthRegen = Info.status.HealthRegen;
            float healthRegenMultiplier = 0f;

            foreach (BuffConfig buff in _buffList) {
                finalHealthRegen += buff.HealthRegen;
            }
            foreach (BuffConfig buff in _buffList) {
                healthRegenMultiplier += buff.HealthRegenPercent;
            }
            finalHealthRegen += Mathf.FloorToInt(finalHealthRegen * healthRegenMultiplier);

            return finalHealthRegen;
        }
    }

    public int ManaRegen {
        get { 
            int finalManaRegen = Info.status.ManaRegen;
            float manaRegenMultiplier = 0f;

            foreach (BuffConfig buff in _buffList) {
                finalManaRegen += buff.ManaRegen;
            }
            foreach (BuffConfig buff in _buffList) {
                manaRegenMultiplier += buff.ManaRegenPercent;
            }

            finalManaRegen += Mathf.FloorToInt(finalManaRegen * manaRegenMultiplier);
            return finalManaRegen;
        }
    }


    public float AimingEfficiency {
        get { 
            float finalAiming = 0f;
            foreach (BuffConfig buff in _buffList) {
                finalAiming += buff.AimingEfficiency;
            }
            return finalAiming;
        }
    }

    public SynergyType ExtraSynergy {
        get {
            SynergyType extraSynergy = SynergyType.None;
            if (Item && !Item.ExtraSynergy.Equals(SynergyType.None)) {
                extraSynergy = Item.ExtraSynergy;
            }
            return extraSynergy;
        }
    }
}
