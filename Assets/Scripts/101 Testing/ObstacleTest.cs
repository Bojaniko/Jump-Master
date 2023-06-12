using UnityEngine;

using JumpMaster.Obstacles;

namespace JumpMaster.Testing
{
    [DefaultExecutionOrder(-1)]
    public class ObstacleTest : MonoBehaviour
    {
        [SerializeField] private ObstacleSO _obstacleData;
        [SerializeField] private SpawnSO _spawnData;

        private IObstacle _obstacle;
        private GameObject _obstacleGameObject;

        private void Awake()
        {
            if (ObstacleLevelController.Instance != null &&
                ObstacleLevelController.Instance.gameObject.activeSelf &&
                ObstacleLevelController.Instance.enabled)
            {
                Debug.LogError("To test obstacles you need to disable the obstacle level controller.");
                gameObject.SetActive(false);
            }
        }

        public void GenerateTestObstacle()
        {
            if (_obstacle != null && _obstacle.ObstacleDataType.Equals(_obstacleData.GetType()))
            {
                Debug.LogWarning("You are trying to generate the same obstacle.");
                return;
            }

            if (_obstacleData == null)
            {
                Debug.LogError("To test obstacles you need to attach an obstacle data block.");
                return;
            }
            if (_spawnData == null)
            {
                Debug.LogError("To test obstacles you need to attach an obstacle spawn data block.");
                return;
            }
            if (!_obstacleData.ObstaclePrefab.GetComponent<IObstacle>().SpawnDataType.Equals(_spawnData.GetType()))
            {
                Debug.LogError("The spawn data doesn't match the obstacle data.");
                return;
            }

            if (_obstacleGameObject != null)
                Destroy(_obstacleGameObject);

            _obstacleGameObject = Instantiate(_obstacleData.ObstaclePrefab);
            _obstacle = _obstacleGameObject.GetComponent<IObstacle>();
            _obstacle.Generate(_obstacleData);
        }

        public void SpawnTestObstacle()
        {
            if (_obstacle == null)
            {
                Debug.LogError("First generate the obstacle.");
                return;
            }
            //_obstacle.Spawn();
        }
    }
}