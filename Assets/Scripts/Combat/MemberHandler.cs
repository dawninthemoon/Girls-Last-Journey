using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MemberHandler : MonoBehaviour {
    [SerializeField] private CombatDamageDisplay _damageDisplayer;
    [SerializeField] private CombatReward _rewardControl;
    private KdTree<EntityBase> _members;
    private SynergyHandler _synergyHandler;
    private MemberFactory _memberFactory;
    private EntitySpawner _spawner;
    private static readonly string MemberPrefabName = "MemberPrefab";
    public static readonly string MemberTagName = "Ally";
    public int NumOfMembers {
        get { return _members.Count; }
    }

    public KdTree<EntityBase> Members {
        get { return _members; }
    }

    private void Awake() {
        _memberFactory = GetComponent<MemberFactory>();
        _spawner = new EntitySpawner(MemberPrefabName, transform, _damageDisplayer);
        _members = new KdTree<EntityBase>(true);
        _synergyHandler = new SynergyHandler();
    }

    public void Progress(EnemyHandler enemyHandler) {
        foreach (EntityBase ally in _members) {
            var activeEnemies = enemyHandler.ActiveEnemies;
            ITargetable target = activeEnemies.FindClosest(ally.transform.position)?.GetComponent<Agent>();
            ally.SetTarget(target);

            ally.transform.position = CombatMap.ClampPosition(ally.transform.position, ally.Radius);
        }

        for (int i = 0; i < _members.Count; ++i) {
            var ally = _members[i];
            if (ally.Health <= 0 || !ally.gameObject.activeSelf) {
                _members[i].SetTarget(null);
                _members.RemoveAt(i--);
            }
        }
    }

    public void SpawnMember(Vector3 position) {
        EntityDecorator decorator = new EntityDecorator(_memberFactory.GetRandomMember());
        EntityBase newEntity = _spawner.CreateEntity(decorator);
        newEntity.transform.position = position;

        newEntity.InitializeInteractiveSettings(_rewardControl.OnItemRelease);
        _members.Add(newEntity);
    }

    public void GainExpToMembers(int totalExpAmount) {
        int amount = totalExpAmount / Members.Count;
        foreach (EntityBase member in Members) {
            member.GainExp(amount);
        }
    }

    public void OnEntityActive(EntityBase entity) {
        entity.gameObject.SetActive(true);
        _members.Add(entity); 
        _synergyHandler.AddSynergy(entity, true);
    }

    public void OnEntityInactive(EntityBase entity) {
        entity.SetTarget(null);
        _synergyHandler.AddSynergy(entity, false);
        ApplySynergies();

        entity.gameObject.SetActive(false);
    }

    public void DisarmAllAllies() {
        for (int i = 0; i < _members.Count; ++i) {
            _members[i].SetTarget(null);
        }
    }

    public void RemoveAllAllies(EntitySpawner spawner) {
        for (int i = 0; i < _members.Count; ++i) {
            spawner.RemoveEntity(_members[i]);
            _members.RemoveAt(i--);
        }
    }

    public void ApplySynergies() {
        int starts = (int)SynergyType.None + 1;
        int ends = (int)SynergyType.Count;
        for (int i = starts; i < ends; ++i) {
            SynergyType type = (SynergyType)i;
            (BuffConfig, BuffConfig) buffPair = _synergyHandler.GetSynergyBuffPair(type);
            if (buffPair.Item2 != null) {
                ApplyBuffToAll(buffPair.Item2, buffPair.Item1);
            }
        }
    }

    private void ApplyBuffToAll(BuffConfig toApply, BuffConfig toRemove) {
        foreach (EntityBase entity in _members) {
            entity.BuffControl.AddBuff(toApply);
            if (toRemove != null) {
                entity.BuffControl.RemoveBuff(toRemove);
            }
        }
    }

    public bool DoesEveryoneHasItem() {
        bool result = true;
        foreach (EntityBase entity in _members) {
            if (!entity.HasItem) {
                result = false;
                break;
            }
        }
        return result;
    }
}
