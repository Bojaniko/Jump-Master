using UnityEngine;

using JumpMaster.LevelControllers;

namespace JumpMaster.Structure
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
                if (LevelController.Paused)
                    return true;

                float elapsed_time;
                if (LevelController.LastPauseStartTime > _startTime)
                    elapsed_time = Time.time - _startTime - LevelController.LastPauseDuration;
                else
                    elapsed_time = Time.time - _startTime;
                if (elapsed_time >= _duration)
                    return false;
                return true;
            }
        }
    }
}