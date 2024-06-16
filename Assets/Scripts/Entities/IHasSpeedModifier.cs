using BondomanShooter.Structs;

namespace BondomanShooter.Entities {
    public interface IHasSpeedModifier {
        ModifierStack<float> SpeedModifier { get; }
    }
}
