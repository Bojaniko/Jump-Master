using System.Collections.Generic;

using UnityEngine;

using JumpMaster.LevelTrackers;

namespace JumpMaster.Obstacles
{
    public class ObstacleSpawnPointTracker
    {
        private delegate void ScreenPointActiveStateEventHandler(SpawnPoint point, bool active);
        private sealed class SpawnPoint
        {
            public Vector2 Point => _point;
            private readonly Vector2 _point;

            public bool Active => _active;
            private bool _active;

            public event ScreenPointActiveStateEventHandler OnActiveStateChanged;

            public SpawnPoint(Vector2 point)
            {
                _point = point;
                _active = false;
            }

            private void DeactivePoint()
            {
                _active = false;
                OnActiveStateChanged?.Invoke(this, false);
            }

            public void UseSpawnPoint(float cooldown_time)
            {
                if (_active)
                    return;
                _active = true;
                OnActiveStateChanged?.Invoke(this, true);
                TimeTracker.Instance.StartTimeTracking(DeactivePoint, cooldown_time);
            }
        }

        private delegate void ScreenPointsTrackerAvailableStateEventHandler(SpawnPointsTracker tracker, bool available);
        public enum EdgePosition { HORIZONTAL_TOP, HORIZONTAL_BOTTOM, VERTICAL_LEFT, VERTICAL_RIGHT }
        private sealed class SpawnPointsTracker
        {
            public event ScreenPointsTrackerAvailableStateEventHandler OnAvailabilityStateChange;

            public bool HasAvailablePoints => _availablePoints.Count > 0;

            public EdgePosition Position => _edgePosition;
            private readonly EdgePosition _edgePosition;

            private SpawnPoint[] _screenPoints;

            private readonly List<SpawnPoint> _availablePoints;
            private void HandleScreenPointState(SpawnPoint point, bool active)
            {
                if (active)
                {
                    _availablePoints.Remove(point);
                    if (_availablePoints.Count == 0)
                        OnAvailabilityStateChange?.Invoke(this, false);
                }
                else
                {
                    _availablePoints.Add(point);
                    if (_availablePoints.Count == 1)
                        OnAvailabilityStateChange?.Invoke(this, true);
                }
            }

            public bool TryGetRandomScreenPoint(out Vector2 screen_point, float cooldown_time)
            {
                if (_availablePoints.Count == 0)
                {
                    screen_point = Vector2.zero;
                    return false;
                }

                int target = Random.Range(0, _availablePoints.Count);
                screen_point = _availablePoints[target].Point;
                _availablePoints[target].UseSpawnPoint(cooldown_time);
                return true;
            }

            public SpawnPointsTracker(EdgePosition edge_position, int number_of_points)
            {
                _edgePosition = edge_position;
                _availablePoints = new();

                GeneratePoints(number_of_points);
            }

            private void GeneratePoints(int number_of_points)
            {
                _screenPoints = new SpawnPoint[number_of_points];
                for (int p = 0; p < number_of_points; p++)
                {
                    if (_edgePosition.Equals(EdgePosition.HORIZONTAL_TOP))
                    {
                        float horizontalPointDistance = Screen.width / (float)number_of_points;
                        _screenPoints[p] = new(new Vector2((horizontalPointDistance * p) + (horizontalPointDistance * 0.5f), Screen.height));
                    }
                    else if (_edgePosition.Equals(EdgePosition.HORIZONTAL_BOTTOM))
                    {
                        float horizontalPointDistance = Screen.width / (float)number_of_points;
                        _screenPoints[p] = new(new Vector2((horizontalPointDistance * p) + (horizontalPointDistance * 0.5f), 0f));
                    }
                    else if (_edgePosition.Equals(EdgePosition.VERTICAL_LEFT))
                    {
                        float verticalPointDistance = Screen.height / (float)number_of_points;
                        _screenPoints[p] = new(new Vector2(0f, (verticalPointDistance * p) + (verticalPointDistance * 0.5f)));
                    }
                    else if (_edgePosition.Equals(EdgePosition.VERTICAL_RIGHT))
                    {
                        float verticalPointDistance = Screen.height / (float)number_of_points;
                        _screenPoints[p] = new(new Vector2(Screen.width, (verticalPointDistance * p) + (verticalPointDistance * 0.5f)));
                    }
                    _screenPoints[p].OnActiveStateChanged += HandleScreenPointState;
                }
                _availablePoints.AddRange(_screenPoints);
            }
        }

        public int HorizontalPoints => _horizontalPoints;
        private readonly int _horizontalPoints;

