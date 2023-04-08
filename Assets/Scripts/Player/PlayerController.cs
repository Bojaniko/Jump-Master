using UnityEngine;

using JumpMaster.Movement;

namespace JumpMaster.LevelControllers
{
    [RequireComponent(typeof(MovementController))]
    public class PlayerController : LevelControllerInitializable
    {
        [Range(0f, 2f)] public float Size = 0.2f;
        [Range(1f, 200f)] public float MaxHealth = 100f;

        public float Health { get; private set; }

        private static PlayerController s_instance;
        public static PlayerController Instance
        {
            get
            {
                return s_instance;
            }
            private set
            {
                if (s_instance == null)
                    s_instance = value;
                else
                    Debug.LogError("There can only be one Player Controller in the scene!");
            }
        }

        protected override void Initialize()
        {
            Instance = this;

            transform.localScale = Vector3.one * Size;

            Health = MaxHealth;

            LevelController.OnLoad += EnableDamage;
            LevelController.OnEndLevel += DisableDamage;
        }

        private void EnableDamage() { DamageController.OnDamageLogged += InputDamage; }
        private void DisableDamage() { DamageController.OnDamageLogged -= InputDamage; }

        private void InputDamage(DamageInfo info)
        {
            float distance_percentage = Vector2.Distance(info.Position, transform.position) / info.Radius;
            if (distance_percentage > 1f)
                return;

            float damage = 1f - Mathf.Clamp(distance_percentage, 0f, 1f);
            damage *= info.Amount;

            Damage(damage);
        }

        private void Damage(float amount)
        {
            Health -= amount;
            if (Health < 1f)
                Health = 0f;
            if (Health == 0f)
            {
                LevelController.EndLevel();
            }
        }
    }
}