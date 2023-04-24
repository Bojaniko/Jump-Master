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

            Cache();

            transform.localScale = Vector3.one * Size;

            Health = MaxHealth;

            LevelController.OnLoad += EnableDamage;
            LevelController.OnEndLevel += DisableDamage;
        }

        private void Update()
        {
            if (!LevelController.Started)
                return;

            if (LevelController.Paused)
                return;

            if (LevelController.Ended)
                return;

            if (!c_movement.ActiveControl.ActiveState.Equals(MovementState.FALLING)
                && !c_movement.ActiveControl.ActiveState.Equals(MovementState.HANGING))
                return;

            if (c_movement.BoundsScreenPosition.min.y <= 0f)
            {
                DamageInfo damage_info = new(transform.position, Vector2.up, 1f, 20f, DamageType.TRAP);
                DamageController.LogDamage(damage_info);
            }
        }

        // ##### DAMAGE ##### \\

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

            DamageInfo output_info = new(info.Position, Vector2.up, info.Radius, damage, info.TypeOfDamage);
            DamageController.LogPlayerDamage(output_info);
        }

        private void Damage(float amount)
        {
            Health -= amount;
            if (Health < 1f)
                Health = 0f;

            Debug.Log($"Player health is {Health}.");
            
            if (Health == 0f)
            {
                LevelController.EndLevel();
            }
        }

        // ##### CACHE ##### \\

        private MovementController c_movement;

        private void Cache()
        {
            c_movement = GetComponent<MovementController>();
        }
    }
}