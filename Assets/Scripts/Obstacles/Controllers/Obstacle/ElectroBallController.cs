using UnityEngine;

namespace JumpMaster.Obstacles
{
    public class ElectroBallController : ObstacleController<ElectroBall, ElectroBallSO, ElectroBallSpawnSO, ElectroBallSpawnMetricsSO, SpawnArgs>
    {
        public ElectroBallController(ElectroBallSpawnMetricsSO default_spawn_metrics) : base(default_spawn_metrics)
        {

        }

        protected override bool CanSpawn()
        {
            throw new System.NotImplementedException();
        }

        protected override SpawnArgs GenerateSpawnArguments()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnUpdateData()
        {
            throw new System.NotImplementedException();
        }
    }
}