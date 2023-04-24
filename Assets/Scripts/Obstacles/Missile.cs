using System.Collections;

using UnityEngine;

using Studio28.SFX;

using JumpMaster.SFX;
using JumpMaster.LevelControllers;
using JumpMaster.UI;

namespace JumpMaster.Obstacles
{
    public enum MissileDirection { UP, DOWN, LEFT, RIGHT }

    public sealed class MissileSpawnArgs : SpawnArgs
    {
        public readonly MissileDirection Direction;
        public MissileSpawnArgs(Vector3 screen_position, MissileDirection direction) : base(screen_position)
        {
            Direction = direction;
        }
    }

    public class Missile : Obstacle<MissileSO, MissileController, MissileSpawnSO, MissileSpawnMetricsSO, MissileSpawnArgs>
    {
        protected override void Initialize()
        {
            Cache();

            c_particles.transform.localScale = Vector3.one * Data.Scale;

            LevelController.OnPause += Pause;
            LevelController.OnResume += Resume;
            LevelController.OnEndLevel += EndLevel;
        }

        private void Pause()
        {
            c_particles.Pause();
            c_rigidbody.velocity = Vector3.zero;
            c_animator.SetFloat("SwirlMult", 0f);
        }

        private void Resume()
        {
            if (c_warning.Playing)
                return;

            c_particles.Play();
            c_animator.SetFloat("SwirlMult", 1f);
        }

        private void EndLevel()
        {
            if (c_warning.Playing)
            {
                c_warning.Stop();
                return;
            }
            StartExplosion();
        }

        protected override void OnUpdate()
        {
            
        }

        void FixedUpdate()
        {
            if (LevelController.Paused)
                return;

            if (_explodeCoroutine != null)
                return;

            if (c_warning.Playing)
                return;

            c_rigidbody.velocity = _direction * SpawnData.Speed;
        }

        // ##### SPAWNING ##### \\

        private Vector3 _direction;

        protected override void SpawnInstructions()
        {
            c_animator.SetFloat("SwirlMult", 1f);
            c_renderer.enabled = false;

            ApplyDirection(SpawnArgs.Direction);

            StartWarning(SpawnArgs);
        }

        protected override void DespawnInstructions()
        {
            if (_explodeCoroutine != null)
            {
                StopCoroutine(_explodeCoroutine);
                _explodeCoroutine = null;
            }
        }

        protected override bool IsDespawnable()
        {
            if (_explodeCoroutine != null)
                return false;
            if (c_warning.Playing)
                return false;

            switch (SpawnArgs.Direction)
            {
                case MissileDirection.UP:
                    return BoundsOverScreen;

                case MissileDirection.DOWN:
                    return BoundsUnderScreen;

                case MissileDirection.LEFT:
                    return BoundsLeftOfScreen;

                case MissileDirection.RIGHT:
                    return BoundsRightOfScreen;
            }
            return false;
        }

        private void ApplyDirection(MissileDirection direction)
        {
            switch (direction)
            {
                case MissileDirection.UP:
                    _direction = Vector3.up;

                    transform.rotation = Quaternion.Euler(0, 0, 180f);
                    c_bounds.transform.rotation = Quaternion.identity;
                    break;

                case MissileDirection.DOWN:
                    _direction = Vector3.down;

                    // Default model rotation is down, so the rotation is 0 degrees in all angles

                    transform.rotation = Quaternion.identity;
                    c_bounds.transform.rotation = Quaternion.identity;
                    break;

                case MissileDirection.LEFT:
                    _direction = Vector3.left;

                    transform.rotation = Quaternion.Euler(0f, 0f, -90f);
                    c_bounds.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    break;

                case MissileDirection.RIGHT:
                    _direction = Vector3.right;

                    transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    c_bounds.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
                    break;
            }
        }

        private Vector3 GetWorldSpawnPosition(Vector3 screen_position, MissileDirection direction)
        {
            Vector3 position_world = c_camera.ScreenToWorldPoint(screen_position);
            switch (direction)
            {
                case MissileDirection.UP:
                    position_world.y -= SpawnMetrics.SpawnOffset;
                    break;

                case MissileDirection.DOWN:
                    position_world.y += SpawnMetrics.SpawnOffset;
                    break;

                case MissileDirection.LEFT:
                    position_world.x += SpawnMetrics.SpawnOffset;
                    break;

                case MissileDirection.RIGHT:
                    position_world.x -= SpawnMetrics.SpawnOffset;
                    break;
            }
            position_world.z = transform.position.z;
            return position_world;
        }

        // ##### WARNING ##### \\

        private void StartWarning(MissileSpawnArgs spawn_args)
        {
            MissileWarningArgs warningData = new(spawn_args.ScreenPosition, spawn_args.Direction);

            c_warning.OnWarningEnded += EndWarning;
            c_warning.Play(warningData);
        }

        private void EndWarning()
        {
            transform.position = GetWorldSpawnPosition(SpawnArgs.ScreenPosition, SpawnArgs.Direction);

            SFXController.Instance.PlayLoopSound<MissileThrustSFXSourceController>(Data.ThrustSFX, new MissileThrust_SFX_SC_Args(gameObject, SpawnArgs.Direction));
            c_renderer.enabled = true;
            c_particles.Play();
        }

        // ##### EXPLOSION ##### \\

        private Coroutine _explodeCoroutine;

        public delegate void ExplosionEventHandler();
        public event ExplosionEventHandler OnExplode;

        private void OnTriggerEnter(Collider other)
        {
            if (c_warning.Playing)
                return;

            if (other.gameObject.CompareTag("Player"))
            {
                StartExplosion();
            }
        }

        private void StartExplosion()
        {
            if (!gameObject.activeSelf)
                return;
            if (_explodeCoroutine != null)
                return;
            _explodeCoroutine = StartCoroutine("Explode");
        }

        private IEnumerator Explode()
        {
            c_rigidbody.velocity = Vector3.zero;

            c_animator.SetFloat("SwirlMult", 0f);

            Instantiate(Data.ExplosionPrefab, transform.position - Vector3.forward * 0.2f, Quaternion.identity);

            SFXController.Instance.PlaySound(Data.ExplosionSFX, gameObject);

            OnExplode?.Invoke();

            yield return new WaitForSeconds(Data.GameObjectDestroyDelayMS / 1000f);

            Despawn();
        }

        // ##### CACHE ##### \\

        private MissileWarning c_warning;

        private MeshRenderer c_renderer;

        private ParticleSystem c_particles;

        private void Cache()
        {
            c_warning = MissileWarning.Generate(Data.WarningData);

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).name.Equals("missile"))
                    c_renderer = transform.GetChild(i).GetComponent<MeshRenderer>();
                if (transform.GetChild(i).name.Equals("thrust_particles"))
                    c_particles = transform.GetChild(i).GetComponent<ParticleSystem>();
            }
        }
    }
}