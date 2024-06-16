using BondomanShooter.Entities.Collectibles;
using BondomanShooter.Game;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BondomanShooter.Entities.Pets
{
    public class PetOwner : MonoBehaviour
    {
        [Header("Pets")]
        [SerializeField] private PetEntityInstance[] initialPets;

        public bool[] OwnedPets { get; private set; }
        public bool[] EquippedPets { get; private set; }
        public bool IsFullHPPet { get; set; }

        public List<PetController> Pets => pets;
        private List<PetController> pets;

        // Start is called before the first frame update
        void Awake()
        {
            pets = new List<PetController> ();
            OwnedPets = new bool[2];
            EquippedPets = new bool[2];
            int i = 0;
            foreach (PetEntityInstance petEntity in initialPets)
            {
                // Instantiate pet game object
                GameObject instance = Instantiate(petEntity.Pet.gameObject);

                // Set pet attributes
                PetController pet = instance.GetComponent<PetController>();
                pet.Owner = this;

                // Push pet into the owner list
                pets.Add(pet);
                OwnedPets[i] = petEntity.owned;
                EquippedPets[i] = petEntity.equipped;

                if (!EquippedPets[i])
                    pets[i].gameObject.SetActive(false);
                i++;
            }
        }

        void Update()
        {
            for (int i = 0; i < pets.Count; i++)
            {
                pets[i].gameObject.SetActive(EquippedPets[i]);
            }
        }

        public void EquipPet(PetController petController) => EquipPet(Pets.IndexOf(petController));

        public void EquipPet(int petIndex)
        {
            GameController.Instance.player.PetOwner.EquippedPets[petIndex] = true;
            Pets[petIndex].transform.position = transform.position;
            Debug.Log($"Equip pet {petIndex}");
        }

        public void UnequipPet(PetController petController) => UnequipPet(Pets.IndexOf(petController));

        public void UnequipPet(int petIndex)
        {
            GameController.Instance.player.PetOwner.EquippedPets[petIndex] = false;
            Debug.Log($"Unequip pet {petIndex}");
        }

        public void OwnPet(PetController petController) => OwnPet(Pets.IndexOf(petController));

        public void OwnPet(int petIndex)
        {
            GameController.Instance.player.PetOwner.OwnedPets[petIndex] = true;
            Debug.Log($"Own pet {petIndex}");
        }

        public void DisownPet(PetController petController) => DisownPet(Pets.IndexOf(petController));

        public void DisownPet(int petIndex)
        {
            GameController.Instance.player.PetOwner.OwnedPets[petIndex] = false;
            Debug.Log($"Disown pet {petIndex}");
        }

        public void FullHPPet()
        {
            IsFullHPPet = !IsFullHPPet;
            for (int i = 0; i < pets.Count; i++)
            {
                if (EquippedPets[i])
                {
                    Pets[i].FullHPPet();
                    break;
                }
            }
        }

        public void KillPet()
        {
            for (int i = 0; i < pets.Count; i++)
            {
                if (EquippedPets[i])
                {
                    Pets[i].KillPet();
                    break;
                }
            }
        }
    }

    [Serializable]
    public class PetEntityInstance
    {
        [SerializeField] private PetController pet;

        public PetController Pet => pet;
        public bool owned = false;
        public bool equipped = false;
    }
}
