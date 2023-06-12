using System.Collections.Generic;

using UnityEngine;

using JumpMaster.Core;

namespace JumpMaster.Damage
{
    public delegate void DamageDelegate(IDamageRecord record);

    /// <summary>
    /// The damage controller works as an event bus between damage sources and damage registrations <br/>
    /// <see cref="DamageSource">DamageSource</see>'s are objects that define how damage is outputed, 
    /// while objects of type <see cref="IDamageRegistration">DamageRegistration</see> hold the contract between a caller object and a damage source type.
    /// </summary>
    [DisallowMultipleComponent]
    public class DamageController : LevelController
    {
        public LayerMask DamageLayerMask => _damageLayerMask;
        [SerializeField, InspectorName("Layer Mask")] private LayerMask _damageLayerMask;

        private void Update()
        {
            if (LevelManager.Paused)
                return;

            if (_registrations.Count == 0)
                return;

            if (_sources.Count == 0)
                return;

            ProcessSourcesToRecords();

            ProcessRecordsToListeners();
        }

        #region Instance and initialization
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

            if (_damageLayerMask == 0)
                _damageLayerMask = LayerMask.GetMask("Default");
        }
        #endregion

        #region Damage source listener registration
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
        #endregion

        #region Processing
        private List<IDamageRecord> _records;
        private void ProcessSourcesToRecords()
        {
            _records.Clear();
            foreach (DamageSource source in _sources)
                _records.AddRange(source.RecordDamage(DamageLayerMask));
        }

        private void ProcessRecordsToListeners()
        {
            foreach (IDamageRegistration registration in _registrations)
            {
                foreach (IDamageRecord record in _records)
                {
                    if (IsRegistrationRecordMatching(registration, record))
                        registration?.Callback(record);
                }
            }
        }
        #endregion

        #region Source logging

        private List<DamageSource> _sources;
        public void LogSource(DamageSource source)
        {
            _sources.Add(source);
            source.OnRecorded += RemoveSource;
        }
        private void RemoveSource(DamageSource source)
        {
            _sources.Remove(source);
            source.OnRecorded -= RemoveSource;
        }
        #endregion

        #region Gizmos
#if UNITY_EDITOR
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
                    Gizmos.DrawLine(new Vector2(sa.AreaData.PointA.x, sa.AreaData.PointA.y), new Vector2(sa.AreaData.PointB.x, sa.AreaData.PointA.y)); // BOTTOM
                    Gizmos.DrawLine(new Vector2(sa.AreaData.PointA.x, sa.AreaData.PointB.y), new Vector2(sa.AreaData.PointB.x, sa.AreaData.PointB.y)); // TOP
                    Gizmos.DrawLine(new Vector2(sa.AreaData.PointA.x, sa.AreaData.PointA.y), new Vector2(sa.AreaData.PointA.x, sa.AreaData.PointB.y)); // LEFT
                    Gizmos.DrawLine(new Vector2(sa.AreaData.PointB.x, sa.AreaData.PointA.y), new Vector2(sa.AreaData.PointB.x, sa.AreaData.PointB.y)); // RIGHT
                }
                if (_sources[i] is ExplosionDamageSource)
                {
                    ExplosionDamageSource e = (ExplosionDamageSource)_sources[i];
                    Gizmos.DrawWireSphere(e.Data.Origin, e.ExplosionData.Radius);
                }
            }
        }
        #endif
        #endregion
    }
}