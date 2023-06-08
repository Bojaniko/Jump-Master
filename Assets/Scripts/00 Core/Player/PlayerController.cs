using UnityEngine;

using JumpMaster.Movement;
using JumpMaster.Damage;

namespace JumpMaster.Core
{
    [RequireComponent(typeof(MovementController))]
    public class PlayerController : LevelController
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

            DamageController.Instance.RegisterListener<ExplosionDamageSource>(gameObject, InputDamage);
            DamageController.Instance.RegisterListener<StunAreaDamageSource>(gameObject, InputDamage);

            Restart();
            LevelManager.OnRestart += Restart;
        }

        private void Restart()
        {
            Health = MaxHealth;
        }

        private void Update()
        {
            if (!LevelManager.Started)
                return;

            if (LevelManager.Paused)
                return;

            if (LevelManager.Ended)
                return;

            if (!c_movement.ActiveControl.ActiveState.Equals(MovementState.FALLING)
                && !c_movement.ActiveControl.ActiveState.Equals(MovementState.HANGING))
                return;

            if (c_movement.Bounds.ScreenMin.y <= 0f)
            {
                //DamageInfo damage_info = new(transform.position, Vector2.up, 1f, 20f, DamageType.TRAP);
                //DamageController.LogDamage(damage_info);
            }
        }

        // ##### DAMAGE ##### \\

        private void InputDamage(IDamageRecord record)
        {
            Health -= record.DamageOutput;
            if (Health < 1f)
                Health = 0f;

            //Debug.Log($"Player health is {Health}.");

            if (Health == 0f)
            {
                LevelManager.EndLevel();
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