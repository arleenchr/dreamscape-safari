using BondomanShooter.Items.Weapons;
using System;
using UnityEngine;

namespace BondomanShooter.Entities {
    public interface IHealth {
        int Health { get; }
        int MaxHealth { get; }

        public bool IsDead => Health <= 0;

        DamageResult ApplyDamage(WeaponOwner source, int damage, Vector3 location, bool parriable = true);
    }

    [Flags]
    public enum DamageResult {
        None = 0,
        Hit = 1,
        Parried = 2
    }
}
