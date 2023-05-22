using System.Collections.Generic;

namespace JumpMaster.Obstacles
{
    public class ObstacleMarginTracker
    {
        private List<IObstacle> _allObstacles;

        private readonly List<IObstacle> _topMargin;
        private readonly List<IObstacle> _bottomMargin;
        private readonly List<IObstacle> _leftMargin;
        private readonly List<IObstacle> _rightMargin;

        public IObstacle[] TopMargin => _topMargin.ToArray();
        public IObstacle[] BottomMargin => _bottomMargin.ToArray();
        public IObstacle[] LeftMargin => _leftMargin.ToArray();
        public IObstacle[] RightMargin => _rightMargin.ToArray();

        public ObstacleMarginTracker(in List<IObstacle> all_obstacles)
        {
            _topMargin = new();
            _bottomMargin = new();
            _leftMargin = new();
            _rightMargin = new();

            _allObstacles = all_obstacles;
            BindObstaclesMarginTracker(_allObstacles.ToArray());
        }

        public void UpdateObstacles(in List<IObstacle> all_obstacles)
        {
            _topMargin.Clear();
            _bottomMargin.Clear();
            _leftMargin.Clear();
            _rightMargin.Clear();

            UnbindObstaclesMarginTracker(_allObstacles.ToArray());

            _allObstacles = all_obstacles;
            BindObstaclesMarginTracker(_allObstacles.ToArray());
        }

        private void DefineObstalceMarginPosition(IObstacle obstacle)
        {
            if (obstacle.InTopMargin)
            {
                if (!_topMargin.Contains(obstacle))
                    _topMargin.Add(obstacle);
            }
            else
            {
                if (_topMargin.Contains(obstacle))
                    _topMargin.Remove(obstacle);
            }

            if (obstacle.InBottomMargin)
            {
                if (!_bottomMargin.Contains(obstacle))
                    _bottomMargin.Add(obstacle);
            }
            else
            {
                if (_bottomMargin.Contains(obstacle))
                    _bottomMargin.Remove(obstacle);
            }

            if (obstacle.InLeftMargin)
            {
                if (!_leftMargin.Contains(obstacle))
                    _leftMargin.Add(obstacle);
            }
            else
            {
                if (_leftMargin.Contains(obstacle))
                    _leftMargin.Remove(obstacle);
            }

            if (obstacle.InRightMargin)
            {
                if (!_rightMargin.Contains(obstacle))
                    _rightMargin.Add(obstacle);
            }
            else
            {
                if (_rightMargin.Contains(obstacle))
                    _rightMargin.Remove(obstacle);
            }
        }

        // ##### DESPAWNING ##### \\

        private void RemoveObstacleFromMarginsOnDespawn(IObstacle obstacle)
        {
            if (_topMargin.Contains(obstacle))
                _topMargin.Remove(obstacle);

            if (_bottomMargin.Contains(obstacle))
                _bottomMargin.Remove(obstacle);

            if (_leftMargin.Contains(obstacle))
                _leftMargin.Remove(obstacle);

            if (_rightMargin.Contains(obstacle))
                _rightMargin.Remove(obstacle);
        }

        // ##### BINDING ##### \\

        private void BindObstaclesMarginTracker(IObstacle[] obstacles)
        {
            foreach (IObstacle obstacle in obstacles)
            {
                obstacle.OnMarginPositionChange += DefineObstalceMarginPosition;
                obstacle.OnDespawn += RemoveObstacleFromMarginsOnDespawn;
            }
        }

        private void UnbindObstaclesMarginTracker(IObstacle[] obstacles)
        {
            foreach (IObstacle obstacle in obstacles)
            {
                obstacle.OnMarginPositionChange -= DefineObstalceMarginPosition;
                obstacle.OnDespawn -= RemoveObstacleFromMarginsOnDespawn;
            }
        } 
    }
}