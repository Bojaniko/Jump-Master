using UnityEngine;

namespace JumpMaster.CameraControls
{
    public struct Bounding
    {
        public Vector2 Min;
        public Vector2 Max;
        public Vector2 Position;
    }

    [RequireComponent(typeof(BoxCollider2D))]
    public class BoundingBox : MonoBehaviour
    {

        private void Awake()
        {
            Cache();
            ResetTransform();
        }

        void FixedUpdate()
        {
            WorldToScreenBounding(c_boundsCollider, c_camera, ref _boundingScreen);
        }

        private void ResetTransform()
        {
            _boundingScreen = new();
            WorldToScreenBounding(c_boundsCollider, c_camera, ref _boundingScreen);
        }

        // ##### WORLD TO SCREEN ##### \\

        public Vector2 ScreenPosition => _boundingScreen.Position;
        public Vector2 ScreenMin => _boundingScreen.Min;
        public Vector2 ScreenMax => _boundingScreen.Max;

        private Bounding _boundingScreen;
        private void WorldToScreenBounding(in BoxCollider2D bounds_collider, in Camera camera, ref Bounding screen)
        {
            screen.Min = camera.WorldToScreenPoint(bounds_collider.bounds.min);
            screen.Max = camera.WorldToScreenPoint(bounds_collider.bounds.max);

            screen.Position = camera.WorldToScreenPoint(bounds_collider.transform.position);
        }

        // ##### SETTERS ##### \\

        public void SetSize(Vector2 size)
        {
            c_boundsCollider.size = size;
            WorldToScreenBounding(c_boundsCollider, c_camera, ref _boundingScreen);
        }

        // ##### CACHE ##### \\

        private Camera c_camera;

        private BoxCollider2D c_boundsCollider;

        private void Cache()
        {
            c_camera = Camera.main;

            c_boundsCollider = GetComponent<BoxCollider2D>();
            c_boundsCollider.isTrigger = true;
        }
    }
}