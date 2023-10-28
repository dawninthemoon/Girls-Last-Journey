using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RieslingUtils;

public class MemberHandler : MonoBehaviour {
    [SerializeField] private CombatDamageDisplay _damageDisplayer;
    [SerializeField] private CombatReward _rewardControl;
    [SerializeField] private SynergyHandler _synergyHandler;
    private KdTree<EntityBase> _members;
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
                ChangeSynergy(member, false);
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

        ChangeSynergy(newEntity, true);
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

    private void ChangeSynergy(EntityBase entity, bool isAdded) {
        _synergyHandler.ApplyAllSynergyBuffs(Members, true);
        _synergyHandler.ChangeSynergy(entity, isAdded);
        _synergyHandler.ApplyAllSynergyBuffs(Members, false);
    }
}
