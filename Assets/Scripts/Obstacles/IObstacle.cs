namespace JumpMaster.Obstacles
{
    public interface IObstacle<ObstacleScriptableObject, ObstacleController>
        where ObstacleScriptableObject : ObstacleSO
        where ObstacleController : JumpMaster.LevelControllers.Obstacles.ObstacleController
    {
        public ObstacleScriptableObject Data { get; }
        public ObstacleController Controller { get; }
    }
}