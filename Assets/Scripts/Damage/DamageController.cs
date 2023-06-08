using System.Collections.Generic;

using UnityEngine;

using JumpMaster.Core;

namespace JumpMaster.Damage
{
    public delegate void DamageDelegate(IDamageRecord record);

    [DisallowMultipleComponent]
    public class DamageController : LevelController
    {
        public static DamageController Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;
                return GenerateInstance();
            }
            private set
            {
                if (_instance == null)
                    _instance = value;
                else if (_instance != value)
                    Debug.LogError("You can have only one instance of DamageController in your scene!");
            }
        }
        private static DamageController _instance;
        private static DamageController GenerateInstance()
        {
            GameObject go = new GameObject("DAMAGE-CONTROLLER", typeof(DamageController));
            return go.GetComponent<DamageController>();
        }

        protected override void Initialize()
        {
            Instance = this;

            _sources = new();
            _records = new();
            _registrations = new();

            if (DamageLayerMask == 0)
                DamageLayerMask = LayerMask.GetMask("Default");
        }

        public LayerMask DamageLayerMask;

        private void Update()
        {
            if (LevelManager.Paused)
                return;

            if (_registrations.Count == 0)
                return;

            if (_sources.Count == 0)
                return;

            RecordSources(ref _sources, ref _records);

            ProcessRecords(_records.ToArray(), _registrations.ToArray());
        }

        // ##### REGISTRATION ##### \\

        private List<IDamageRegistration> _registrations;
        /// <summary>
        /// Register a damage output listener.
        /// </summary>
        /// <typeparam name="DamageSourceType">The type of the source.</typeparam>
        /// <param name="target">The gameobject which will take damage.</param>
        /// <param name="callback">The method which will play when a correct damage output is detected, which takes an IDamageRecord argument.</param>
        public void RegisterListener<DamageSourceType>(GameObject target, DamageDelegate callback) where DamageSourceType : DamageSource
        {
            DamageRegistration<DamageSourceType> registration = new(target, callback);
            _registrations.Add(registration);
        }

        private bool IsRegistrationRecordMatching(IDamageRegistration registration, IDamageRecord record)
        {
            if (!registration.GetDamageSourceType().Equals(record.GetDamageSourceType()))
                return false;

            if (!registration.Target.Equals(record.Target))
                return false;

            return true;
        }

        // ##### RECORDING SOURCES ##### \\

        private List<DamageSource> _sources;
        /// <summary>
        /// Log a damage source.
        /// </summary>
        /// <param name="source">The source.</param>
        public void LogDamageSource(DamageSource source)
        {
            _sources.Add(source);
        }

        private void RecordSources(ref List<DamageSource> sources, ref List<IDamageRecord> records)
        {
            records.Clear();
            for (int i = 0; i < sources.Count; i++)
            {
                records.AddRange(sources[i].RecordDamage(DamageLayerMask));
                if (sources[i].Recorded)
                    sources.Remove(sources[i]);
            }
        }

        // ##### RECORD PROCESSING ##### \\

        private List<IDamageRecord> _records;
        private void ProcessRecords(IDamageRecord[] records, IDamageRegistration[] registrations)
        {
            foreach (IDamageRegistration registration in registrations)
            {
                foreach (IDamageRecord record in records)
                {
                    LogDamageOutput(record, registration);
                }
            }
        }

        private void LogDamageOutput(IDamageRecord record, IDamageRegistration registration)
        {
            if (IsRegistrationRecordMatching(registration, record))
                registration?.Callback(record);
        }

        #if UNITY_EDITOR
        // ##### GIZMOS ##### \\

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            Gizmos.color = Color.magenta;

            for (int i = 0; i < _sources.Count; i++)
            {
                if (_sources[i] is StunAreaDamageSource)
                {
                    StunAreaDamageSource sa = (StunAreaDamageSource)_sources[i];
                    Gizmos.DrawLine(new Vector2(sa.Data.PointA.x, sa.Data.PointA.y), new Vector2(sa.Data.PointB.x, sa.Data.PointA.y)); // BOTTOM
                    Gizmos.DrawLine(new Vector2(sa.Data.PointA.x, sa.Data.PointB.y), new Vector2(sa.Data.PointB.x, sa.Data.PointB.y)); // TOP
                    Gizmos.DrawLine(new Vector2(sa.Data.PointA.x, sa.Data.PointA.y), new Vector2(sa.Data.PointA.x, sa.Data.PointB.y)); // LEFT
                    Gizmos.DrawLine(new Vector2(sa.Data.PointB.x, sa.Data.PointA.y), new Vector2(sa.Data.PointB.x, sa.Data.PointB.y)); // RIGHT
                }
                if (_sources[i] is ExplosionDamageSource)
                {
                    ExplosionDamageSource e = (ExplosionDamageSource)_sources[i];
                    Gizmos.DrawWireSphere(e.Data.Origin, e.Data.Radius);
                }
            }
        }
        #endif
    }
}