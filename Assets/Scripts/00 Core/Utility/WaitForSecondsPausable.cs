using UnityEngine;

namespace JumpMaster.Core
{
    public sealed class WaitForSecondsPausable : CustomYieldInstruction
    {
        private readonly float _duration;
        private readonly float _startTime;

        public WaitForSecondsPausable(float duration)
        {
            _duration = duration;
            _startTime = Time.time;
        }

        public override bool keepWaiting
        {
            get
            {
                if (LevelManager.Paused)
                    return true;

                float elapsed_time;
                if (LevelManager.LastPauseStartTime > _startTime)
                    elapsed_time = Time.time - _startTime - LevelManager.LastPauseDuration;
                else
                    elapsed_time = Time.time - _startTime;
                if (elapsed_time >= _duration)
                    return false;
                return true;
            }
        }
    }
}