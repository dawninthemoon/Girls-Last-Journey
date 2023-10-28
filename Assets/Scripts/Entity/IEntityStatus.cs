public interface IEntityStatus {
    int Health { get; }
    int HealthRegen { get; }
    int Mana { get; }
    int ManaRegen { get; }
    int Block { get; }
    int AttackDamage { get; }
    float AttackSpeed { get; }
    int MoveSpeed { get; }
}