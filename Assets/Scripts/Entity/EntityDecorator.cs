using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RieslingUtils;

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
    public int FinalLevel {
        get {
            int finalLevel = Level;
            foreach (BuffConfig buff in _buffList) {
                finalLevel += buff.ExtraLevel;
            }
            return finalLevel;
        }
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
        if (_buffList.Exists(x => x.Equals(config))) {
            _buffList.Remove(config);
        }
    }

    public int Health { 
        get {
            int finalHealth = Info.status.Health.IncreaseByLevel(FinalLevel);

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
            int finalMana = Info.status.Mana.IncreaseByLevel(FinalLevel);

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
            int finalBlock = Info.status.Block.IncreaseByLevel(FinalLevel);
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
            int finalDamage = Info.status.AttackDamage.IncreaseByLevel(FinalLevel);
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
            float finalAttackSpeed = Info.status.AttackSpeed.IncreaseByLevel(FinalLevel);
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
            int finalMoveSpeed = Info.status.MoveSpeed.IncreaseByLevel(FinalLevel);
            float moveSpeedMultiplier = 0f;

            if (Item) {
                finalMoveSpeed += Item.MoveSpeed;
            }

            foreach (BuffConfig buff in _buffList) {
                finalMoveSpeed += buff.MoveSpeed;
                moveSpeedMultiplier += buff.MoveSpeedPercent;
            }
            finalMoveSpeed += Mathf.FloorToInt(finalMoveSpeed * moveSpeedMultiplier);

            return finalMoveSpeed;
        }
    }

    public int HealthRegen {
        get { 
            int finalHealthRegen = Info.status.HealthRegen.IncreaseByLevel(FinalLevel);;
            float healthRegenMultiplier = 0f;

            if (Item) {
                finalHealthRegen += Item.HealthRegen;
            }

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
            int finalManaRegen = Info.status.ManaRegen.IncreaseByLevel(FinalLevel);;
            float manaRegenMultiplier = 0f;

            if (Item) {
                finalManaRegen += Item.ManaRegen;
            }

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
