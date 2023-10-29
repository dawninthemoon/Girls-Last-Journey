using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class EntityHealthMana {
    private int _currentHealth;
    private int _currentMana;
    private EntityDecorator _entityDecorator;
    private EntityUIControl _uiControl;
    public int Health {
        get { return _currentHealth; }
        set {
            _currentHealth = value;
            _uiControl.UpdateHealthBar(Health, _entityDecorator.Health);
        }
    }
    public int Mana { 
        get { return _currentMana; }
        set { 
            _currentMana = value;
            _uiControl.UpdateManaBar(Mana, _entityDecorator.Mana);
        }
    }
    private static readonly double RegenDelay = 1.0;

    public EntityHealthMana(EntityUIControl uiControl) {
        _uiControl = uiControl;
    }

    public void Initialize(EntityDecorator decorator) {
        _entityDecorator = decorator;
        _currentHealth = _entityDecorator.Health;

        RegenProgress().Forget();
    }

    private async UniTaskVoid RegenProgress() {
        while (_uiControl.gameObject.activeSelf) {
            Health = Mathf.Min(_currentHealth + _entityDecorator.HealthRegen, _entityDecorator.Health);
            Mana = Mathf.Min(_currentMana + _entityDecorator.ManaRegen, _entityDecorator.Mana);

            await UniTask.Delay(System.TimeSpan.FromSeconds(RegenDelay));
        }
    }

    public void AddHealth(int amount) {
        Health = Mathf.Min(_currentHealth + amount, _entityDecorator.Health);
    }

    public void AddMana(int amount) {
        Mana = Mathf.Min(_currentMana + amount, _entityDecorator.Mana);
    }

    public void ReduceHealth(int amount) {
        Health = Mathf.Max(_currentHealth - amount, 0);
    }

    public void ReduceMana(int amount) {
        Mana = Mathf.Max(_currentMana - amount, 0);
    }
}
