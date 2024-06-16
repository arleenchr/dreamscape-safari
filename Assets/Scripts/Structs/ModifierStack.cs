using System.Collections.Generic;

namespace BondomanShooter.Structs {
    public class ModifierStack<T> {
        public delegate T Modifier(T value);
        
        private readonly List<(int priority, IModifierItem modifier)> modifiers;

        public T BaseValue { get; set; }
        public T Value {
            get {
                T value = BaseValue;
                for(int i = 0; i < modifiers.Count; i++) {
                    IModifierItem mod = modifiers[i].modifier;

                    if(mod.ShouldRemove) {
                        modifiers.RemoveAt(i);
                        mod.OnRemoved();
                        i--;
                        continue;
                    }

                    value = mod.Mod(value);
                }

                return value;
            }
        }

        public int Count => modifiers.Count;

        public ModifierStack(T baseValue) {
            modifiers = new List<(int priority, IModifierItem modifier)>();
            BaseValue = baseValue;
        }

        public void Add(int priority, IModifierItem modifier) {
            for(int i = 0; i < modifiers.Count; i++) {
                if(modifiers[i].priority < priority) {
                    modifiers.Insert(i, (priority, modifier));
                    return;
                }
            }
            modifiers.Add((priority, modifier));
        }

        public void Remove(IModifierItem modifier) {
            for(int i = 0; i < modifiers.Count; i++) {
                if(modifiers[i].modifier == modifier) {
                    modifiers.RemoveAt(i);
                    return;
                }
            }
        }

        public interface IModifierItem {
            Modifier Mod { get; }
            bool ShouldRemove { get; }
            void OnRemoved();
        }
    }
}
