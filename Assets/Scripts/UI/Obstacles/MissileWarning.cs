using System.Collections;

using UnityEngine;
using UnityEngine.UI;

using JumpMaster.Structure;
using JumpMaster.Obstacles;
using JumpMaster.UI.Data;

namespace JumpMaster.UI
{
    public struct MissileWarningArgs
    {
        public readonly Vector2 ScreenPosition;

        public readonly MissileDirection Direction;

        public MissileWarningArgs(Vector2 screen_position, MissileDirection direction)
        {
            ScreenPosition = screen_position;

            Direction = direction;
        }
    }

    public delegate void MissileWarningEventHandler();

    public class MissileWarning : Initializable
    {
        public event MissileWarningEventHandler OnWarningEnded;

        // ##### GENERATION ##### \\

        private MissileWarningSO _data;
        private static MissileWarningSO s_data;

        public static MissileWarning Generate(MissileWarningSO data)
        {
            GameObject gameObject = Instantiate(data.Prefab);
            MissileWarning generated = gameObject.GetComponent<MissileWarning>();

            gameObject.SetActive(false);

            s_data = data;

            generated.Initialize();

            return generated;
        }

        protected override void Initialize()
        {
            _data = s_data;

            Cache();

            SetupRectTransform();

            c_image.enabled = false;

            _flashInterval = _data.FlashIntervalMS / 1000f;

            Initialized = true;
        }

        private void Start()
        {
            if (_data != null)
                return;
            Debug.LogError("Please use the Generate function for creating Missile Warnings!");
            enabled = false;
        }

        // ##### ACTIONS ##### \\

        public bool Playing { get; private set; }

        public void Play(MissileWarningArgs args)
        {
            if (Playing)
                return;

            c_rect.anchoredPosition = GetMarginedScreenPosition(args.ScreenPosition, args.Direction);

            gameObject.SetActive(true);

            Playing = true;

            _flashes = 0;
            _flashCoroutine = StartCoroutine(Flash());
        }

        public void Stop()
        {
            if (!Playing)
                return;

            OnWarningEnded = null;

            if (_flashCoroutine != null)
            {
                StopCoroutine(_flashCoroutine);
                _flashCoroutine = null;
            }

            c_image.enabled = false;

            gameObject.SetActive(false);

            Playing = false;
        }

        private Vector2 GetMarginedScreenPosition(Vector2 screen_position, MissileDirection direction)
        {
            Vector2 margined_screen_position = screen_position;
            switch (direction)
            {
                case MissileDirection.UP:
                    margined_screen_position.y += _data.ScreenMargin;
                    break;

                case MissileDirection.DOWN:
                    margined_screen_position.y -= _data.ScreenMargin;
                    break;

                case MissileDirection.LEFT:
                    margined_screen_position.x -= _data.ScreenMargin;
                    break;

                case MissileDirection.RIGHT:
                    margined_screen_position.x += _data.ScreenMargin;
                    break;
            }
            return margined_screen_position;
        }

        // ##### FLASH ##### \\

        private int _flashes;
        private float _flashInterval;

        private Coroutine _flashCoroutine;
        private IEnumerator Flash()
        {
            _flashes++;
            c_image.enabled = true;

            yield return new WaitForSecondsPausable(_flashInterval);

            if (_flashes == _data.FlashIntervals)
            {
                OnWarningEnded?.Invoke();
                Stop();
            }
            else
            {
                c_image.enabled = false;

                yield return new WaitForSecondsPausable(_flashInterval);

                yield return Flash();
            }
        }

        // ##### CACHE ##### \\

        private RawImage c_image;

        private RectTransform c_rect;

        private void Cache()
        {
            c_image = GetComponent<RawImage>();
            c_rect = GetComponent<RectTransform>();
        }

        private void SetupRectTransform()
        {
            c_rect.SetParent(GameObject.Find("UI_CANVAS").GetComponent<RectTransform>(), false);
            c_rect.localScale = Vector3.one;
        }
    }
}