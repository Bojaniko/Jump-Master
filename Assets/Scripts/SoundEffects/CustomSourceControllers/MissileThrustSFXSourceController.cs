using UnityEngine;

using Unity.Mathematics;

using Studio28.SFX;

using JumpMaster.Obstacles;

namespace JumpMaster.SFX
{
    public class MissileThrustSFXSourceController : SFXSourceController
    {

        private MissileThrust_SFX_SC_Args _data;

        private Missile _callerMissile;

        protected override void InitializeSourceController(SFXSourceControllerArgs args)
        {
            _data = (MissileThrust_SFX_SC_Args)args;

            if (_data.Caller == null)
            {
                EndSFX();
                return;
            }

            _callerMissile = _data.Caller.GetComponent<Missile>();

            _callerMissile.OnExplode += EndSFX;

            Source.volume = 0f;
        }

        private void Update()
        {
            if (Source.isPlaying)
            {
                Vector3 missilePositionOnScreen = Camera.main.WorldToScreenPoint(_data.Caller.transform.position);

                float volume = 0f;

                float distanceToCenter = 0f;

                if (_data.Direction.Equals(MissileDirection.UP) || _data.Direction.Equals(MissileDirection.DOWN))
                {
                    distanceToCenter = Vector3.Distance(missilePositionOnScreen,
                            new Vector3(missilePositionOnScreen.x, Screen.height * 0.5f, missilePositionOnScreen.z));

                    distanceToCenter /= (Screen.height * 0.5f);
                }

                if (_data.Direction.Equals(MissileDirection.LEFT) || _data.Direction.Equals(MissileDirection.RIGHT))
                {
                    distanceToCenter = Vector3.Distance(missilePositionOnScreen,
                            new Vector3(Screen.width * 0.5f, missilePositionOnScreen.y, missilePositionOnScreen.z));

                    distanceToCenter /= (Screen.width * 0.5f);
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
            else if (_callerMissile.SpawnController.Spawned) Source.Play();

            if (_data.Caller.activeInHierarchy == false)
            {
                if (Source.volume > 0f)
                    Source.volume -= Time.deltaTime;
                if (Source.volume <= 0.0f)
                    EndSFX();
            }
        }

        private void EndSFX()
        {
            _callerMissile.OnExplode -= EndSFX;
            Destroy(gameObject);
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