using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

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
        foreach (EntityBase member in _members) {
            var activeEnemies = enemyHandler.ActiveEnemies;
            ITargetable target = activeEnemies.FindClosest(member.transform.position)?.GetComponent<Agent>();
            member.SetTarget(target);
            member.transform.position = CombatMap.ClampPosition(member.transform.position, member.Radius);
        }

        for (int i = 0; i < _members.Count; ++i) {
            var member = _members[i];
            if (member.Health <= 0 || !member.gameObject.activeSelf) {
                _spawner.RemoveEntity(member);
                _members.RemoveAt(i--);
            }
        }
    }

    public void SpawnMember(Vector3 position) {
        EntityDecorator decorator = new EntityDecorator(_memberFactory.GetRandomMember());
        EntityBase newEntity = _spawner.CreateEntity(decorator);
        newEntity.transform.position = position.ChangeZPos(-5f);

        newEntity.InitializeInteractiveSettings(_rewardControl.OnItemRelease);
        newEntity.InitializeEncounterSettings(OnEntitySold);
        _members.Add(newEntity);
    }

    public void GainExpToMembers(int totalExpAmount) {
        int amount = totalExpAmount / Members.Count;
        foreach (EntityBase member in Members) {
            member.GainExp(amount);
        }
    }

    public void DisarmAllMember() {
        for (int i = 0; i < _members.Count; ++i) {
            _members[i].SetTarget(null);
        }
    }

    public void RemoveAllMember(EntitySpawner spawner) {
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

    private void OnEntitySold(EntityBase entity) {
        Vector3 position = entity.transform.position;

        // 다음 Progress에 지워짐
        entity.gameObject.SetActive(false);
        SpawnMember(position);        
    }
}
