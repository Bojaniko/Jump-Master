using UnityEngine;

using Unity.Mathematics;

using Studio28.SFX;

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

        private void Update()
        {
            if (LevelController.Paused)
                return;

            if (Source.isPlaying)
            {
                float volume = 0f;
                float distanceToCenter = 0f;

                if (_data.Direction.Equals(MissileDirection.UP) || _data.Direction.Equals(MissileDirection.DOWN))
                {
                    distanceToCenter = Vector2.Distance(c_callerMissile.ScreenPosition,
                            new Vector2(c_callerMissile.ScreenPosition.x, c_screenHeightHalf));

                    distanceToCenter /= c_screenHeightHalf;
                }

                if (_data.Direction.Equals(MissileDirection.LEFT) || _data.Direction.Equals(MissileDirection.RIGHT))
                {
                    distanceToCenter = Vector2.Distance(c_callerMissile.ScreenPosition,
                            new Vector2(c_screenWidthHalf, c_callerMissile.ScreenPosition.y));

                    distanceToCenter /= c_screenWidthHalf;
                }

                if (distanceToCenter <= 1f)
                    volume = math.remap(0f, 1f, Info.VolumeRange.x, Info.VolumeRange.y, 1f - Mathf.Clamp(distanceToCenter, 0f, 1f));
                if (distanceToCenter > 1f)
                {
                    volume = Source.volume;
                    if (volume < Info.VolumeRange.x)
                    {
                        volume += Info.VolumeRange.x * Time.deltaTime;
                        if (volume > Info.VolumeRange.x)
                            volume = Info.VolumeRange.x;
                    }
                }

                Source.volume = volume;
            }
            else if (c_callerMissile.Spawned) Source.Play();

            if (_data.Caller.activeInHierarchy == false)
            {
                if (Source.volume > 0f)
                    Source.volume -= Time.deltaTime;
                if (Source.volume <= 0.0f)
                    EndSFX();
            }
        }

        private void Pause()
        {
            Source.volume = 0f;
        }

        private void EndSFX()
        {
            if (c_callerMissile != null)
                c_callerMissile.OnExplode -= EndSFX;

            LevelController.OnRestart -= EndSFX;
            LevelController.OnEndLevel -= EndSFX;
            LevelController.OnPause -= Pause;

            Destroy(gameObject);
        }

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

        public MissileThrust_SFX_SC_Args(GameObject caller, MissileDirection direction) : base(caller)
        {
            Direction = direction;
        }
    }
}