using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EntityBase : MonoBehaviour {
    [SerializeField] private Transform _bulletPosition = null;
    private Agent _agent;
    private EntityAnimationControl _animationControl;
    private EntityDecorator _entityDecorator;
    private EntityHealthMana _healthManaControl;
    private EntityUIControl _uiControl;
    private float _currentMorale;
    public EntityInfo Info { get { return _entityDecorator.Info; } }
    public DamageInfo FinalDamageInfo {
        get;
        private set;
    }
    public EntityBuff BuffControl {
        get;
        private set;
    }
    public float Radius {
        get {
            if (_entityDecorator == null) {
                return 20f;
            }
            return Info.bodyRadius;
        }
    }
    public string ID {
        get { return Info.entityID; }
    }
    public int Health { 
        get { return _healthManaControl.Health; }
    }
    public int Mana { 
        get { return _healthManaControl.Mana; }
    }
    public SynergyType Synergy1 {
        get { return Info.synergy1; }
    }
    public SynergyType Synergy2 {
        get { return Info.synergy2; }
    }
    public SynergyType ExtraSynergy {
        get { return _entityDecorator.ExtraSynergy; }
    }

    public Vector2 HandDirection { get; private set; }
    public int AttackDamage { get { return _entityDecorator.AttackDamage; } }
    public Vector3 BulletPosition { get { return _bulletPosition.position; } }
    public bool CanBehaviour { get; set; } = true;
    private InteractiveEntity _interactiveControl;

    private void Awake() {
        _agent = GetComponent<Agent>();
        _interactiveControl = GetComponent<InteractiveEntity>();
        _animationControl = GetComponent<EntityAnimationControl>();
        _uiControl = GetComponent<EntityUIControl>();
        _healthManaControl = new EntityHealthMana(_uiControl);
        FinalDamageInfo = new DamageInfo(this);

        BuffControl = new EntityBuff(this, _entityDecorator);

        _agent.OnMovementInput.AddListener(Move);
        _agent.OnAttackRequested.AddListener(Attack);
    }

    public void Initialize(EntityDecorator entityDecorator) {
        _entityDecorator = entityDecorator;
        _bulletPosition.localPosition = Info.bulletOffset;

        Sprite weaponSprite = Info.attackConfig.Config.weaponSprite;

        _healthManaControl.Initialize(entityDecorator);
        _animationControl.Initialize(Info.bodySprite, weaponSprite, Info.animatorController);
        
        _agent.Initialize(_entityDecorator, Radius);

        InitalizeStatus();
    }

    private void Update() {
        if (BuffControl != null) {
            CanBehaviour = !BuffControl.IsDebuffExists("stun");
        }
    }

    private void InitalizeStatus() {
        _healthManaControl.Health = _entityDecorator.Health;
        _healthManaControl.Mana = 0;
    }

    public void InitializeInteractiveSettings() {
        _interactiveControl.ClearAllEvents();
        
        _interactiveControl.OnMouseDownEvent.AddListener(() => {
            CanBehaviour = false;
        });

        _interactiveControl.OnMouseDragEvent.AddListener(() => {
            Vector2 mousePos = RieslingUtils.ExMouse.GetMouseWorldPosition();
            transform.position = mousePos;
        });

        _interactiveControl.OnMouseUpEvent.AddListener(() => {
            CanBehaviour = true;
        });
    }

    public void SetTarget(ITargetable target) {
        _agent?.SetTarget(target);
    }

    private void Move(Vector2 direction) {
        if (!CanBehaviour) {
            return;
        }
        _animationControl.SetMoveAnimationState(!direction.Equals(Vector2.zero));
        if (direction.sqrMagnitude > 0f) {
            Vector3 nextPosition = transform.position + (Vector3)direction * Time.deltaTime * _entityDecorator.MoveSpeed;
            transform.position = nextPosition;

            Vector3 faceDir = direction;
            if (_agent.SelectedTarget != null) {
                faceDir = (_agent.SelectedTarget.Position - transform.position);
            }
            _animationControl.SetFaceDir(faceDir);
        }
    }

    private void Attack() {
        if (!CanBehaviour) {
            return;
        }

        _healthManaControl.AddMana(10);

        AttackConfig config = Info.attackConfig.Config;
        if (_healthManaControl.IsManaFull) {
            // Use Skill
            _healthManaControl.Mana = 0;
            config = Info.skillConfig.Config;
        }
        _animationControl.PlayAttackAnimation();

        var effects = config.attackEffects;
        List<EntityBase> targets 
            = Physics2D.OverlapCircleAll(transform.position, _entityDecorator.AttackRange + Radius * 2.5f, config.targetLayerMask)
                .Select(x => x.GetComponent<EntityBase>())
                .Where(x => x.Health > 0)
                .OrderBy(x => (x.transform.position - transform.position).sqrMagnitude)
                .ToList();

        if (targets.Count > 0) {
            Vector2 direction = (targets[0].transform.position - transform.position).normalized;
            HandDirection = direction;
            _animationControl.SetFaceDir(direction);

            SoundManager.Instance.PlayGameSe(config.soundEffectName);
            config.attackBehaviour.Behaviour(this, targets, effects);
        }
    }

    public void ReceiveDamage(int damage, EntityBase caster = null) {
        if (Health <= 0) return;

        _healthManaControl.AddMana(10);
        int finalDamage = Mathf.Max(1, damage - _entityDecorator.Block);

        _healthManaControl.ReceiveDamage(finalDamage);
        FinalDamageInfo.ReceiveDamage(damage, caster);

        if (Health <= 0) {
           OnEntityDead();
        }
    }

    public void EquipItem(EntityItem item) {
        _entityDecorator.Item = item;
        _uiControl.ShowEquipedItem(item.Sprite);
    }

    private void OnEntityDead() {
        gameObject.SetActive(false);
    }

    private void OnTriggerStay2D(Collider2D other) {
        if ((other.CompareTag(EnemyHandler.EnemyTagName) || other.CompareTag(MemberHandler.MemberTagName))) {
            Vector3 direction = (other.transform.position - transform.position).normalized;
            other.transform.position += direction * 100f * Time.deltaTime;
        }
    }
}
