using BondomanShooter.Structs;

namespace BondomanShooter.Entities {
    public interface IHasDamageModifier {
        ModifierStack<float> DamageModifier { get; }
    }
}
