using UnityEngine;

using Unity.Mathematics;

using Studio28.Audio.SFX;

using JumpMaster.Obstacles;
using JumpMaster.LevelControllers;

namespace JumpMaster.SFX
{
    public class MissileThrustSFXSourceController : SFXSourceController
    {
        private MissileThrust_SFX_SC_Args _data;

        protected override void InitializeSourceController(SFXSourceControllerArgs args)
        {
            _data = (MissileThrust_SFX_SC_Args)args;

            if (_data.Caller == null)
            {
                Destroy(gameObject);
                return;
            }

            Cache();

            c_callerMissile.OnExplode += EndSFX;

            LevelController.OnRestart += EndSFX;
            LevelController.OnEndLevel += EndSFX;
            LevelController.OnPause += Pause;

            Source.volume = 0f;
        }

        private void Pause()
        {
            Source.volume = 0f;
        }

        private void Start()
        {
            Source.Play();
        }

        private void Update()
        {
            if (LevelController.Paused)
                return;

            if (!_data.Caller.activeInHierarchy)
            {
                if (Source.volume > 0f)
                    Source.volume -= Time.deltaTime;
                if (Source.volume <= 0.0f)
                    EndSFX();
                return;
            }

            SetVolumeBasedOnDistance();
        }

        // ##### VOLUME ##### \\

        private float _distanceToCenter;
        private void SetVolumeBasedOnDistance()
        {
            CalculateDistanceToCenterOfScreen();
            Source.volume = Mathf.Clamp(_distanceToCenter, _data.Thrust.VolumeRange.x, _data.Thrust.VolumeRange.y);
        }

        private void CalculateDistanceToCenterOfScreen()
        {
            if (_data.Direction.Equals(MissileDirection.UP) || _data.Direction.Equals(MissileDirection.DOWN))
            {
                _distanceToCenter = Vector2.Distance(c_callerMissile.ScreenPosition,
                        new Vector2(c_callerMissile.ScreenPosition.x, c_screenHeightHalf));
                _distanceToCenter /= c_screenHeightHalf;
            }
            if (_data.Direction.Equals(MissileDirection.LEFT) || _data.Direction.Equals(MissileDirection.RIGHT))
            {
                _distanceToCenter = Vector2.Distance(c_callerMissile.ScreenPosition,
                        new Vector2(c_screenWidthHalf, c_callerMissile.ScreenPosition.y));
                _distanceToCenter /= c_screenWidthHalf;
            }
        }

        // ##### END ##### \\

        private void EndSFX()
        {
            if (c_callerMissile != null)
                c_callerMissile.OnExplode -= EndSFX;

            LevelController.OnRestart -= EndSFX;
            LevelController.OnEndLevel -= EndSFX;
            LevelController.OnPause -= Pause;

            OnSoundEffectEnded?.Invoke(this);
        }

        public override event SoundEffectEventHandler OnSoundEffectEnded;

        // ##### CACHE ##### \\

        private float c_screenWidthHalf;
        private float c_screenHeightHalf;

        private Missile c_callerMissile;

        private void Cache()
        {
            c_screenWidthHalf = Screen.width * 0.5f;
            c_screenHeightHalf = Screen.height * 0.5f;

            c_callerMissile = _data.Caller.GetComponent<Missile>();
        }
    }

    public class MissileThrust_SFX_SC_Args : SFXSourceControllerArgs
    {
        public readonly MissileDirection Direction;
        public readonly MinMaxSoundEffectInfo Thrust;

        public MissileThrust_SFX_SC_Args(GameObject caller, MinMaxSoundEffectInfo thrust, MissileDirection direction) : base(caller)
        {
            Thrust = thrust;
            Direction = direction;
        }
    }
}