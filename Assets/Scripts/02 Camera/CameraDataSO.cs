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
        public Vector2 MarginScreen => aspect_marginScreen;
        private Vector2 aspect_marginScreen;
        [SerializeField, Range(0f, 400f), Tooltip("The screen margin in pixels.")] private float _margin = 200f;

        /// <summary>
        /// The normal speed of camera ascention.
        /// </summary>
        public float AscendingSpeed => ascpet_ascendingSpeed;
        [SerializeField, Range(0.1f, 10f), Tooltip("The normal speed of camera ascention.")] private float _ascendingSpeed = 0.6f;
        private float ascpet_ascendingSpeed;

        /// <summary>
        /// The speed at which the camera moves verticaly when the player jumps over the screen.
        /// </summary>
        public float ReachEdgeSpeed => aspect_reachEdgeSpeed;
        [SerializeField, Range(1f, 50f), Tooltip("The speed at which the camera moves verticaly when the player jumps over the screen.")]
        private float _reachEdgeSpeed = 25f;
        private float aspect_reachEdgeSpeed;

        /// <summary>
        /// The distance from the top of the screen at which the camera increases it's speed to follow the player.
        /// </summary>
        public float MaxScreenHeightPosition => aspect_maxScreenHeightPosition;
        [SerializeField, Range(50, 500), Tooltip("The distance from the top of the screen at which the camera increases it's speed to follow the player.")]
        private float _maxScreenHeightPosition = 450f;
        private float aspect_maxScreenHeightPosition;

        /// <summary>
        /// The change in resolution from the reference resolution.
        /// </summary>
        public Vector2 ResolutionAspectChange => _resolutionAspectChange;
        private Vector2 _resolutionAspectChange;

        private void OnEnable()
        {
            if (Screen.width == ReferenceResolution.x && Screen.height == ReferenceResolution.y)
                _resolutionAspectChange = Vector2.one;
            else
                _resolutionAspectChange = new Vector2(Screen.width / ReferenceResolution.x, Screen.height / ReferenceResolution.y);

            aspect_marginScreen = new Vector2(_margin * _resolutionAspectChange.x, _margin * _resolutionAspectChange.y);

            ascpet_ascendingSpeed = _ascendingSpeed * _resolutionAspectChange.y;
            aspect_reachEdgeSpeed = _reachEdgeSpeed * _resolutionAspectChange.y;
            aspect_maxScreenHeightPosition = _maxScreenHeightPosition * _resolutionAspectChange.y;
        }
    }
}