        public int VerticalPoints => _verticalPoints;
        private readonly int _verticalPoints;

        private readonly List<SpawnPointsTracker> _trackers;
        private readonly List<SpawnPointsTracker> _availableTrackers;

        public ObstacleSpawnPointTracker(int horizontal_points, int vertical_points, Vector2 aspect_ratio_multiplier)
        {
            _horizontalPoints = Mathf.FloorToInt(horizontal_points * aspect_ratio_multiplier.x);
            _verticalPoints = Mathf.FloorToInt(vertical_points * aspect_ratio_multiplier.y);

            _trackers = new(4);
            _availableTrackers = new(4);
            _selectedTrackers = new();
            GenerateTrackers();
        }

        private void GenerateTrackers()
        {
            _trackers.Add(new(EdgePosition.HORIZONTAL_TOP, _horizontalPoints));
            _trackers.Add(new(EdgePosition.HORIZONTAL_BOTTOM, _horizontalPoints));
            _trackers.Add(new(EdgePosition.VERTICAL_LEFT, _verticalPoints));
            _trackers.Add(new(EdgePosition.VERTICAL_RIGHT, _verticalPoints));

            _availableTrackers.AddRange(_trackers);
            foreach (SpawnPointsTracker tracker in _trackers)
                tracker.OnAvailabilityStateChange += HandleTrackerAvailabilityState;
        }

        private void HandleTrackerAvailabilityState(SpawnPointsTracker tracker, bool available)
        {
            if (available)
                _availableTrackers.Add(tracker);
            else
                _availableTrackers.Remove(tracker);
        }

        /// <summary>
        /// A random point along a specific edge.
        /// </summary>
        /// <param name="edge_position">The edge for which the point is taken from.</param>
        /// <param name="cooldown_time">The cooldown time until the point can be used again.</param>
        /// <param name="screen_position">The output screen position of a point along the edge.</param>
        /// <returns>true if there is an available point on the given edge.</returns>
        public bool TryGetRandomPoint(EdgePosition edge_position, float cooldown_time, out Vector2 screen_position)
        {
            foreach (SpawnPointsTracker spt in _trackers)
            {
                if (spt.Position.Equals(edge_position))
                    return spt.TryGetRandomScreenPoint(out screen_position, cooldown_time);
            }
            screen_position = Vector2.zero;
            return false;
        }

        /// <summary>
        /// A random point along any edge.
        /// </summary>
        /// <param name="cooldown_time">The cooldown time until the point can be used again.</param>
        /// <param name="screen_position">The output screen position of a point along a random edge.</param>
        /// <param name="edge_position">The output edege position.</param>
        /// <returns>true if there is an available point on any given edge.</returns>
        public bool TryGetAnyRandomPoint(float cooldown_time, out Vector2 screen_position, out EdgePosition edge_position)
        {
            int target = Random.Range(0, _availableTrackers.Count);
            edge_position = _availableTrackers[target].Position;
            return _availableTrackers[target].TryGetRandomScreenPoint(out screen_position, cooldown_time);
        }

        private readonly List<SpawnPointsTracker> _selectedTrackers;
        /// <summary>
        /// A random point along a random edge from a given array.
        /// </summary>
        /// <param name="cooldown_time">The cooldown time until the point can be used again.</param>
        /// <param name="screen_position">The output screen position of a point along a random edge.</param>
        /// <param name="edge_position">The output edege position.</param>
        /// <param name="edge_positions">The array of edge positions for which to choose a point.</param>
        /// <returns>true if there is an available point on the given edges.</returns>
        public bool TryGetRandomPointForEdges(float cooldown_time, out Vector2 screen_position, out EdgePosition edge_position, params EdgePosition[] edge_positions)
        {
            _selectedTrackers.Clear();
            foreach (SpawnPointsTracker spt in _trackers)
            {
                for (int i = 0; i < edge_positions.Length && i < 4; i++)
                {
                    if (spt.Position.Equals(edge_positions[i]) && !_selectedTrackers.Contains(spt) && spt.HasAvailablePoints)
                        _selectedTrackers.Add(spt);
                }
            }

            if (_selectedTrackers.Count == 0)
            {
                screen_position = Vector2.zero;
                edge_position = EdgePosition.HORIZONTAL_TOP;
                return false;
            }

            int target = 0;
            if (_selectedTrackers.Count > 1)
                target = Random.Range(0, _selectedTrackers.Count);

            edge_position = _selectedTrackers[target].Position;
            return _selectedTrackers[target].TryGetRandomScreenPoint(out screen_position, cooldown_time);
        }
    }
}