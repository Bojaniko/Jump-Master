using UnityEngine;

namespace JumpMaster.Obstacles
{
    [CreateAssetMenu(fileName = "Controllers Data", menuName = "Game/Obstacles/Controllers/Controllers Data")]
    public class ObstacleControllersSO : ScriptableObject
    {
        public ScriptableObject[] SpawnMetricsData { get => _spawnMetricsData; set => _spawnMetricsData = value; }
        [SerializeField] private ScriptableObject[] _spawnMetricsData;

        public ISpawnMetricsSO GetSpawnMetricsForControllerType(System.Type type)
        {
            System.Type controller_sm = GetSpawnMetricsTypeForControllerType(type);
            foreach (ISpawnMetricsSO sm in _spawnMetricsData)
            {
                if (sm.GetType().Equals(controller_sm))
                    return sm;
            }
            return null;
        }

        private System.Type GetSpawnMetricsTypeForControllerType(System.Type type)
        {
            System.Type[] generic_arguments = type.BaseType.GetGenericArguments();
            foreach (System.Type t in generic_arguments)
            {
                if (t.GetInterface("ISpawnMetricsSO") != null)
                    return t;
            }
            return null;
        }

        public ISpawnMetricsSO GetSpawnMetricsForController(in IObstacleController controller)
        {
            System.Type controller_sm = GetSpawnMetricsTypeForController(in controller);
            foreach (ISpawnMetricsSO sm in _spawnMetricsData)
            {
                if (sm.GetType().Equals(controller_sm))
                    return sm;
            }
            return null;
        }

        private System.Type GetSpawnMetricsTypeForController(in IObstacleController controller)
        {
            System.Type[] generic_arguments = controller.GetType().BaseType.GetGenericArguments();
            foreach (System.Type t in generic_arguments)
            {
                if (t.GetInterface("ISpawnMetricsSO") != null)
                    return t;
            }
            return null;
        }
    }
}