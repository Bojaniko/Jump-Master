using UnityEngine;

using JumpMaster.Core;

namespace JumpMaster.Damage
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ExplosionEffect : MonoBehaviour, IInitializable
    {
        public static ExplosionEffect Generate(GameObject particle_prefab)
        {
            GameObject go = Instantiate(particle_prefab, Vector3.zero, Quaternion.identity);
            go.SetActive(false);
            if (go.GetComponent<ParticleSystem>() == null)
                return null;
            ExplosionEffect ee = go.AddComponent<ExplosionEffect>();
            ee.Initialize();
            ee._initialized = true;
            return ee;
        }

        public bool Initialized => _initialized;
        private bool _initialized;
        private void Initialize()
        {
            Cache();
        }

        private void Update()
        {
            if (c_particles.isStopped)
                gameObject.SetActive(false);
        }

        public void Play(ExplosionDataSO data, Vector3 position, float scale)
        {
            transform.position = position;

            gameObject.SetActive(true);

            SetupParticles(data, scale);

            c_particles.Play();
        }

        public void Stop()
        {
            gameObject.SetActive(false);
        }

        private void SetupParticles(ExplosionDataSO data, float scale)
        {
            c_shape.radius = data.Radius * scale;

            ParticleSystem.Burst burst = c_emission.GetBurst(0);
            burst.cycleCount = (int)Mathf.Floor(data.Duration / 0.3f);
            c_emission.SetBurst(0, burst);
        }

        // ##### CACHE ##### \\

        private ParticleSystem.EmissionModule c_emission;
        private ParticleSystem.ShapeModule c_shape;
        private ParticleSystem c_particles;

        private void Cache()
        {
            c_particles = GetComponent<ParticleSystem>();

            c_shape = c_particles.shape;
            c_emission = c_particles.emission;
        }
    }
}