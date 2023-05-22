using UnityEngine;

namespace JumpMaster.CameraControls
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S1104:Fields should not have public accessibility", Justification = "<Pending>")]
    [CreateAssetMenu(fileName = "Camera Data", menuName = "Game/Camera")]
    public class CameraDataSO : ScriptableObject
    {
        public float ZPosition = -15f;

        /// <summary>
        /// The resolution for which the settings apply.
        /// </summary>
        [Tooltip("The resolution for which the settings apply.")] public Vector2 ReferenceResolution;

        /// <summary>
        /// The screen margin in pixels.
        /// </summary>
        public Vector2 MarginScreen => _marginScreen;
        private Vector2 _marginScreen;
        [SerializeField, Range(0f, 400f), Tooltip("The screen margin in pixels.")] private float _margin = 200f;

        /// <summary>
        /// The normal speed of camera ascention.
        /// </summary>
        public float AscendingSpeed => _ascendingSpeed;
        [SerializeField, Range(0.1f, 10f), Tooltip("The normal speed of camera ascention.")] private float _ascendingSpeed = 0.6f;

        /// <summary>
        /// The spped at which the camera moves verticaly when the player jumps over the screen.
        /// </summary>
        public float ReachEdgeSpeed => _reachEdgeSpeed;
        [SerializeField, Range(1f, 10f), Tooltip("The spped at which the camera moves verticaly when the player jumps over the screen.")]
        private float _reachEdgeSpeed = 5f;

        /// <summary>
        /// The distance from the top of the screen at which the camera increases it's speed to follow the player.
        /// </summary>
        public float MaxScreenHeightPosition => _maxScreenHeightPosition;
        [SerializeField, Range(50, 500), Tooltip("The distance from the top of the screen at which the camera increases it's speed to follow the player.")]
        private float _maxScreenHeightPosition = 450f;

        /// <summary>
        /// The change in resolution from the reference resolution.
        /// </summary>
        public Vector2 ResolutionAspectChange => _resolutionAspectChange;
        private Vector2 _resolutionAspectChange;

        private void Awake()
        {
            if (Screen.currentResolution.width == ReferenceResolution.x && Screen.currentResolution.height == ReferenceResolution.y)
                _resolutionAspectChange = Vector2.one;
            else
                _resolutionAspectChange = new Vector2(Screen.currentResolution.width / ReferenceResolution.x, Screen.currentResolution.height / ReferenceResolution.y);

            _marginScreen = new Vector2(_margin * _resolutionAspectChange.x, _margin * _resolutionAspectChange.y);

            _ascendingSpeed *= _resolutionAspectChange.y;
            _reachEdgeSpeed *= _resolutionAspectChange.y;
            _maxScreenHeightPosition *= _resolutionAspectChange.y;
        }
    }
}
